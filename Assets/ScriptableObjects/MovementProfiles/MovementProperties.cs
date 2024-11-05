/**************************************************************************************************************
* Movement Properties
* A scriptable object that stores the properties of each car. They're used to create movement profiles for the cars. 
* Done by giving parameters for the engine power, wheel friction curves and braking, boosting, air control speeds... etc.
*
* Used in every playable vehicle in the game.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementProp", menuName = "Scriptable Objects/Movement")]
public class MovementProperties : ScriptableObject
{
    public string CarName;
    [Tooltip("Measured in Newton-meters (Nm)")] 
    public float EnginePower;
    [Tooltip("Measured in Newton-meters (Nm)")] 
    public float BrakingPower;

    [Tooltip("(In MPH)\nUse the debugger to find the car's Real top speed (Car stops accelerating after this speed)...")]
    public float TopSpeed;



    [Header("Boosting")]
    [Range(20, 200)] public int BoostCapacity;

    [Tooltip("The amount of boost regained after a 1/10 of a second.")] 
    public float BoostRegenRate;

    public float BoostPower = 100;

    [Tooltip("In seconds")] 
    public float RegenDelay = 4;



    [Header("Physics")]
    public float GravityMultiplier = 1;

    [Tooltip("Drastically affects acceleration and top speed...")] 
    public float Drag = .2f;

    [Tooltip("a.k.a Angular Drag")]
    public float TurnResistance = .05f;




    [Header("Air Control")]
    [Range(1000, 10000), Tooltip("The speed that the car rotates in the forward/backwards direction.")] 
    public int PitchSpeed;

    [Range(1000, 10000), Tooltip("The speed that the car rotates in the sidewards direction.")]
    public int YawSpeed;

    [Tooltip("The maximum amount of angular velocity the vehicle is allowed to have (Max spin velocity).")] 
    public float MaxTurnSpeed;


    [Header("Drifting")]
    [Range(.001f, 100), Tooltip("Higher number = more slipping")]
    public float ForwardDriftMultiplier = 10;

    [Range(.001f, 100), Tooltip("Higher number = more slipping")]
    public float SidewardDriftMultiplier = 3.6f;

    [Min(.0001f), Tooltip("Higher number = more powerful brake")]
    public float DriftBrakeMultiplier = .1f;




    [Header("Drift Boosting")]
    [Range(1000, 2900), Tooltip("The force of the drift boost after a short drift.")]
    public int SmallDriftBoost = 1800;

    [Range(1100, 4000), Tooltip("The force of the drift boost after a long drift.")]
    public int BigDriftBoost = 3150;

    [SerializeField, Tooltip("The time it takes (in seconds) for the small boost to activate.")]
    public float DriftTimeThreshold = .8f;

    [Tooltip("How fast the player has to be going in order to add to the drift-time count.")]
    public float DriftSpeedThreshold = 80;




    [Header("Basic Handling")]
    [Range(.00001f, 5), Tooltip("For better handling, forward handling factor should be LOWER than sidewards.")]
    public float ForwardGrip = 1;

    [Range(.00001f, 5), Tooltip("For better handling, sideward handling factor should be HIGHER than forwards.")]
    public float SidewardGrip= 1;

    // Slip VS the required force to start a slipping motion
    // Asymptote force = The minimum amount of force for slipping occur 
    // Extremum force = The amount of force required to start slipping

    // Extremum slip = The amount of slip the wheel feels before the dip in required force to further slip the wheel
    // Asymptote slip = The amount of slip applied with the minimum amount of force required

    // Extremum force should be higher than the Asymptote force
    // Asymptote slip should be higher than the Extremum slip




    /////// Forwards
    // Asymptote slip
    [Header("Forward Friction (Affects acceleration/braking)")]
    [Range(.001f, 10), Tooltip("The amount of slip applied once the force of the curve has peaked." +
        "|| Should be HIGHER than Min Slip\n" +
        "The closer the slip values are to each other, the more stable the handling is...")]
    public float FWD_MaxSlip = 4;


    // Extremum slip
    [Range(.001f, 10), Tooltip("The amount of slip the wheel feels before the dip in required force to further slip the wheel." +
        "|| Should be LOWER than Max Slip\n" +
        "The closer the slip values are to each other, the more stable the handling is...")] 
    public float FWD_MinSlip = .7f;


    // Extremum force
    [Range(.001f, 10), Tooltip("The amount of force required to start slipping. || Should be HIGHER than Min Force\n" +
        "The closer the force values are to each other, the more stable the handling is...")]
    public float FWD_InitiationForce = 6.5f;


    // Asymptote force
    [Range(.001f, 10), Tooltip("The minimum amount of force for slipping to occur. || Should be LOWER than Initiation Force\n" +
        "The closer the force values are to each other, the more stable the handling is...\n" +
        "|| Increasing this value greatly increases acceleration (Keep in mind, the value should be less than Initiation Force).")]
    public float FWD_MinForce = 2;




    /////// Sideways
    // Asymptote slip
    [Header("Sideward Friction (Affects steering/handling)")]
    [Range(.001f, 10), Tooltip("The amount of slip applied once the force of the curve has peaked." +
        "|| Should be HIGHER than Min Slip - " +
        "Responsible for depicting the Maximum amount the car can turn WHILE the car is slipping\n" +
        "The closer the slip values are to each other, the more stable the handling is...")]
    public float Side_MaxSlip = 1;


    // Extremum slip
    [Range(.001f, 10), Tooltip("The amount of slip the wheel feels before the dip in required force to further slip the wheel." +
        "|| Should be LOWER than Min Slip - " +
        "Responsible for depicting the Maximum amount the car is allowed to turn BEFORE it starts slipping.\n" +
        "The closer the slip values are to each other, the more stable the handling is...")]
    public float Side_MinSlip = .1f;


    // Extremum force
    [Range(.001f, 10), Tooltip("The amount of force required to start slipping. || Should be HIGHER than Min Force\n" +
        "The closer the force values are to each other, the more stable the handling is...")]
    public float Side_InitiationForce = 8;

    // Asymptote force
    [Range(.001f, 10), Tooltip("The minimum amount of force for slipping to occur. || Should be LOWER than Initiation Force\n" +
        "The closer the force values are to each other, the more stable the handling is...")]
    public float Side_MinForce = 3;

    public WheelFrictionCurve FWD_HandlingCurve { get; private set; }
    public WheelFrictionCurve Side_HandlingCurve { get; private set; }

    public WheelFrictionCurve SetFWDHandling(WheelFrictionCurve curve)
    {
        FWD_HandlingCurve = curve;
        return FWD_HandlingCurve;
    }
    public WheelFrictionCurve SetSideHandling(WheelFrictionCurve curve)
    {
        Side_HandlingCurve = curve;
        return Side_HandlingCurve;
    }


    private void OnEnable()
    {
        CarName = "MProp_" + name;
    }
}
