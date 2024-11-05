using Unity.Netcode;
using UnityEngine;

public class PointTurn : NetworkBehaviour
{
    private InputManager inputManager;
    private MovementController moveController;
    private bool active;

    [SerializeField, Tooltip("Use the same exact curve that's on the Movement Controller.\nX-axis = The speed of the car \nY-axis = The degree the front wheels should turn")]
    private AnimationCurve steeringCurve;
    [SerializeField, Tooltip("Insert both back wheels")] private WheelCollider[] backWheels;

    private ShowActiveAbility abilityIcon;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        moveController = GetComponent<MovementController>();

        abilityIcon = GetComponentInChildren<ShowActiveAbility>();
    }

    private void Update()
    {
        if (!active) return;

        float input = inputManager.SteerAction.ReadValue<float>();

        float steerRotation = input * steeringCurve.Evaluate(moveController.GetSpeed());
        backWheels[0].steerAngle = moveController.GetInOil() ? steerRotation : -steerRotation;
        backWheels[1].steerAngle = moveController.GetInOil() ? steerRotation : -steerRotation;
    }

    private void OnMovementAbility()
    {
        active = !active;
        abilityIcon.ToggleIcon(active);

        // Reset the wheels if they're set back to normal
        if (!active) foreach(WheelCollider wheel in  backWheels) wheel.steerAngle = 0;
            
    }

    public bool GetActive()
    {
        return active;
    }
}
