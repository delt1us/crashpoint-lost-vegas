/**************************************************************************************************************
* Wheelie
* One of the movement abilities for the Hippy van. It moves its center of mass backwards and increases engine power so that the van does a wheelie
*
* Created by Dean Atkinson-Walker 2023
*
* (UNUSED)
*
***************************************************************************************************************/

using UnityEngine;

public class Wheelie : MonoBehaviour
{
    private bool active;

    [SerializeField, Tooltip("In seconds")] private float maxTime= 12;
    private float currentTime = 12;

    [Header("Movement")]
    [SerializeField, Range(1.25f, 10)] private float engineMultiplier = 5;
    [SerializeField] private float dragMultiplier = .8f;

    [Header("Center of Mass")]
    [SerializeField] private Transform centerMass;
    [SerializeField, Range(0, .1f), Tooltip("The Amount that the centre of mass should go backwards.")] 
    private float centerMassBack = .001f;

    private Vector3 centerMassOffset;

    private bool wheelie;

    // Boost Regen
    private bool timerOn;
    private float regenTimer;
    [SerializeField] private float regenCooldown = 4.5f;

    [SerializeField] private float regenRate = 1;
    private bool canRegen;

    private void Start()
    {
        currentTime = maxTime;
        centerMassOffset = new(centerMass.localPosition.x, centerMass.localPosition.y, centerMass.localPosition.z - centerMassBack);
    }


    public void Update()
    {
        ActivationTimer();
        Regen();
        Cooldown();
    }

    private void OnMovementAbility()
    {
        if (currentTime == 0) return;

        active = !active;
        wheelie = currentTime > 0 && active;
        centerMass.localPosition = active? centerMassOffset : centerMass.localPosition;

        if (!active) Deactivate();

        // Turn off the regen timer...
        else
        {
            timerOn = false;
            regenTimer = 0;
        }

        centerMass.localPosition += centerMassOffset;
    }

    private void ActivationTimer()
    {
        // When there's no more boost...
        if (currentTime < 0) Deactivate();

        // Only whilst boosting
        if (!wheelie) return;
        canRegen = false;
        currentTime -= Time.deltaTime;
    }

    private void Deactivate()
    {
        //moveController.ReconfigureCar(1, 1);
        active = false;
        wheelie = false;

        if (currentTime < 0) currentTime = 0;
        timerOn = true;
    }

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
        currentTime += Time.deltaTime * regenRate;

        if (currentTime > maxTime)
        {
            currentTime = maxTime;
            canRegen = false;
        }
    }


    ///////////////////////////////// DEBUGGING ///////////////////////////////// 


    public float GetCurrentTime()
    {
        return currentTime;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public Vector3 GetCenterMass()
    {
        return centerMass.localPosition;
    }

    public float GetRegenRate()
    {
        return regenRate;
    }

    public float GetTimer()
    {
        return regenTimer;
    }

    public bool GetActive()
    {
        return wheelie;
    }

}
