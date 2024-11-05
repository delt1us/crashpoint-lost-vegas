/**************************************************************************************************************
* Input Manager
* Used to store all of the input actions that are holding actions (like shooting and accelerating). Other scripts would use this to get references to the action they need
* to read.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Keybinds keybinds;
    private PlayerInput playerInput;

    // Movement
    public InputAction AimAction { get; private set; }
    public InputAction AimLookAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction AccelerateAction { get; private set; }
    public InputAction BrakeAction { get; private set; }
    public InputAction SteerAction { get; private set; }
    public InputAction AirControlAction { get; private set; }
    public InputAction AirRollAction { get; private set; }
    public InputAction DriftAction { get; private set; }
    public InputAction BoostAction { get; private set; }

    // Action
    public InputAction PrimaryShootAction { get; private set; }

    public bool AimToggle { get; private set; }


    // Start is called before the first frame update
    private void Awake()
    {
        ResetContoller();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool UsingController()
    {
        return playerInput.currentControlScheme == playerInput.defaultControlScheme;
    }

    public void ResetContoller()
    {
        keybinds = new();
        keybinds.Player.Enable();

        playerInput = GetComponent<PlayerInput>();

        AimAction = keybinds.Player.Aim;
        AimLookAction = keybinds.Player.AimLook;
        LookAction = keybinds.Player.FreeLook;

        AccelerateAction = keybinds.Player.Accelerate;
        BrakeAction = keybinds.Player.Brake;
        SteerAction = keybinds.Player.Steering;
        DriftAction = keybinds.Player.HandBrake;
        BoostAction = keybinds.Player.Boost;
        AirControlAction = keybinds.Player.AirControl;
        AirRollAction = keybinds.Player.AirRoll;

        PrimaryShootAction = keybinds.Player.Fire;
    }

    public void SetLookSens(float x)
    {
        LookAction.ApplyBindingOverride(new() { overrideProcessors = $"ScaleVector2(x={x},y={1})" });
    }

    public void SetAimSens(float x, float y)
    {
        AimLookAction.ApplyBindingOverride(new() { overrideProcessors = $"ScaleVector2(x={x},y={y})" });
    }

    public void OnToggleAimToggle()
    {
        AimToggle = !AimToggle;
    }

    ////////  - Press Enter
    private void OnDebugHideMouse()
    {
        if (Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
