/**************************************************************************************************************
* AI Movement
* 
* Created by Daniel Greaves 2023
* 
* Change Log:
* Daniel - AI movement that does not require the unity AI navigation system to naviagte the game world. As it uses raycasts to navigate its surroundings
* This was the first iteration of the AI functionality and works well
***************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    // Car body
    public Rigidbody body;

    // Target to find
    public GameObject Target;

    // Floats
    private float AIRange = 500.0f;
    private float CarSpeed = 40.0f;
    private float MaxSteerAngle = 45.0f;

    private float BreakForce = 1.0f;
    private float VerticalMovement;
    private float CurrentbreakForce;
  
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

    private float HardRightTurnRaycastRange = 30;
    private float HardLeftTurnRaycastRange = 30;
    private float MidRightTurnRaycastRange = 20;
    private float MidLeftTurnRaycastRange = 20;
    private float SoftRightTurnRaycastRange = 10;
    private float SoftLeftTurnRaycastRange = 10;

    // Bools
    private bool bBreaking;

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
  
    // Car wheel references 
    [SerializeField] private WheelCollider FrontLeftWheelCollider;
    [SerializeField] private WheelCollider FrontRightWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    [SerializeField] private Transform FrontLeftWheelTransform;
    [SerializeField] private Transform FrontRightWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransform;
    [SerializeField] private Transform RearRightWheelTransform;

    [SerializeField] public GameObject RotationalPoint;

    public GameObject SoftRightTurnRaycast;
    public GameObject SoftLeftTurnRaycast;
    public GameObject MidRightTurnRaycast;
    public GameObject MidLeftTurnRaycast;
    public GameObject HardRightTurnRaycast;
    public GameObject HardLeftTurnRaycast;

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
        AIRotation();   
        CarSpeedFunction();
        SteeringFunction();
        UpdateWheels();
        FindNextTarget();
        AIObstacleDetetection();

    }

    private void FindNextTarget()
    {
        // Move to target
        if (Target == null)
        {
            // Then find target
            FindTarget();
        }

        // Move to target
        Vector3 Placement = Vector3.MoveTowards(transform.position, Target.transform.position, TargetSpeed * Time.deltaTime);
        body.MovePosition(Placement);
    }

    // AI Rotation
    private void AIRotation()
    {
        // Wheel rotation
        Quaternion FrontWheelsRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, FrontWheelsRotation, 2* MaxSteerAngle * Time.deltaTime);
    }

    // Find target on game start
    private void FindTarget()
    {
       Target = GameObject.FindWithTag("Player");
    }

    private void AIObstacleDetetection()
    {
        // Create raycast for car turning hard left function
        Vector3 HardRightTurningDirection = Vector3.forward;
        Ray HardRightTurningLineTrace = new Ray(HardRightTurnRaycast.transform.position, HardRightTurnRaycast.transform.TransformDirection(HardRightTurningDirection * HardRightTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(HardRightTurnRaycast.transform.position, HardRightTurnRaycast.transform.TransformDirection(HardRightTurningDirection * HardRightTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(HardRightTurningLineTrace, out RaycastHit HardRightHit, HardRightTurnRaycastRange))
        {
            // If raycast hits a AI_Obstacle
            if (HardRightHit.collider.tag == "AI_Obstacle")
            {
                bHardRightTurnRaycast = true;

                // If raycast is true
                if (bHardRightTurnRaycast == true)
                {
                    // Call this function
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = -30;
                    FrontRightWheelCollider.steerAngle = -30;
                }

                // Else if the raycast has no target 
                else if (HardRightHit.collider.tag == null)
                {
                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bHardRightTurnRaycast = false;
                }

            }

        }

        // Create raycast for car turning hard right function
        Vector3 HardLeftTurningDirection = Vector3.forward;
        Ray HardLeftTurningLineTrace = new Ray(HardLeftTurnRaycast.transform.position, HardLeftTurnRaycast.transform.TransformDirection(HardLeftTurningDirection * HardLeftTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(HardLeftTurnRaycast.transform.position, HardLeftTurnRaycast.transform.TransformDirection(HardLeftTurningDirection * HardLeftTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(HardLeftTurningLineTrace, out RaycastHit HardLeftHit, HardLeftTurnRaycastRange))
        {
            // If raycast hits a wall AI_Obstacle
            if (HardLeftHit.collider.tag == "AI_Obstacle")
            {
                bHardLeftTurnRaycast = true;

                // If raycast is true
                if (bHardLeftTurnRaycast == true)
                {
                    // Call this function  
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = 30;
                    FrontRightWheelCollider.steerAngle = 30;
                }

                // Else if the raycast has no target 
                else if (HardRightHit.collider.tag == null)
                {

                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bHardLeftTurnRaycast = false;
                }

            }

        }

        // Create raycast for car turning mid left function
        Vector3 MidRightTurningDirection = Vector3.forward;
        Ray MidRightTurningLineTrace = new Ray(MidRightTurnRaycast.transform.position, MidRightTurnRaycast.transform.TransformDirection(MidRightTurningDirection * MidRightTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(MidRightTurnRaycast.transform.position, MidRightTurnRaycast.transform.TransformDirection(MidRightTurningDirection * MidRightTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(MidRightTurningLineTrace, out RaycastHit MidRightHit, MidRightTurnRaycastRange))
        {
            // If raycast hits a AI_Obstacle
            if (MidRightHit.collider.tag == "AI_Obstacle")
            {
                bMidRightTurnRaycast = true;

                // If raycast is true
                if (bMidRightTurnRaycast == true)
                {
                    // Call this function
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = -20;
                    FrontRightWheelCollider.steerAngle = -20;
                }

                // Else if the raycast has no target 
                else if (MidRightHit.collider.tag == null)
                {

                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bMidRightTurnRaycast = false;
                }

            }

        }

        // Create raycast for car turning mid right function
        Vector3 MidLeftTurningDirection = Vector3.forward;
        Ray MidLeftTurningLineTrace = new Ray(MidLeftTurnRaycast.transform.position, MidLeftTurnRaycast.transform.TransformDirection(MidLeftTurningDirection * MidLeftTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(MidLeftTurnRaycast.transform.position, MidLeftTurnRaycast.transform.TransformDirection(MidLeftTurningDirection * MidLeftTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(MidLeftTurningLineTrace, out RaycastHit MidLeftWallHit, MidLeftTurnRaycastRange))
        {
            // If raycast hits a AI_Obstacle
            if (MidLeftWallHit.collider.tag == "AI_Obstacle")
            {
                bMidLeftTurnRaycast = true;

                // If raycast is true
                if (bMidLeftTurnRaycast == true)
                {
                    // Call this function  
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = 20;
                    FrontRightWheelCollider.steerAngle = 20;

                }

                // Else if the raycast has no target 
                else if (MidRightHit.collider.tag == null)
                {

                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bMidLeftTurnRaycast = false;
                }

            }

        }

        // Create raycast for car turning soft left function
        Vector3 SoftRightTurningDirection = Vector3.forward;
        Ray SoftRightTurningLineTrace = new Ray(SoftRightTurnRaycast.transform.position, SoftRightTurnRaycast.transform.TransformDirection(SoftRightTurningDirection * SoftRightTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(SoftRightTurnRaycast.transform.position, SoftRightTurnRaycast.transform.TransformDirection(SoftRightTurningDirection * SoftRightTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(SoftRightTurningLineTrace, out RaycastHit SoftRightHit, SoftRightTurnRaycastRange))
        {
            // If raycast hits a AI_Obstacle
            if (SoftRightHit.collider.tag == "AI_Obstacle")
            {
                bSoftRightTurnRaycast = true;

                // If raycast is true
                if (bSoftRightTurnRaycast == true)
                {
                    // Call this function
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = -10;
                    FrontRightWheelCollider.steerAngle = -10;

                }

                // Else if the raycast has no target 
                else if (SoftRightHit.collider.tag == null)
                {

                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bSoftRightTurnRaycast = false;
                }

            }

        }

        // Create raycast for car turning soft right function
        Vector3 SoftLeftTurningDirection = Vector3.forward;
        Ray SoftLeftTurningLineTrace = new Ray(SoftLeftTurnRaycast.transform.position, SoftLeftTurnRaycast.transform.TransformDirection(SoftLeftTurningDirection * SoftLeftTurnRaycastRange));

        // Draw raycast line
        Debug.DrawRay(SoftLeftTurnRaycast.transform.position, SoftLeftTurnRaycast.transform.TransformDirection(SoftLeftTurningDirection * SoftLeftTurnRaycastRange));

        // Raycast collision
        if (Physics.Raycast(SoftLeftTurningLineTrace, out RaycastHit SoftLeftHit, SoftLeftTurnRaycastRange))
        {
            // If raycast hits a AI_Obstacle
            if (SoftLeftHit.collider.tag == "AI_Obstacle")
            {
                bSoftLeftTurnRaycast = true;

                // If raycast is true
                if (bSoftLeftTurnRaycast == true)
                {
                   
                    // Call this function  
                    Invoke("AITurnRight40Angle", AITurnRight40AngleDelay);

                    FrontLeftWheelCollider.steerAngle = 10;
                    FrontRightWheelCollider.steerAngle = 10;

                }

                // Else if the raycast has no target 
                else if (SoftRightHit.collider.tag == null)
                {

                    FrontLeftWheelCollider.steerAngle = 0;
                    FrontRightWheelCollider.steerAngle = 0;

                    bSoftLeftTurnRaycast = false;
                }

            }
       
        }

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
        if (RotationalPoint.transform.rotation.eulerAngles.y > -40)
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

}
