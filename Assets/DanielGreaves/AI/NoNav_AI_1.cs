// Created by Daniel Greaves.
// Dont forget, If you change amything then dont forget to comment and say what you did ;)
// Still working on this....

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
//using UnityEditor.Rendering;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.Events;

public class NoNav_AI_1 : MonoBehaviour
{
    // Car body
    public Rigidbody body;


    // Left and right exhaust
    // public GameObject RightExhaust;
    // public GameObject LeftExhaust;

    // Left and right particle system for nitro
    // public GameObject Right_Nitro_Particle_System;
    //public GameObject Left_Nitro_Particle_System;

    // Game objects
    // public GameObject Explosion_Particle_System;

    // Target to find
    public GameObject Target;
   
    // Floats
    private float AIHealth = 300.0f;
    private float AIRange = 500.0f;
    private float CarSpeed = 40.0f;
    private float MaxSteerAngle = 0.0f;
    private float MaxCarAngle = 300.0f;
    private float BreakForce = 1.0f;
    private float VerticalMovement;
    private float CurrentbreakForce;
    private float MissileRaycastRange = 100;

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
    private float AIMovementDelay = 0.0f;

    // Bools
    private bool bBreaking;
    private bool bJumping;
    private bool bFiringProjectile;
    private bool bNitro;
    private bool bSonicBoom;
    private bool bDropMine;
    private bool bFiringMissile;
    private bool bFiringHeatSeekingMissile;
    private bool bWeaponRaycastFoundTarget = false;

    private bool bAIMovement;
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

    private bool bHardRightTurnRaycast = false;
    private bool bHardLeftTurnRaycast = false;
    private bool bMidRightTurnRaycast = false;
    private bool bMidLeftTurnRaycast = false;
    private bool bSoftRightTurnRaycast = false;
    private bool bSoftLeftTurnRaycast = false;
    private bool bCallRightTurn = false;
    private bool bCallLeftTurn = false;
    

    // Car wheel references 
    [SerializeField] private WheelCollider FrontLeftWheelCollider;
    [SerializeField] private WheelCollider FrontRightWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    [SerializeField] private Transform FrontLeftWheelTransform;
    [SerializeField] private Transform FrontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransform;
    [SerializeField] private Transform RearRightWheelTransform;

    // Sound Effects
    // public AudioSource PlayerProjectile_1;
    // public AudioSource HydraulicJumpSound;
    // public AudioSource NitroSound;
    // public AudioSource MissileSound;
    // public AudioSource HeatSeekingMissileSound_1;
    // public AudioSource SonicBoomSound_1;
    // public AudioSource DropMineSound;
    // public AudioSource MissileExplosionSound;
    // public AudioSource BulletHitCarSound;


    public float TargetSpeed = 5.0f;

    // Start is called before the first frame update
    private void Start()
    {
        // Car body
        body = GetComponent<Rigidbody>();
    }

    // Fixed update
    private void FixedUpdate()
    {
        AIMovement();
        CarSpeedFunction();
        SteeringFunction();
        UpdateWheels();
        OnEnable();
        OnDisable();

        // Move to target
        if (Target == null)
        {
            // Then find target
            FindTarget();
        }

        // Wheel rotation
        Quaternion FrontWheelsRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,FrontWheelsRotation,  MaxSteerAngle * Time.deltaTime);

        // Car body rotation
        Quaternion CarRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, CarRotation, MaxCarAngle * Time.deltaTime);

        // Move to target
        Vector3 Placement = Vector3.MoveTowards(transform.position, Target.transform.position, TargetSpeed * Time.deltaTime);
        body.MovePosition(Placement);
    }

    // Find target on game start
    void FindTarget()
    {
        Target = GameObject.FindWithTag("Player");
    }

    // Reset this delay and set its bool to false
    void ResetDropMine()
    {
        bDropMine = false;
    }

    // Reset this delay and set its bool to false
    void ResetSonicBoom()
    {
        bSonicBoom = false;
    }

    // Reset this delay and set its bool to false
    void ResetProjectileFire()
    {
        bFiringProjectile = false;
    }

    // Reset this delay and set its bool to false
    void ResetMissileFire()
    {
        bFiringMissile = false;
    }

    // Reset this delay and set its bool to false
    void ResetHeatSeekingMissile()
    {
        bFiringHeatSeekingMissile = false;
    }

    // Reset this delay and set its bool to false
    void ResetJump()
    {
        bJumping = false;
    }

    // Reset this delay and set its bool to false
    void ResetNitro()
    {
        bNitro = false;

        CarSpeed = 2000;
    }

  
    // AI do random Turn function
    private void RandomTurn()
    {
        int RandomMovement = Random.Range(0, 1);
        switch (RandomMovement)
        {
            case 0:
             CallRandomRightTurn();
                break;
            case 1:
              CallRandomLeftTurn();
                break;
        
            default: break;
        }
    }

    private void CallRandomRightTurn()
    {
        bCallRightTurn = true;
    }

    private void CallRandomLeftTurn()
    {
        bCallLeftTurn = true;
    }

    // AI movement
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
      
    }

    // Dean feel free to chnage the values of the steering rotation as some of the values may not be the same as they are in my test project
    // AI steering function
    private void SteeringFunction()
    {

        // If rotation is more then 5
        if (bAITurnLeft5Angle == false && transform.rotation.eulerAngles.z > 5)
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

        // else if rotation is more then -5 then rotate in the other direction
        else if (bAITurnRight5Angle == false && transform.rotation.eulerAngles.z > -5)
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
        if (bAITurnLeft10Angle == false && transform.rotation.eulerAngles.z > 10)
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

        // else if rotation is is more then -10 then rotate in the other direction
        else if (bAITurnRight10Angle == false && transform.rotation.eulerAngles.z > -10)
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
        if (bAITurnLeft20Angle == false && transform.rotation.eulerAngles.z > 20)
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

        // else if rotation is is more then -20 then rotate in the other direction
        else if (bAITurnRight20Angle == false && transform.rotation.eulerAngles.z > -20)
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
        if (bAITurnLeft30Angle == false && transform.rotation.eulerAngles.z > 30)
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

        // else if rotation is more then -30 then rotate in the other direction
        else if (bAITurnRight30Angle == false && transform.rotation.eulerAngles.z > -30)
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
        if (bAITurnLeft40Angle == false && transform.rotation.eulerAngles.z > 40)
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

        // else if rotation is more then -40 then rotate in the other direction
        else if (bAITurnRight40Angle == false && transform.rotation.eulerAngles.z > -40)
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
        if (bAITurnLeft45Angle == false && transform.rotation.eulerAngles.z > 45)
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

        // else if rotation is more then -45 then rotate in the other direction
        else if (bAITurnRight45Angle == false && transform.rotation.eulerAngles.z > -45)
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

        // Else if rotation is between 4 and -4. Then set fron wheel rotation to 0
        else if (transform.rotation.eulerAngles.z < 4 && transform.rotation.eulerAngles.z < -4)
        {
            // Set delay
            if (bAIMovement) return;
            bAIMovement = true;

            // Then rotate front wheels to 0
            FrontLeftWheelCollider.steerAngle = 0;
            FrontRightWheelCollider.steerAngle = 0;

            // Call this function
            Invoke("AIMoving", AIMovementDelay);
        }

        //////////////////////////////////////////////////////////
       
        if(bCallRightTurn == true)
        {
            //  rotate front wheels to 45
            FrontLeftWheelCollider.steerAngle = 45;
            FrontRightWheelCollider.steerAngle = 45;
        }

        if(bCallLeftTurn == true)
        {
            //  rotate front wheels to -45
            FrontLeftWheelCollider.steerAngle = -45;
            FrontRightWheelCollider.steerAngle = -45;
        }
    }


    public void SoftRightTurn(Collider collider)
    {
        // Then rotate front wheels to 20
        FrontLeftWheelCollider.steerAngle = 20;
        FrontRightWheelCollider.steerAngle = 20;
    }

    public void SoftLeftTurn(Collider collider)
    {
        // Then rotate front wheels to -20
        FrontLeftWheelCollider.steerAngle = -20;
        FrontRightWheelCollider.steerAngle = -20;
    }

    public void MidRandomTurn(Collider collider)
    {
        
        RandomTurn();
    }

    public void HardRightTurn(Collider collider)
    {
        //  rotate front wheels to 45
        FrontLeftWheelCollider.steerAngle = 45;
        FrontRightWheelCollider.steerAngle = 45;
    }

    public void HardLeftTurn(Collider collider)
    {
        //  rotate front wheels to -45
        FrontLeftWheelCollider.steerAngle = -45;
        FrontRightWheelCollider.steerAngle = -45;
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

    // Reset this delay and set its bool to false
    private void AIMoving()
    {

        bAIMovement = false;
    }

    // Reset this delay and set its bool to false
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

    [SerializeField] public Detector SoftRightTurnArea;
    [SerializeField] public Detector SoftLeftTurnArea;
    [SerializeField] public Detector MidRandomTurnArea;
    [SerializeField] public Detector HardRightTurnArea;
    [SerializeField] public Detector HardLeftTurnArea;


    private void OnEnable()
    {
        SoftRightTurnArea.onTriggerEnter.AddListener(SoftRightTurn);
       // SoftRightTurnArea.onTriggerExit.AddListener(SoftRightTurn);
        SoftLeftTurnArea.onTriggerEnter.AddListener(SoftRightTurn);
       // SoftLeftTurnArea.onTriggerExit.AddListener(SoftRightTurn);
        MidRandomTurnArea.onTriggerEnter.AddListener(MidRandomTurn);
       // HardRightTurnArea.onTriggerExit.AddListener(HardRightTurn);
       
      //  MidLeftTurnArea.onTriggerExit.AddListener(MidRightTurn);
        HardRightTurnArea.onTriggerEnter.AddListener(HardRightTurn);
      //  HardRightTurnArea.onTriggerExit.AddListener(HardRightTurn);
        HardLeftTurnArea.onTriggerEnter.AddListener(HardRightTurn);
       // HardLeftTurnArea.onTriggerExit.AddListener(HardRightTurn);
    }

    private void OnDisable()
    {
       // SoftRightTurnArea.onTriggerEnter.RemoveListener(SoftRightTurn);
       // SoftRightTurnArea.onTriggerExit.RemoveListener(SoftRightTurn);
       // SoftLeftTurnArea.onTriggerEnter.RemoveListener(SoftRightTurn);
        //SoftLeftTurnArea.onTriggerExit.RemoveListener(SoftRightTurn);
        //MidRightTurnArea.onTriggerEnter.RemoveListener(MidRightTurn);
       // HardRightTurnArea.onTriggerExit.RemoveListener(HardRightTurn);
        //MidLeftTurnArea.onTriggerEnter.RemoveListener(MidRightTurn);
        //MidLeftTurnArea.onTriggerExit.RemoveListener(MidRightTurn);
       // HardRightTurnArea.onTriggerEnter.RemoveListener(HardRightTurn);
       //////// HardRightTurnArea.onTriggerExit.RemoveListener(HardRightTurn);
      //  HardLeftTurnArea.onTriggerEnter.RemoveListener(HardRightTurn);
       // HardLeftTurnArea.onTriggerExit.RemoveListener(HardRightTurn);
    }

}

