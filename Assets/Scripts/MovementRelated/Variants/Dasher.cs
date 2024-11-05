/**************************************************************************************************************
* Dasher
* Used in the Racer car. It allows the player to perform dashes and manages its availability.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Dasher : MonoBehaviour
{
    private Rigidbody carBody;
    private Vector3 direction;

    private InputManager inputManager;
    private InputAction steerAction;
    private InputAction accelerateAction;

    private byte by_maxCharges = 2;
    private byte by_charges;
    [Header("Dash Regen")]
    [SerializeField, Min(.1f)] private float chargeRate = 1;
    [SerializeField, Tooltip("The time it takes to regenerate one boost dash charge")] private float chargeDelay = 10;
    private float chargeTimer;

    [SerializeField] private float dashForce = 35;

    [Header("Visual Effects")]
    [SerializeField] private VisualEffect[] dashVFX;
    private float timeOfDash;
    private const float vfxDuration = 3;

    private AudioManager audioManager;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        audioManager = GetComponent<AudioManager>();
        audioManager.CreateAudioSource("dash");
        audioManager.EditPitch("dash", 1.8f);

        steerAction = inputManager.SteerAction;
        accelerateAction = inputManager.AccelerateAction;

        by_charges = by_maxCharges;
    }

    private void Update()
    {
        ChargeRegen();
    }

    private void CalculateDiretion()
    {
        float forwardInput = accelerateAction.ReadValue<float>();
        float rawSideInput = steerAction.ReadValue<float>();

        // Gets the normalised value
        float sideInput = rawSideInput > 0? 1: -1;

        // Prioritse the sideways directions
        direction = Mathf.Abs(rawSideInput) > .25f ? transform.right * sideInput : transform.forward * forwardInput;

        // Default directional dash is forward
        if (direction.magnitude < .5f) direction = transform.forward;
    }

    private void OnMovementAbility()
    {
        if(by_charges < 1) return;

        timeOfDash = chargeTimer;
        foreach(VisualEffect fx in dashVFX) fx.Play();

        audioManager.PlayDash();
        CalculateDiretion();
        by_charges--;
        carBody.velocity += dashForce * direction;
    }
    
    private void ChargeRegen()
    {
        // Since this vfx loops... turn it off when it's been active for the set amount of time
        if(timeOfDash > timeOfDash + vfxDuration) foreach (VisualEffect fx in dashVFX) fx.Stop();

        if (by_charges == by_maxCharges) return;

        if (chargeTimer > chargeDelay)
        {
            by_charges++;
            chargeTimer = 0;
        }

        chargeTimer += Time.deltaTime * chargeRate;
    }




    ////////////////////////////////// Debugging //////////////////////////////////
    
    public byte GetChargeTotal()
    {
        return by_charges;
    }

    public float GetChargeTimer()
    {
        if (by_charges > 0) return 0;
        return chargeTimer;
    }

    public float GetChargeDelay()
    {
        return chargeDelay;
    }
}
