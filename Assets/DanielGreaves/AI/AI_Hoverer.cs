using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AI_Hoverer : MonoBehaviour
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

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
        
        audioManager = GetComponent<AudioManager>();

        audioManager.CreateAudioSource("hover");
        audioManager.EditVolume("hover", .5f);

       // moveController = GetComponent<MovementController>();
       // moveStats = moveController.GetMoveStats();

        foreach (VisualEffect fx in smokeVFX) fx.Stop();

        // Using pythagoras to find the distance between the hover point position and the floor (since the angle is 45 degrees, only need one distance (the minFloorDistance))
        hypDistance = Mathf.Sqrt((minFloorDistance * minFloorDistance) + (minFloorDistance * minFloorDistance));
    }


    private void FixedUpdate()
    {
        if (!active) return;

        Hover();
        AerialMovement();

        OnMovementAbility();

       
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

        //float forwardInput = inputManager.AccelerateAction.ReadValue<float>();
       // if (forwardInput > 0 && !topSpeed) carBody.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);

        // The horizontal movement is performed in the action controller...
    }

    private void OnMovementAbility()
    {
        active = !active;

        if (active)
        {
            audioManager.PlayHover();
            foreach (VisualEffect fx in smokeVFX) fx.Play();
        }
        else
        {
            audioManager.StopHover();
            foreach (VisualEffect fx in smokeVFX) fx.Stop();
        }

        carBody.centerOfMass = active ? hoverCenter.localPosition : normalCenter.localPosition;
        boostPoint.localPosition = new(boostPoint.localPosition.x, active ? hoverCenter.localPosition.y : normalCenter.localPosition.y, boostPoint.localPosition.z);

        // When wheel colliders are reactivated the car loses its velocity....
        Vector3 currentVelocity = carBody.velocity;
        Vector3 currentTorque = carBody.angularVelocity;

        if (active)
        {
            foreach (WheelCollider wheel in wheelColliders) wheel.enabled = false;
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
}
