/**************************************************************************************************************
* AI_Nav_Movement_1
* 
* Created by Daniel Greaves 2023
* 
* Change Log:
* 
* Daniel - first iteration of AI using AI Unity navigation system
* 
* Daniel - For AI to chase other players or other AI in a sequence using a timer. Order of the objects are created in the editor and added there using the public Transform [] Players;
* 
* Daniel - Ground layer must be layer called (Hill) for the cars to be able to rotate upwards and down. See on line 211
*
* Armin - 11/08/23 - Added dynamic targeting and therefore multiplayer support
* This AI needs the Unity AI navigation system to work and must be assigned targets in the editor
*       The above is no longer true FOR THIS SCRIPT. The targets are now assigned dynamically.
***************************************************************************************************************/

using Assets.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class AI_Nav_Movement_1 : NetworkBehaviour
{
    // NavMesh
    public NavMeshAgent navMeshAgent;
    public LayerMask PlayerMask = 90;
    public LayerMask ObstacleMask;

    public Rigidbody body;
    public Transform [] Players;

    // Floats
    public float CarSpeed = 40;
    private float FindPlayerTimer = 900.0f;
    private float MaxSteerAngle = 4.50f;
    private float VerticalMovement;
    private float CurrentbreakForce;
    private float BreakForce = 1.0f;
    private float AITurnRight5AngleDelay = .5f;
    private float AITurnLeft5AngleDelay = .5f;
    private float AITurnRight10AngleDelay = 1.0f;
    private float AITurnLeft10AngleDelay = 1.0f;
    private float AITurnRight20AngleDelay = 1.50f;
    private float AITurnLeft20AngleDelay = 1.50f;
    private float AITurnRight30AngleDelay = 2.0f;
    private float AITurnLeft30AngleDelay = 2.0f;
    private float AITurnRight40AngleDelay = 2.5f;
    private float AITurnLeft40AngleDelay = 2.5f;
    private float AITurnRight45AngleDelay = 2.5f;
    private float AITurnLeft45AngleDelay = 2.5f;

    private float AIApplyBreaking = 10.0f;
    private float DetectionRaycastRange = 0.0f;
    private float MaxRayCastDistance = 5.0f;
    private float SlopeRotChangeSpeed = 100f;

    // Ints
    int CurrentPlayerIndex;
 
    // Bools
    private bool bBreaking;
    private bool bDetectionRaycast;
    private bool bAITurnRight5Angle;
    private bool bAITurnLeft5Angle;
    private bool bAITurnRight10Angle;
    private bool bAITurnLeft10Angle;
    private bool bAITurnRight20Angle;
    private bool bAITurnLeft20Angle;
    private bool bAITurnRight30Angle;
    private bool bAITurnLeft30Angle;
    private bool bAITurnRight40Angle;
    private bool bAITurnLeft40Angle;
    private bool bAITurnRight45Angle;
    private bool bAITurnLeft45Angle;

    // Car wheel references 
    [SerializeField] private WheelCollider FrontLeftWheelCollider;
    [SerializeField] private WheelCollider FrontRightWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    [SerializeField] private Transform FrontLeftWheelTransform;
    [SerializeField] private Transform FrontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransform;
    [SerializeField] private Transform RearRightWheelTransform;

    // Game Objects
    [SerializeField] public GameObject DetectionRaycast;
    [SerializeField] public GameObject RotationalPoint;

    // Vectors
    private Vector3 Offset;

    private NetworkAI _networkAiInParent;
    public float RetargetDelay = 10f;
    private Transform _target;
    public NetworkVariable<bool> aiHasControl = new NetworkVariable<bool>(false);
    
    private GameObject _playerHolder;
    private GameObject _aiHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;

        // Car body
        body = GetComponent<Rigidbody>();

        CurrentPlayerIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = CarSpeed;
        
        _playerHolder = GameObject.FindWithTag("PlayerHolder");
        _aiHolder = GameObject.FindWithTag("AIHolder");
    }

    private void OnEnable()
    {
        GameMode.Instance.StartTimer.TimerFinishedEvent += _StartTargeting;
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        GameMode.Instance.StartTimer.TimerFinishedEvent -= _StartTargeting;
    }
    
    private void _StartTargeting()
    {
        _networkAiInParent = GetComponentInParent<NetworkAI>();
        StartCoroutine(_TargetingCoroutine());
    }

    private void _ForceRetarget()
    {
        StopAllCoroutines();
        StartCoroutine(_TargetingCoroutine());
    }
    
    private IEnumerator _TargetingCoroutine()
    {
        while (true)
        {
            if (aiHasControl.Value)
            {
                print("finding target");
                _target = _GetRandomPlayerTarget(_playerHolder, _aiHolder).transform;
                _target.GetComponentInChildren<HealthManager>().DeathEvent += _ForceRetarget;
                yield return new WaitForSeconds(RetargetDelay);
                _target.GetComponentInChildren<HealthManager>().DeathEvent -= _ForceRetarget;
            }  
            else yield return new WaitForSeconds(2);
        }
    }
    
    // AI Rotation
    void AIRotation()
    {
       Vector3 LookRotation = navMeshAgent.steeringTarget - transform.position;
       transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookRotation), MaxSteerAngle * Time.deltaTime);
    }
   
    // Update
    private void FixedUpdate()
    {
        // server only method 
        if (!IsServer || !aiHasControl.Value) return;

        AIMovement();
        _MoveToTarget();
        CarSpeedFunction();
        SteeringFunction();
        UpdateWheels();
        AIRotation();

        // Update agent values
        navMeshAgent.angularSpeed = 0;
        navMeshAgent.updateRotation = false;
        navMeshAgent.acceleration = 0;
        navMeshAgent.autoTraverseOffMeshLink = false;
       
        // If the agent is over link
        if (navMeshAgent.isOnOffMeshLink)
        {
            // Lower car speed for the instance we use the link. As using the nav link speeds up the car for a short time
            CarSpeed = 18;

            OffMeshLinkData data = navMeshAgent.currentOffMeshLinkData;

            // Calculate the final point of the link
            Vector3 EndPosistion = data.endPos + Vector3.up * navMeshAgent.baseOffset;
           
            // Move the agent to the end point
            navMeshAgent.transform.position = Vector3.MoveTowards(navMeshAgent.transform.position, EndPosistion, CarSpeed * Time.deltaTime);

            if (navMeshAgent.transform.position == EndPosistion)
            {
                navMeshAgent.CompleteOffMeshLink();
                CarSpeed = 40;
               
            }
        }

        // Create raycast to detect walls infront of the AI car. When this happens, The AI will apply its breaks and slow down
        Vector3 WallDetection = Vector3.forward;
        Ray LineTrace = new Ray(DetectionRaycast.transform.position, DetectionRaycast.transform.TransformDirection(WallDetection * DetectionRaycastRange));

        // Draw raycast line
        Debug.DrawRay(DetectionRaycast.transform.position, DetectionRaycast.transform.TransformDirection(WallDetection * DetectionRaycastRange));

        // Raycast collision
        if (Physics.Raycast(LineTrace, out RaycastHit WallHit, DetectionRaycastRange))
        {
            // If raycast hits a wall
            if (WallHit.collider.tag == "Wall")
            {
                bDetectionRaycast = true;

                // If raycast is true
                if (bDetectionRaycast == true)
                {
                    if (bBreaking) return;
                    bBreaking = true;
                    Invoke("ApplyBreaking", AIApplyBreaking);

                    CarSpeed = 20;
                }

                // Else if the raycast has no target 
                else if (WallHit.collider.tag == null)
                {
                    
                    bDetectionRaycast = false;

                    bBreaking = false;

                    CarSpeed = 40;
                }

            }
        }
        
        // Get the cars position
        Transform CarTransform = RotationalPoint.transform;
        Vector3 Origin = CarTransform.position;

        // Check if we have hit a hill
        int hillLayerIndex = LayerMask.NameToLayer("Hill");

        // Calculate layermask to raycast too. 
        int layerMask = (1 << hillLayerIndex);

        // Raycast variable
        RaycastHit SlopeHit;

        // Spawn raycast from the cars position down to the ground
        if (Physics.Raycast(Origin + Offset, Vector3.down, out SlopeHit, MaxRayCastDistance, layerMask))
        {
            // Draw linetrace to show the hit point
            Debug.DrawLine(Origin + Offset, SlopeHit.point, Color.red);

            // Set speed for consistant speed for going up hill
            SlopeRotChangeSpeed = 100;

            // Get slope angle from the raycast hit normal then calcuate new pos of the object
            Quaternion newRot = Quaternion.FromToRotation(CarTransform.up, SlopeHit.normal)
               * CarTransform.rotation;

            // Apply the rotation 
            CarTransform.rotation = Quaternion.Lerp(CarTransform.rotation, newRot,
                Time.deltaTime * SlopeRotChangeSpeed);
        }
    }

    private void _MoveToTarget()
    {
        if (!_target) return;
        navMeshAgent.SetDestination(_target.position);
    }
    
    private GameObject _GetRandomPlayerTarget(GameObject playerHolder, GameObject aiHolder)
    {
        List<GameObject> potentialTargets = new List<GameObject>();
        potentialTargets = AddChildrenToList(playerHolder.transform, potentialTargets);
        // potentialTargets = AddChildrenToList(aiHolder.transform, potentialTargets);
        return potentialTargets[Random.Range(0, potentialTargets.Count)];

        List<GameObject> AddChildrenToList(Transform holder, List<GameObject> outputList)
        {
            int thisAisTeamId = _networkAiInParent.teamId.Value;
            // Add all players from opposite team to list
            for (int i = 0; i < holder.childCount; i++)
            {
                Transform child = holder.GetChild(i);
                bool thisTargetDead = child.GetComponentInChildren<HealthManager>().Dead;
                int teamId = child.GetComponentInChildren<TeamMember>().teamId.Value;
                // If on a different team to this ai
                if (teamId != thisAisTeamId && !thisTargetDead)
                {
                    GameObject potentialTargetGameObject = child.GetComponentInChildren<HealthManager>().gameObject;
                    outputList.Add(potentialTargetGameObject);
                }
            }
            return outputList;
        }
    }

    // AI Movement
    private void AIMovement()
    {
        VerticalMovement = CarSpeed;
    }

    // Car speed function for front wheels
    private void CarSpeedFunction()
    {
        // Apply force to the wheels
        FrontLeftWheelCollider.motorTorque = VerticalMovement * CarSpeed;
        FrontRightWheelCollider.motorTorque = VerticalMovement * CarSpeed;

        // Apply force to the breaking if its true
        CurrentbreakForce = bBreaking ? BreakForce : 0f;
        ApplyBreaking();
    }

    // Apply breaking to all four wheels
    private void ApplyBreaking()
    {
        // Apply breaking force to all of the wheel colliders
        FrontRightWheelCollider.brakeTorque = CurrentbreakForce;
        FrontLeftWheelCollider.brakeTorque = CurrentbreakForce;

        bBreaking = false;
    }

    // Update the wheel visuals
    private void UpdateWheels()
    {
        UpdateEachWheel(FrontLeftWheelCollider, FrontLeftWheelTransform);
        UpdateEachWheel(FrontRightWheelCollider, FrontRightWheelTransform);
        UpdateEachWheel(RearRightWheelCollider, RearRightWheelTransform);
        UpdateEachWheel(RearLeftWheelCollider, RearLeftWheelTransform);
    }

    private void UpdateEachWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        // Get wheel collider world position based on its changes
        Vector3 position;
        Quaternion rotation; wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.rotation = rotation;
        wheelTransform.position = position;
    }

    // Dean feel free to change the values of the steering rotation as some of the values may not be the same as they are in my test project
    // AI steering function
    private void SteeringFunction()
    {
        // If rotation is more then 5
        if (RotationalPoint.transform.rotation.eulerAngles.y > 5)
        {
            // Set delay
            if (bAITurnRight5Angle) return;
            bAITurnRight5Angle = true;

            // Then rotate front wheels to 5
            FrontLeftWheelCollider.steerAngle = 5;
            FrontRightWheelCollider.steerAngle = 5;

            // Call this function
            Invoke("AITurnRight5Angle", AITurnRight5AngleDelay);
        }

        // if rotation is more then -5 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y > -5)
        {
            // Set delay
            if (bAITurnLeft5Angle) return;
            bAITurnLeft5Angle = true;

            // Then rotate front wheels to -5
            FrontLeftWheelCollider.steerAngle = -5;
            FrontRightWheelCollider.steerAngle = -5;

            // Call this function
            Invoke("AITurnLeft5Angle", AITurnLeft5AngleDelay);
        }

        // If rotation is more then 10
        if (RotationalPoint.transform.rotation.eulerAngles.y > 10)
        {
            // Set delay
            if (bAITurnRight10Angle) return;
            bAITurnRight10Angle = true;

            // Then rotate front wheels to 10
            FrontLeftWheelCollider.steerAngle = 10;
            FrontRightWheelCollider.steerAngle = 10;

            // Call this function
            Invoke("AITurnRight10Angle", AITurnRight10AngleDelay);
        }

        // if rotation is is more then -10 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y > -10)
        {
            // Set delay
            if (bAITurnLeft10Angle) return;
            bAITurnLeft10Angle = true;

            // Then rotate front wheels to 10
            FrontLeftWheelCollider.steerAngle = -10;
            FrontRightWheelCollider.steerAngle = -10;

            // Call this function
            Invoke("AITurnLeft10Angle", AITurnLeft10AngleDelay);
        }

        // If rotation is more then 20
        if (RotationalPoint.transform.rotation.eulerAngles.y > 20)
        {
            // Set delay
            if (bAITurnRight20Angle) return;
            bAITurnRight20Angle = true;

            // Then rotate front wheels to 20
            FrontLeftWheelCollider.steerAngle = 20;
            FrontRightWheelCollider.steerAngle = 20;

            // Call this function
            Invoke("AITurnRight20Angle", AITurnRight20AngleDelay);
        }

        // if rotation is is more then -20 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y > -20)
        {
            // Set delay
            if (bAITurnLeft20Angle) return;
            bAITurnLeft20Angle = true;

            // Then rotate front wheels to -20
            FrontLeftWheelCollider.steerAngle = -20;
            FrontRightWheelCollider.steerAngle = -20;

            // Call this function
            Invoke("AITurnLeft20Angle", AITurnLeft20AngleDelay);
        }

        // If rotation is more then 30
        if (RotationalPoint.transform.rotation.eulerAngles.y > 30)
        {
            // Set delay
            if (bAITurnRight30Angle) return;
            bAITurnRight30Angle = true;

            // Then rotate front wheels to 30
            FrontLeftWheelCollider.steerAngle = 30;
            FrontRightWheelCollider.steerAngle = 30;

            // Call this function
            Invoke("AITurnRight30Angle", AITurnRight30AngleDelay);
        }

        // if rotation is more then -30 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y > -30)
        {
            // Set delay
            if (bAITurnLeft30Angle) return;
            bAITurnLeft30Angle = true;

            // Then rotate front wheels to -30
            FrontLeftWheelCollider.steerAngle = -30;
            FrontRightWheelCollider.steerAngle = -30;

            // Call this function
            Invoke("AITurnLeft30Angle", AITurnLeft30AngleDelay);
        }

        // If rotation is more then 40
        if (RotationalPoint.transform.rotation.eulerAngles.y > 40)
        {
            // Set delay
            if (bAITurnRight40Angle) return;
            bAITurnRight40Angle = true;

            // Then rotate front wheels to 40
            FrontLeftWheelCollider.steerAngle = 40;
            FrontRightWheelCollider.steerAngle = 40;

            // Call this function
            Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);
        }

        // if rotation is more then -40 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y >-40)
        {
            // Set delay
            if (bAITurnLeft40Angle) return;
            bAITurnLeft40Angle = true;

            // Then rotate front wheels to -40
            FrontLeftWheelCollider.steerAngle = -40;
            FrontRightWheelCollider.steerAngle = -40;


            // Call this function
            Invoke("AITurnLeft40Angle", AITurnLeft40AngleDelay);
        }

        // If rotation is more then 45
        if (RotationalPoint.transform.rotation.eulerAngles.y > 45)
        {
            // Set delay
            if (bAITurnRight45Angle) return;
            bAITurnRight45Angle = true;

            // Then rotate front wheels to 45
            FrontLeftWheelCollider.steerAngle = 45;
            FrontRightWheelCollider.steerAngle = 45;

            // Call this function
            Invoke("AITurnRight45Angle", AITurnRight45AngleDelay);
        }

        // if rotation is more then -45 then rotate in the other direction
        if (RotationalPoint.transform.rotation.eulerAngles.y > -45)
        {
            // Set delay
            if (bAITurnLeft45Angle) return;
            bAITurnLeft45Angle = true;

            // Then rotate front wheels to -45
            FrontLeftWheelCollider.steerAngle = -45;
            FrontRightWheelCollider.steerAngle = -45;

            // Call this function
            Invoke("AITurnLeft45Angle", AITurnLeft45AngleDelay);
        }
    }

    // Reset this delay and set its bool to false
    private void AITurnRight5Angle()
    {
        bAITurnRight5Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft5Angle()
    {
        bAITurnLeft5Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnRight10Angle()
    {
        bAITurnRight10Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft10Angle()
    {
        bAITurnLeft10Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnRight20Angle()
    {
        bAITurnRight20Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft20Angle()
    {
        bAITurnLeft20Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnRight30Angle()
    {
        bAITurnRight30Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft30Angle()
    {
        bAITurnLeft30Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnRight40Angle()
    {
        bAITurnRight40Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft40Angle()
    {
        bAITurnLeft40Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnRight45Angle()
    {
        bAITurnRight45Angle = false;
    }

    // Reset this delay and set its bool to false
    private void AITurnLeft45Angle()
    {
        bAITurnLeft45Angle = false;
    }
}

