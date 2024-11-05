/**************************************************************************************************************
* Hoverer
* The script used to that allows the hover to work. Uses pythagoras theorem to calculate the correct lengths of the hover rays so that they're all even. 
*
* Created by Dean Atkinson-Walker 2023
*
* Change log:
*       Armin - 09/08/23 - Added multiplayer support
*       Armin - 13/08/23 - Removed friendly fire
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class Hoverer : NetworkBehaviour
{
    [SerializeField, Tooltip("Takes all of the hover points that are present...")]
    private Transform[] hoverPoints;

    [Header("Configuration")]
    [SerializeField] private GameObject[] wheelMeshes;
    [SerializeField] private WheelCollider[] wheelColliders;

    [Space]
    [SerializeField, Tooltip("The hover center of mass for the car...")] private Transform hoverCenter;
    [SerializeField, Tooltip("The normal center of mass for the car...")] private Transform normalCenter;
    [SerializeField] private Transform boostPoint;

    private InputManager inputManager;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25;
    [SerializeField] private float turnPower = 100000;
    [SerializeField, Tooltip("A multiplier that affects how much force should be applied to the floor to make the car hover.")] private float propulsionPow = 1.5f;
    private float hypDistance;

    private Rigidbody carBody;
    private bool active;
    private MovementController moveController;
    private MovementProperties moveStats;

    [SerializeField, Tooltip("The distance from the ground the car shold be whilst hovering.")]
    private float minFloorDistance = 5;

    private AudioManager audioManager;

    [SerializeField] private VisualEffect[] smokeVFX;

    // Returns whether or not the hover raycasts are hitting anything
    public bool ValidPropulsion { get; private set; }
    
    private NetworkVariable<bool> _showSmokeVFXNetworkVariable = new NetworkVariable<bool>(default, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private ShowActiveAbility abilityIcon;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        audioManager = GetComponent<AudioManager>();

        audioManager.CreateAudioSource("hover");
        audioManager.EditVolume("hover", .5f);

        moveController = GetComponent<MovementController>();
        moveStats = moveController.GetMoveStats();

        abilityIcon = GetComponentInChildren<ShowActiveAbility>();

        foreach (VisualEffect fx in smokeVFX) fx.Stop();

        // Using pythagoras to find the distance between the hover point position and the floor (all the rays that are at an angle are 45 degrees - which makes the right angle).
        hypDistance = Mathf.Sqrt((minFloorDistance * minFloorDistance) + (minFloorDistance * minFloorDistance));
    }

    private void OnEnable()
    {
        _showSmokeVFXNetworkVariable.OnValueChanged += _ToggleSmokeVFX;
    }

    private void OnDisable()
    {
        _showSmokeVFXNetworkVariable.OnValueChanged -= _ToggleSmokeVFX;
    }

    private void FixedUpdate()
    {
        if (!active) return;

        // The Hover function needs to be in FixedUpdate otherwise its less stable.
        Hover();
        AerialMovement();
    }

    private void Hover()
    {
        foreach (Transform pos in hoverPoints)
        {
            Vector3 propulsionDirection = (carBody.mass * -Physics.gravity.magnitude * pos.transform.forward) / hoverPoints.Length;
            float distance = pos.localEulerAngles.x < 90 ? hypDistance : minFloorDistance;

            if (Physics.Raycast(pos.position, pos.transform.forward, out RaycastHit floor, distance))
            {
                carBody.AddForceAtPosition((propulsionDirection * Mathf.Pow(distance - floor.distance, propulsionPow) / distance) / hoverPoints.Length, pos.position);
                ValidPropulsion = true;
            }

            else ValidPropulsion = false;
        }
    }

    private void AerialMovement()
    {
        // To prevent acceleration while at top speed (making the top speed while hovering a bit higher since AddForce() is more responsive than the wheel colliders)...
        bool topSpeed = moveController.GetSpeed() > moveStats.TopSpeed * 1.2f;

        float forwardInput = inputManager.AccelerateAction.ReadValue<float>();
        if (forwardInput > 0 && !topSpeed) carBody.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);

        // The horizontal movement is performed in the action controller...
    }

    private void OnMovementAbility()
    {
        active = !active;
        abilityIcon.ToggleIcon(active);

        if (active) audioManager.PlayHover();
        else audioManager.StopHover();

        _showSmokeVFXNetworkVariable.Value = active;

        carBody.centerOfMass = active ? hoverCenter.localPosition : normalCenter.localPosition;
        boostPoint.localPosition = new(boostPoint.localPosition.x, active ? hoverCenter.localPosition.y : normalCenter.localPosition.y, boostPoint.localPosition.z);

        // When wheel colliders are reactivated the car loses all of its velocity....
        Vector3 currentVelocity = carBody.velocity;
        Vector3 currentTorque = carBody.angularVelocity;

        if (active)
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                wheel.enabled = false;

                // Ensure that the collider has no rotation
                wheel.transform.localRotation = new();
            }
            foreach (GameObject wheel in wheelMeshes)
            {
                bool flip = wheel.GetComponentInParent<WheelCollider>().transform.localPosition.x < 0;
                wheel.transform.localEulerAngles = new(0, 0, flip ? -90 : 90);
            }
        }

        else foreach (WheelCollider wheel in wheelColliders) wheel.enabled = true;


        GetComponent<MovementController>().SetHovering(active);

        // Giving the car back its original velocity.
        carBody.velocity = currentVelocity;
        carBody.angularVelocity = currentTorque;
    }

    private void _ToggleSmokeVFX(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            foreach (VisualEffect fx in smokeVFX) fx.Play();
            return;
        }
        foreach (VisualEffect fx in smokeVFX) fx.Stop();
    }

    public bool GetActive()
    {
        return active;
    }
}
