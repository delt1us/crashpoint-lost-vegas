/**************************************************************************************************************
* Universal Cooldown
* Used in the HUD to show how long is left in the ability's cooldown
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class UniversalCooldown : MonoBehaviour
{
    private Dasher dasher;
    private QuickTurn quickTurn;
    private SpeedDelivery delivery;
    private Speeder speeder;
    private GrappleGround grapple;


    private float coolDownDuration = 1;

    private Slider cooldownSlider;

    private void Start()
    {
        cooldownSlider = GetComponentInChildren<Slider>();

        float cooldown = 0;

        // Get all of the possible movement ability scripts that have cooldowns.
        if (GetComponentInParent<Dasher>())
        {
            dasher = GetComponentInParent<Dasher>();
            cooldown = dasher.GetChargeDelay();
        }
        else if (GetComponentInParent<QuickTurn>())
        {
            quickTurn = GetComponentInParent<QuickTurn>();
            cooldown = quickTurn.GetChargeDelay();
        }
        else if (GetComponentInParent<SpeedDelivery>())
        {
            delivery = GetComponentInParent<SpeedDelivery>();
            cooldown = delivery.GetCooldown();
        }
        else if (GetComponentInParent<Speeder>())
        {
            speeder = GetComponentInParent<Speeder>();
            cooldown = speeder.GetCooldown();
        }
        else if(GetComponentInParent<GrappleGround>()) grapple = GetComponentInParent<GrappleGround>();

        // Also get the ability's cooldown
        coolDownDuration = cooldown;
    }

    // Update is called once per frame
    private void Update()
    {
        float currentCooldownTime = 0;

        // Since the grapple's cooldown changes depending on whether the grapple was canceled or not...
        if (grapple)
        {
            // Need to get the the cooldown aswell
            coolDownDuration = grapple.GetCooldown();
            currentCooldownTime = grapple.GetCooldownTimer();
        }

        // Get all of the current times in the cooldown
        else if (dasher) currentCooldownTime = dasher.GetChargeTimer();
        else if(quickTurn) currentCooldownTime = quickTurn.GetChargeTime();
        else if(delivery) currentCooldownTime = delivery.GetCurrentThrowTimer();
        else if(speeder) currentCooldownTime = speeder.GetTimer();
        
        cooldownSlider.value = currentCooldownTime / coolDownDuration;
    }
}
