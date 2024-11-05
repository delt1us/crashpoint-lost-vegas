using System.Timers;
using UnityEngine;

public class BoostManager
{
    private bool canBoost;

    // The amount of boost currently in the player's tank
    private float currentBoost;
    private readonly float boostPower;
    private readonly float boostCapacity;

    // Boost Regen
    private readonly float regenDelay;
    private readonly float regenRate;
    private bool canRegen;
    private Timer regenDelayTimer;

    public BoostManager(float capacity, float rate, float power, float delay)
    {
        boostCapacity = capacity;
        regenRate = rate;
        boostPower = power;
        regenDelay = delay * 1000;

        currentBoost = boostCapacity;
    }

    public void Update()
    {
        Regen();
    }

    public void Boost(Rigidbody car, Transform exhaustTransform, bool grounded)
    {
        canBoost = currentBoost > 0;

        if (canBoost)
        {
            currentBoost -= Time.deltaTime * 10;

            // Dampens the force of the boost depending on whether the car is in the air or not
            car.AddForceAtPosition(car.transform.forward * (grounded? boostPower: boostPower *.25f), exhaustTransform.position, ForceMode.Acceleration);

            // Reset the regen timers every time the player boosts.
            DeactivateRegen();
        }

        // Prevents the boost value from going in the minuses..
        if (currentBoost < 0) currentBoost = 0;
        
        if (currentBoost < boostCapacity) if(!canRegen) SetRegenDelay();
    }

    private void Regen()
    {
        if (!canRegen) return;
        currentBoost += Time.deltaTime * 10 * regenRate;

        if (currentBoost > boostCapacity)
        {
            currentBoost = boostCapacity;

            DeactivateRegen();
        }
    }

    private void SetRegenDelay()
    {
        regenDelayTimer?.Dispose();

        regenDelayTimer = new Timer(regenDelay);
        regenDelayTimer.Start();
        regenDelayTimer.Elapsed += RegenDelay_Elapsed;
    }

    private void RegenDelay_Elapsed(object sender, ElapsedEventArgs e)
    {
        regenDelayTimer?.Stop();
        regenDelayTimer?.Dispose();

        canRegen = true;
    }

    private void DeactivateRegen()
    {
        canRegen = false;
        regenDelayTimer?.Stop();
        regenDelayTimer?.Dispose();
    }

    public void RefillBoost()
    {
        currentBoost = boostCapacity;
        DeactivateRegen();
    }



    //////////////////////// DEBUGGING ////////////////////////

    public float GetCurrentBoost()
    {
        return currentBoost;
    }
    
    //c
    public float GetMaxBoost()
    {
        return boostCapacity;
    }

    public float GetRegenRate()
    {
        return regenRate;
    }

    public float GetRegenDelay()
    {
        return regenDelay;
    }

    public float GetPower()
    {
        return boostPower;
    }
}
