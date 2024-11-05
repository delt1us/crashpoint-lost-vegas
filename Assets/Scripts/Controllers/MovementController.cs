/**************************************************************************************************************
* Movement Controller
* Used to manage the player's movements, assigning the frictions (set using the Movement Properties scriptable object) to the tires.
* Responsible for anything to do with the character's movement.
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Dean -  Added basic movement to vehicles. Made so that it can be placed on any 4-wheeled vehicle.
*           Made things like top speed, braking power, engine power wheel frictions..... editable using the Movement Properties scriptable object.
*   Envy -  8/2 Added slow down function for stringers      
*   Dean -  Made the variables involved in the spikes weapon private.
*   Dean -  Simplified how the spike defence weapon work and made it scale with the vary drags of each car
*   Armin - 07/07/23 - Dasher works multiplayer
*   Armin - 01/08/23 - Players cannot move during the countdown
*   Armin - 09/08/23 - Drift boost vfx works multiplayer and speeder works
*   Armin - 10/08/23 - Fixed boost vfx
***************************************************************************************************************/

using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : NetworkBehaviour
{
    private Rigidbody carBody;

    [Header("Configuration")]
    [Tooltip("This controller requires a \"Movement Properties\" scriptable object in order to work. " +
        "\nTo make one: Right-click in the contents folder -> Create -> Car Properties -> Movement")]
    [SerializeField] private MovementProperties moveProperties;

    //// Movements
    [SerializeField, Tooltip("X-axis = The speed of the car \nY-axis = The degree the front wheels should turn")]
    private AnimationCurve steeringCurve;

    private float acceleration;
    private bool reachedTopSpeed;
    private bool isAccelerating;
    private bool isBraking;
    private const float skidThreshold = 25;
    private float currentSpeed;
    private float topSpeed;

    private bool hovering;
    private Hoverer hoverScript;


    //// Frictions
    private WheelFrictionCurve driftCurveSide;
    private WheelFrictionCurve driftCurveFWD;
    private WheelFrictionCurve defaultCurveFWD;
    private WheelFrictionCurve defaultCurveSide;
    private WheelFrictionCurve terrainCurveFWD;
    private WheelFrictionCurve terrainCurveSide;
    private PhysicMaterial groundFriction;
    private PhysicMaterial defaultGroundFriction;


    //// Oil slipping
    [SerializeField, Range(0, 1000)]
    private int oilSpinForce = 100;

    [SerializeField, Range(0, 5), Tooltip("How long the oil effect should last")]
    private float inOilDuration = 2.5f;

    private bool slipInOil;
    private bool oilTimerOn;
    private float oilTimer;


    //// Drifting
    private float driftTimeThreshold;
    private float driftSpeedThreshold;
    private int smallDriftBoost;
    private int bigDriftBoost;
    private float driftTime;

    //// Flip Upright
    [Header("Flip Upright")]
    [SerializeField, Range(0, 1000), Tooltip("For whenever the car topples over.")]
    private int flipForce = 60;

    [SerializeField, Range(.8f, 7), Tooltip("The time (in seconds) the player needs to be upside down in order to do the flip.")]
    private float flipDelay = 2;

    private float flipTimer;
    private bool canFlip;
    private const int flipRpmThreshold = 70;
    private const float flipSpeedThreshold = .0075f;
    private float flipAttempts = 1;

    // The amount of boost currently in the player's tank
    private BoostManager boostManager;

    //// Inputs
    private InputManager inputManager;

    [Header("Points")]
    [SerializeField, Tooltip("The transform of where the boost force should be applied from.")]
    private Transform boostPoint;

    [SerializeField, Tooltip("The transform of the vehicle's center of gravity.\nDON'T TOUCH THIS IF HAPPY WITH THE HANDLING/CONTROL")]
    private Transform centerMass;

    [SerializeField, Tooltip("The transform of the point to apply a force to whenever the player drives in oil.")]
    private Transform spinoutPoint;

    // 0 & 1 == the front wheels     2 & 3 == the back wheels
    [Header("Wheels")]
    [SerializeField, Tooltip("The front wheels have to be in the first 2 index...")]
    private WheelCollider[] wheelColliders;

    [SerializeField, Tooltip("Match the order the wheels are placed in with the Wheel Colliders array")]
    private GameObject[] wheelMeshes;


    [Header("Sound Effects")]
    [SerializeField, Range(0, 8), Tooltip("The index of the engine sfx to play \n" +
        " 0 = Truck-type engine \n 1 = Dirty muscle \n 2 = Aggressive muscle \n 3 = Efficient engine \n 4 = Dirty truck-type engine")]
    private byte engineSFX = 0;

    [Space]
    [SerializeField, Range(5, 35), Tooltip("(in m/s) When the car reaches this speed it will change gears - audibly - Once the gear changes, " +
        "it can go up another gear once it reaches this value again.")]
    private float gearChange = 15;

    [SerializeField, Range(0, 1), Tooltip("A multiplier applied to the pitch of the engine to prevent the pitch from exceeding unrealistic levels.")]
    private float gearPitchDampen = .2f;

    [Space]
    [SerializeField, Range(.5f, 4), Tooltip("The minimum engine pitch while the car is accelerating (while the drive button is being pressed).")]
    private float minEnginePitch = 1.5f;

    [SerializeField, Range(.5f, 10), Tooltip("The maximum engine pitch...")]
    private float maxEnginePitch = 4;

    [SerializeField, Range(.5f, 10), Tooltip("The minimum engine pitch while the car isn't accelerating.")]
    private float idleEnginePitch = .9f;

    [Space]
    [SerializeField, Range(0, 10), Tooltip("The speed of the lerp which occurs whenever the car changes gears")]
    private float engineLerpSpeed = 5f;

    private AudioManager audioManager;

    [Header("Visual Effects")]
    [SerializeField] private VisualEffect[] boostVFX;
    [SerializeField] private VisualEffect[] smokeVFX;
    [SerializeField] private VisualEffect[] driftBoostVFX;

    //// Network
    private NetworkVariable<bool> _isBoostingNetworkVariable = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Space]
    [SerializeField] private PlayerHasControlBoolScriptableObject playerHasControlBoolScriptableObject;

    private void Awake()
    {
        boostManager = new(moveProperties.BoostCapacity, moveProperties.BoostRegenRate, moveProperties.BoostPower, moveProperties.RegenDelay);
    }

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        audioManager = GetComponent<AudioManager>();

        // Creates the engine audio source...
        CreateSFX();
        ConfigureVFX();

        audioManager.PlayIgnition();
        audioManager.PlayEngine();

        // If the selected car is the hoverer.
        if (GetComponent<Hoverer>()) hoverScript = GetComponent<Hoverer>();

        ConfigureCar();
    }

    private void OnEnable()
    {
        _isBoostingNetworkVariable.OnValueChanged += _SetBoostVFX;
    }

    private void OnDisable()
    {
        _isBoostingNetworkVariable.OnValueChanged -= _SetBoostVFX;
    }

    private void FixedUpdate()
    {
        //Gravity();
        AirControl();
        GroundFriction();

        if ((IsServer || IsClient) && !playerHasControlBoolScriptableObject.hasControl) return;

        RotateWheels();
        Accelerate();
        Boost();

        Steering();
        Brake();
        Drift();
        //Burnout();
    }

    private void Update()
    {
        boostManager.Update();
        InOilTime();

        if ((IsServer || IsClient) && !playerHasControlBoolScriptableObject.hasControl) return;
        FlipTimer();
    }

    private void ConfigureCar()
    {
        carBody = GetComponent<Rigidbody>();
        carBody.maxAngularVelocity = moveProperties.MaxTurnSpeed;

        carBody.drag = moveProperties.Drag;
        carBody.angularDrag = moveProperties.TurnResistance;

        if (centerMass) carBody.centerOfMass = centerMass.transform.localPosition;
        SetWheelFriction();

        acceleration = moveProperties.EnginePower;
        driftTimeThreshold = moveProperties.DriftTimeThreshold;
        driftSpeedThreshold = moveProperties.DriftSpeedThreshold;
        smallDriftBoost = moveProperties.SmallDriftBoost;
        bigDriftBoost = moveProperties.BigDriftBoost;
        topSpeed = moveProperties.TopSpeed;
    }

    private void SetWheelFriction()
    {
        // Setting a the default physical material of the ground for the wheel collider to interact with
        defaultGroundFriction = new()
        {
            staticFriction = 1,
            dynamicFriction = 1,
            frictionCombine = PhysicMaterialCombine.Average,
            bounciness = .4f,
            bounceCombine = PhysicMaterialCombine.Average
        };

        defaultCurveFWD = new()
        {
            asymptoteSlip = moveProperties.FWD_MaxSlip,
            extremumSlip = moveProperties.FWD_MinSlip,
            extremumValue = moveProperties.FWD_InitiationForce,
            asymptoteValue = moveProperties.FWD_MinForce,
            stiffness = moveProperties.ForwardGrip
        };

        defaultCurveSide = driftCurveSide = new()
        {
            asymptoteSlip = moveProperties.Side_MaxSlip,
            extremumSlip = moveProperties.Side_MinSlip,
            extremumValue = moveProperties.Side_InitiationForce,
            asymptoteValue = moveProperties.Side_MinForce,
            stiffness = moveProperties.SidewardGrip
        };

        //// Default drift friction values
        driftCurveFWD = new()
        {
            asymptoteSlip = defaultCurveFWD.asymptoteSlip,
            extremumSlip = defaultCurveFWD.extremumSlip,
            extremumValue = defaultCurveFWD.extremumValue,
            asymptoteValue = defaultCurveFWD.asymptoteValue,
            stiffness = moveProperties.ForwardGrip / moveProperties.ForwardDriftMultiplier
        };

        driftCurveSide = new()
        {
            asymptoteSlip = defaultCurveSide.asymptoteSlip,
            extremumSlip = defaultCurveSide.extremumSlip,
            extremumValue = defaultCurveSide.extremumValue,
            asymptoteValue = defaultCurveSide.asymptoteValue,
            stiffness = moveProperties.SidewardGrip / moveProperties.SidewardDriftMultiplier
        };

        foreach (WheelCollider wheel in wheelColliders)
        {
            // Forwards
            wheel.forwardFriction = moveProperties.SetFWDHandling(defaultCurveFWD);

            // Sidewards
            wheel.sidewaysFriction = moveProperties.SetSideHandling(defaultCurveSide);
        }
    }

    private void AirControl()
    {
        if (IsGrounded()) return;

        bool airRoll = inputManager.AirRollAction.ReadValue<float>() > 0;
        Vector2 input = inputManager.AirControlAction.ReadValue<Vector2>();

        // If air roll is on... rotate around the forward axis... otherwise rotate around the up axis
        if (airRoll && !hovering) carBody.AddTorque(transform.forward * (hovering ? moveProperties.YawSpeed * 2 * -input.x : moveProperties.YawSpeed * -input.x));
        else carBody.AddTorque(transform.up * (hovering ? moveProperties.YawSpeed * 2 * input.x : moveProperties.YawSpeed * input.x));

        if (!hovering) carBody.AddTorque(transform.right * moveProperties.PitchSpeed * input.y);
    }

    private void Accelerate()
    {
        // Gets the speed of the car on the forward axis - converts to MPH.
        currentSpeed = Mathf.Abs(Vector3.Dot(carBody.velocity, transform.forward) * 2.2369f);

        float input = inputManager.AccelerateAction.ReadValue<float>();
        isAccelerating = input > 0;

        EngineSounds();


        reachedTopSpeed = currentSpeed > topSpeed;

        // Don't accelerate if the car has reached its top speed.
        if (reachedTopSpeed)
        {
            for (int i = 2; i < wheelColliders.Length; i++) wheelColliders[i].motorTorque = 0;
            return;
        }

        // Ignoring the first 2 wheels since they're the front wheels
        for (int i = 2; i < wheelColliders.Length; i++) wheelColliders[i].motorTorque = input * acceleration / wheelColliders.Length - 2;
        // Splitting the engine power into 2 (since the engine powers both wheels equally).
        //wheelColliders[2].motorTorque = input * moveProperties.EnginePower / 2;
        //wheelColliders[3].motorTorque = input * moveProperties.EnginePower / 2;
    }

    private void EngineSounds()
    {
        float speedInGear = Mathf.Max((Mathf.Abs(GetRawSpeed()) % gearChange) * gearPitchDampen, isAccelerating ? minEnginePitch : idleEnginePitch);
        audioManager.EditPitch("engine", Mathf.Clamp(Mathf.Lerp(audioManager.GetEnginePitch(), speedInGear, Time.deltaTime * engineLerpSpeed), isAccelerating ? minEnginePitch : idleEnginePitch, maxEnginePitch));
        //audioManager.EditPitch("engine", Mathf.Max(Mathf.Lerp(audioManager.GetEnginePitch(), speedInGear, Time.deltaTime * engineLerpSpeed), minEnginePitch));
    }

    private void Boost()
    {
        if (!IsOwner) return;

        float input = inputManager.BoostAction.ReadValue<float>();
        bool canBoost = input > 0 && boostManager.GetCurrentBoost() > 0;

        if (boostVFX.Length > 0 && _isBoostingNetworkVariable.Value != canBoost) _isBoostingNetworkVariable.Value = canBoost;

        if (canBoost) audioManager.PlayBoost();
        else audioManager.StopBoost();


        if (input < 1) return;

        bool grounded = wheelColliders[0].isGrounded && wheelColliders[1].isGrounded;

        boostManager.Boost(carBody, boostPoint, grounded);

    }

    // Called from isBoostingNetworkVariable.OnValueChanged;
    private void _SetBoostVFX(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            foreach (VisualEffect fx in boostVFX) fx.Play();
            foreach (VisualEffect fx in smokeVFX) fx.Stop();
            return;
        }
        foreach (VisualEffect fx in boostVFX) fx.Stop();
        foreach (VisualEffect fx in smokeVFX) fx.Play();
    }
    
    private void Drift()
    {
        float input = inputManager.DriftAction.ReadValue<float>();
        WheelFrictionCurve sideFriction;
        WheelFrictionCurve fwdFriction;

        fwdFriction = input > 0 ? moveProperties.SetFWDHandling(driftCurveFWD) :
            moveProperties.SetFWDHandling(defaultCurveFWD);

        sideFriction = input > 0 ? moveProperties.SetSideHandling(driftCurveSide) :
            moveProperties.SetSideHandling(defaultCurveSide);

        // Every wheel should have the new drifting sideways friction
        foreach (WheelCollider wheel in wheelColliders) wheel.sidewaysFriction = sideFriction;

        // Only the front wheels take the new drifting forwards friction (the back wheels keep their original forward friction).
        wheelColliders[0].forwardFriction = fwdFriction;
        wheelColliders[1].forwardFriction = fwdFriction;


        // Brake in the back wheels
        for (int i = 2; i < wheelColliders.Length; i++) wheelColliders[i].brakeTorque = input * moveProperties.DriftBrakeMultiplier;

        // The angle between the forward direction and the turn direction.
        float driftAmount = Mathf.Abs(Vector3.Dot(carBody.velocity, transform.right));
        float angleThreshold = 10;

        // While the player's speed is over the threshold and drifting passed a certain angle, add to the timer
        if (currentSpeed > driftSpeedThreshold && input > 0 && driftAmount > angleThreshold)
        {
            driftTime += Time.deltaTime;
            if (driftTime > driftTimeThreshold)
            {
                audioManager.PlayDriftingBoost();
                audioManager.EditPitch("drift", 1.5f);
                if (IsOwner) _ToggleDriftBoostVFXServerRpc(true);
            }
        }
        else
        {
            if (audioManager)
            {
                audioManager.StopDriftingBoost();
                audioManager.EditPitch("drift", 1);
            }
            
            if (IsOwner) _ToggleDriftBoostVFXServerRpc(false);
        }

        // Once the player has let go of the drift button and the player has been drifting, attempt to boost the player.
        if (driftTime > 0 && input < 1)
        {
            // Give the current drift time to the drift booster (if there is a time)
            DriftBoost(driftTime);

            // Reset the drift time whenever the player isn't holding the drift button
            driftTime = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void _ToggleBoostVFXServerRpc(bool active)
    {
        _ToggleBoostVFXClientRpc(active);
    }

    [ClientRpc]
    private void _ToggleBoostVFXClientRpc(bool active)
    {
        if (active)
        {
            foreach (VisualEffect fx in boostVFX) fx.Play();
            foreach (VisualEffect fx in smokeVFX) fx.Stop();
        }

        else
        {
            foreach (VisualEffect fx in boostVFX) fx.Stop();
            foreach (VisualEffect fx in smokeVFX) fx.Play();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void _ToggleDriftBoostVFXServerRpc(bool active)
    {
        _ToggleDriftBoostVFXClientRpc(active);
    }

    [ClientRpc]
    private void _ToggleDriftBoostVFXClientRpc(bool active)
    {
        if (active && IsGrounded())
            foreach (VisualEffect fx in driftBoostVFX) fx.Play();
        else foreach (VisualEffect fx in driftBoostVFX) fx.Stop();
    }

    // Uses the parameter's reference of the drift time instead of the actual time since drift time is reset
    // (Done so that boosting can't be done after the speed is out of the threshold)
    private void DriftBoost(float time)
    {
        // Don't boost if the time isn't longer than drift time threshold
        if (time < driftTimeThreshold || currentSpeed < driftSpeedThreshold) return;

        // If the drift time is longer than the minimum drift time + .3 seconds, use the more powerful boost.
        float boostPower = time > driftTimeThreshold * 1.5f ? bigDriftBoost : smallDriftBoost;

        // If both back wheels are grounded, boost.
        if (wheelColliders[2].isGrounded && wheelColliders[3].isGrounded)
        {
            carBody.AddForceAtPosition(transform.forward * boostPower, boostPoint.position, ForceMode.Acceleration);
            audioManager.PlayDriftBoost();

            StartCoroutine(playVFX());
        }

        // Turns on and off the vfx for the boosting
        IEnumerator playVFX()
        {
            if (boostVFX.Length > 0)
            {
                foreach (VisualEffect fx in boostVFX) fx.Play();

                yield return new WaitForSeconds(.7f);

                foreach (VisualEffect fx in boostVFX) fx.Stop();
            }
        }
    }

    private void Steering()
    {
        float input = inputManager.SteerAction.ReadValue<float>();

        float steerRotation = input * steeringCurve.Evaluate(currentSpeed);

        // Steering controls are reversed when affected by oil
        wheelColliders[0].steerAngle = slipInOil ? -steerRotation : steerRotation;
        wheelColliders[1].steerAngle = slipInOil ? -steerRotation : steerRotation;
    }

    private void Brake()
    {
        float input = inputManager.BrakeAction.ReadValue<float>();
        isBraking = input > 0;

        // Front Wheels
        // Stop braking if ABS is active.
        wheelColliders[0].GetGroundHit(out WheelHit hitLeft);
        wheelColliders[1].GetGroundHit(out WheelHit hitRight);
        wheelColliders[0].brakeTorque = SkiddingSidewards(hitLeft) ? input * moveProperties.BrakingPower * .1f : input * moveProperties.BrakingPower * .8f;
        wheelColliders[1].brakeTorque = SkiddingSidewards(hitRight) ? input * moveProperties.BrakingPower * .1f : input * moveProperties.BrakingPower * .8f;

        // Brake slightly in the back wheels
        //wheelColliders[2].brakeTorque = input * moveProperties.BrakingPower * .2f;
        //wheelColliders[3].brakeTorque = input * moveProperties.BrakingPower * .2f;

        // If the direction is less than 0.... The vehicle is moving backwards
        float direction = Vector3.Dot(carBody.velocity, transform.forward);

        // This version requires using a a speed limter when reversing.
        if (direction < 0 && !isAccelerating && isBraking) Reverse(input);
    }

    private bool SkiddingSidewards(WheelHit newHit)
    {
        return Mathf.Abs(newHit.sidewaysSlip * 100) > skidThreshold;
    }

    private void Reverse(float input)
    {
        if (isAccelerating) return;
        // Stop reversing once the reversing top speed has been reached (Reversing Top Speed = a percentage of the top speed)
        if (currentSpeed > moveProperties.TopSpeed / 3)
        {
            wheelColliders[2].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;
            input = 0;
        }

        // So that burnouts can be done
        if (input < 1) return;

        wheelColliders[2].motorTorque = input * -acceleration / 5;
        wheelColliders[3].motorTorque = input * -acceleration / 5;

        // Stop braking
        foreach (WheelCollider wheel in wheelColliders) wheel.brakeTorque = 0;
    }

    private void Burnout()
    {
        if (isBraking && isAccelerating && currentSpeed < 3)
        {
            wheelColliders[2].motorTorque = acceleration * 2;
            wheelColliders[3].motorTorque = acceleration * 2;

            wheelColliders[0].forwardFriction = moveProperties.SetFWDHandling(driftCurveFWD);
            wheelColliders[1].forwardFriction = moveProperties.SetFWDHandling(driftCurveFWD);

            wheelColliders[2].sidewaysFriction = moveProperties.SetSideHandling(driftCurveSide);
            wheelColliders[3].sidewaysFriction = moveProperties.SetSideHandling(driftCurveSide);
        }

        // Make spinning easier
        else if (inputManager.DriftAction.ReadValue<float>() < 1)
        {
            wheelColliders[0].forwardFriction = moveProperties.SetFWDHandling(defaultCurveFWD);
            wheelColliders[1].forwardFriction = moveProperties.SetFWDHandling(defaultCurveFWD);

            wheelColliders[2].sidewaysFriction = moveProperties.SetSideHandling(defaultCurveSide);
            wheelColliders[3].sidewaysFriction = moveProperties.SetSideHandling(defaultCurveSide);
        }
    }

    private void RotateWheels()
    {
        if (hovering) return;

        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            Quaternion rot;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out rot);

            // While there's a valid mesh
            if (wheelMeshes[i])
            {
                wheelMeshes[i].transform.rotation = rot;
                wheelMeshes[i].transform.position = pos;
            }
        }
    }

    private void GroundFriction()
    {
        if (slipInOil)
        {
            OilSlip();
            return;
        }

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.GetGroundHit(out WheelHit ground);

            // Will use the default ground friction if there isn't a physical material
            if (ground.collider) groundFriction = ground.collider.material;
            else groundFriction = defaultGroundFriction;

            terrainCurveFWD = new()
            {
                asymptoteSlip = groundFriction.dynamicFriction * moveProperties.FWD_MaxSlip,
                extremumSlip = groundFriction.staticFriction * moveProperties.FWD_MinSlip,
                extremumValue = moveProperties.FWD_InitiationForce,
                asymptoteValue = moveProperties.FWD_MinForce,
                stiffness = moveProperties.ForwardGrip
            };

            terrainCurveSide = new()
            {
                asymptoteSlip = groundFriction.dynamicFriction * moveProperties.Side_MaxSlip,
                extremumSlip = groundFriction.staticFriction * moveProperties.Side_MinSlip,
                extremumValue = moveProperties.Side_InitiationForce,
                asymptoteValue = moveProperties.Side_MinForce,
                stiffness = moveProperties.SidewardGrip
            };

            wheel.forwardFriction = moveProperties.SetFWDHandling(terrainCurveFWD);
            wheel.sidewaysFriction = moveProperties.SetSideHandling(terrainCurveSide);
        }

    }

    private void OilSlip()
    {
        terrainCurveFWD = new()
        {
            asymptoteSlip = moveProperties.FWD_MaxSlip * 2,
            extremumSlip = moveProperties.FWD_MinSlip,
            extremumValue = moveProperties.FWD_InitiationForce,
            asymptoteValue = moveProperties.FWD_MinForce * .5f,
            stiffness = moveProperties.ForwardGrip * 2f
        };

        terrainCurveSide = new()
        {
            asymptoteSlip = moveProperties.Side_MaxSlip * 2,
            extremumSlip = moveProperties.Side_MinSlip,
            extremumValue = moveProperties.Side_InitiationForce,
            asymptoteValue = moveProperties.Side_MinForce * .5f,
            stiffness = moveProperties.SidewardGrip * .1f
        };

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.forwardFriction = moveProperties.SetFWDHandling(terrainCurveFWD);
            wheel.sidewaysFriction = moveProperties.SetSideHandling(terrainCurveSide);
        }

    }

    private void Gravity()
    {
        carBody.AddForce(Physics.gravity * moveProperties.GravityMultiplier);
    }

    private void FlipTimer()
    {
        float avgRPM = (wheelColliders[0].rpm + wheelColliders[1].rpm) / 2;
        // If the wheels are spinning slow enough or if the car isn't moving (PLAYERS CAN STOP THE FRONT WHEELS FASTER BY BRAKING AND ACCELERATING SIMULTANEOUSLY).
        if (!hovering)
        {
            canFlip = !IsGrounded() && ((avgRPM < flipRpmThreshold && carBody.velocity.magnitude < 2) || currentSpeed < flipSpeedThreshold);
        }

        else canFlip = !hoverScript.ValidPropulsion && carBody.velocity.magnitude < 2.5f;

        // Reset the flip attempts whenever the car is grounded
        if (IsGrounded() || (hovering && hoverScript.ValidPropulsion)) flipAttempts = 1;

        // Reset and return if the conditions aren't met...
        if (!canFlip)
        {
            flipTimer = 0;
            return;
        }

        flipTimer += Time.deltaTime;

        if (flipTimer > flipDelay)
        {
            FlipCar();
            flipTimer = 0;
            canFlip = false;
        }
    }

    private void FlipCar()
    {
        // After each failed flip, the flip gets stronger to ensure the player flips back over eventually
        flipAttempts += .5f;
        flipAttempts = Mathf.Clamp(flipAttempts, 1, 3.5f);

        bool left = transform.localRotation.z < 0;

        carBody.maxAngularVelocity = canFlip ? 1000 : moveProperties.MaxTurnSpeed;

        carBody.velocity += Vector3.up * 5;
        carBody.AddRelativeTorque((left ? -flipForce : flipForce) * carBody.mass * flipAttempts * transform.forward);
    }

    public void ResetInputs()
    {
        inputManager.ResetContoller();
    }

    public Vector3 GetVelocity()
    {
        return carBody.velocity;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    public float GetRawSpeed()
    {
        return Vector3.Dot(carBody.velocity, transform.forward);
    }


    public MovementProperties GetMoveStats()
    {
        return moveProperties;
    }

    public bool IsGrounded()
    {
        // The default value is true.
        bool grounded = true;

        // If any of the wheels aren't on the floor.. grounded is false.
        foreach (WheelCollider wheel in wheelColliders) if (!wheel.isGrounded) grounded = false;

        return grounded;
    }


    //////////////////////////// ABILITIES ////////////////////////////  

    public void SetHovering(bool toSet)
    {
        hovering = toSet;
        audioManager.EditVolume("engine", hovering ? .75f : 1);
    }

    public void ReconfigureCar(float dragMultiplier, float engineMultiplier, float gripMultiplier)
    {
        carBody.drag = moveProperties.Drag * dragMultiplier;

        // Increase the engine power.
        acceleration = dragMultiplier != 1 ? moveProperties.EnginePower * engineMultiplier : moveProperties.EnginePower;

        // If the multiplier isn't 1, the drag would be decreased to make the car move faster (Give the car significantly less air resistance)
        topSpeed = dragMultiplier != 1 ? 99999999999 : moveProperties.TopSpeed;

        WheelFrictionCurve sideFriction;
        WheelFrictionCurve fwdFriction;

        WheelFrictionCurve highSide = new()
        {
            asymptoteSlip = moveProperties.Side_MaxSlip,
            extremumSlip = moveProperties.Side_MinSlip,
            extremumValue = moveProperties.Side_InitiationForce,
            asymptoteValue = moveProperties.Side_MinForce,
            stiffness = moveProperties.SidewardGrip * gripMultiplier
        };

        WheelFrictionCurve highFwd = new()
        {
            asymptoteSlip = moveProperties.FWD_MaxSlip,
            extremumSlip = moveProperties.FWD_MinSlip,
            extremumValue = moveProperties.FWD_InitiationForce,
            asymptoteValue = moveProperties.FWD_MinForce,
            stiffness = moveProperties.ForwardGrip * gripMultiplier
        };

        foreach (WheelCollider wheel in wheelColliders)
        {
            sideFriction = dragMultiplier != 1 ? moveProperties.SetFWDHandling(highSide) :
            moveProperties.SetFWDHandling(defaultCurveFWD);

            fwdFriction = dragMultiplier != 1 ? moveProperties.SetFWDHandling(highFwd) :
            moveProperties.SetFWDHandling(defaultCurveFWD);
        }
    }

    public void SpeedBoost()
    {
        carBody.velocity += 40 * transform.forward;
        boostManager.RefillBoost();
        audioManager.PlaySpeedPack();
    }

    public void InOil(bool inMud)
    {
        if (inMud)
        {
            if (spinoutPoint && !slipInOil)
            {
                // Random int 4-8
                int rndForce = Random.Range(4, 9);
                carBody.AddForceAtPosition(carBody.mass * rndForce * transform.right, spinoutPoint.position, ForceMode.Impulse);
                carBody.AddRelativeTorque(carBody.mass * oilSpinForce * transform.up);
            }
            slipInOil = true;
            oilTimerOn = false;
            oilTimer = 0;
            return;
        }

        // If the player isn't in the mud anymore... turn on the timer but keep the effect.
        oilTimerOn = true;
    }

    private void InOilTime()
    {
        if (!oilTimerOn) return;

        oilTimer += Time.deltaTime;
        if (oilTimer > inOilDuration)
        {
            oilTimer = 0;

            slipInOil = false;
        }
    }

    public bool GetInOil()
    {
        return slipInOil;
    }


    //////////////////////////// AUDIO ////////////////////////////  

    private void ConfigureVFX()
    {
        if (boostVFX.Length > 0) foreach (VisualEffect fx in boostVFX) fx.Stop();
        if (driftBoostVFX.Length > 0) foreach (VisualEffect fx in driftBoostVFX) fx.Stop();
    }

    //////////////////////////// AUDIO ////////////////////////////  


    private void CreateSFX()
    {
        audioManager.CreateAudioSource("Engine");
        audioManager.EditClip("engine", audioManager.GetSfxContainer().EngineSFX[engineSFX]);

        audioManager.CreateAudioSource("ignition");

        audioManager.CreateAudioSource("boost");
        audioManager.CreateAudioSource("driftBoost");
        audioManager.CreateAudioSource("driftingBoost");

        audioManager.CreateAudioSource("speedPack");
        audioManager.EditPitch("speedPack", 1.9f);

        // Since the placeholder impact sfx is the same as the gunshot one...
        audioManager.CreateAudioSource("impact");
        audioManager.EditPitch("impact", .5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude < 7500 || !audioManager) return;
        audioManager.EditVolume("impact", collision.impulse.magnitude * .0005f);
        audioManager.PlayImpact();
    }


    //For Slow Down
    public void StartSlowDown(float dragMultiplier, float duration)
    {
        StartCoroutine(SlowDown(dragMultiplier, duration));
    }

    private IEnumerator SlowDown(float dragMultiplier, float duration)
    {
        carBody.drag *= dragMultiplier;

        yield return new WaitForSeconds(duration);

        carBody.drag = moveProperties.Drag;
    }

    //////////////////// //////// DEBUGGING ////////////////////////////  


    public float GetMass()
    {
        if (!carBody) return 0;

        return carBody.mass;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public float[] GetFrictions()
    {
        float[] frictions = { moveProperties.ForwardGrip, moveProperties.SidewardGrip };
        return frictions;
    }

    public float[] GetDriftFrictions()
    {
        float[] frictions = { moveProperties.ForwardDriftMultiplier, moveProperties.SidewardDriftMultiplier };
        return frictions;
    }

    public float[] GetCurrentFrictions()
    {
        float[] frictions = { moveProperties.FWD_HandlingCurve.stiffness, moveProperties.Side_HandlingCurve.stiffness };
        return frictions;
    }

    public BoostManager GetBoostManager()
    {
        return boostManager;
    }
}

