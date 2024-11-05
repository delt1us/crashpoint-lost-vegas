/**************************************************************************************************************
* Speeder
* Used in the Police car. It makes the car go faster by reducing its drag and increasing engine power. This ability is managed here too using timers.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

public class Speeder : MonoBehaviour
{
    private bool active;
    private MovementController moveController;

    [SerializeField] private float boostAmount = 150;
    private float currentBoost = 90;
    [SerializeField, Range(.01f, 1)] private float dragMultiplier = .1f;
    [SerializeField, Range(1, 10)] private float engineMultiplier = 4;
    [SerializeField, Range(1, 10)] private float gripMultiplier = 2.5f;

    private bool boosting;

    // Boost Regen
    private bool timerOn;
    private float regenTimer;
    [SerializeField] private float regenCooldown = 4.5f;

    [SerializeField] private float regenRate = 1;
    private bool canRegen;

    private AudioManager audioManager;

    private void Start()
    {
        moveController = GetComponent<MovementController>();
        audioManager = GetComponent<AudioManager>();
        audioManager.CreateAudioSource("siren");

        currentBoost = boostAmount;
    }


    public void Update()
    {
        ActivationTimer();
        Regen();
        Cooldown();
    }

    private void OnMovementAbility()
    {
        if (currentBoost == 0) return;

        active = !active;
        boosting = currentBoost > 0 && active;

        // If you're able to boost... play the siren... otherwise stop
        if (boosting) audioManager.PlaySiren();
        else audioManager.StopSiren();


        if (!active) Deactivate();

        // Turn off the regen timer...
        else
        {
            timerOn = false;
            regenTimer = 0;

            // Activate the ability.
            moveController.ReconfigureCar(dragMultiplier, engineMultiplier, gripMultiplier);
        }

        // Turn on/off the lights
        GetComponentInChildren<SirenLights>().ToggleActive(active);
    }

    private void ActivationTimer()
    {
        // When there's no more boost...
        if (currentBoost < 0) Deactivate();

        // Only whilst boosting
        if (!boosting) return; 

        // Disable boost regen
        canRegen = false;
        currentBoost -= Time.deltaTime * 10;
    }

    private void Deactivate()
    {
        // Set all the multipliers to 1 to reset the car
        moveController.ReconfigureCar(1, 1, 1);

        active = false;
        boosting = false;
        audioManager.StopSiren();

        // Preventing the boost value from going below 0
        if(currentBoost < 0) currentBoost = 0;

        // Turns on the regen timer (otherwise regen won't happen)
        timerOn = true;

        GetComponentInChildren<SirenLights>().ToggleActive(false);
    }

    // Called to enable boost regen
    private void Cooldown()
    {
        if (!timerOn) return;

        regenTimer += Time.deltaTime;
        if (regenTimer > regenCooldown)
        {
            regenTimer = 0;
            canRegen = true;

            timerOn = false;
        }
    }

    private void Regen()
    {
        if (!canRegen) return;
        currentBoost += Time.deltaTime * 10 * regenRate;

        // Once the boost is full...
        if (currentBoost > boostAmount)
        {
            // Stop.
            currentBoost = boostAmount;
            canRegen = false;
        }
    }

    ///////////////////////////////// DEBUGGING ///////////////////////////////// 


    public float GetCooldown()
    {
        return regenCooldown;
    }

    public float GetCurrentBoost()
    {
        return currentBoost;
    }

    public float GetMaxBoost()
    {
        return boostAmount;
    }

    public float GetRegenRate()
    {
        return regenRate;
    }

    public float GetTimer()
    {
        if (currentBoost > 0) return 0;

        return regenTimer;
    }

    public bool GetActive()
    {
        return boosting;
    }


}
