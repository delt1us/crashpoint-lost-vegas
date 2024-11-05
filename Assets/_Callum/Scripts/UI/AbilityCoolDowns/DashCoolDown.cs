using UnityEngine;
using UnityEngine.UI;
//c
// Summary 
// Attached To GameObjects - [ WeaponsAbilitiesImage ] 
// Purpose -                 [  Provides a CoolDown Effect Using a Slider ] 
// Functions -               [ 1. Gets the Current Charges for the Dasher Ability and Changes Images Based on the Result ]
//                           [ 2. Updates the Slider Depending on the Diffrence Between the timePassed and cooldownDuration Creating the CoolDown Effect ]
// Dependencies -            [ Dasher ]
// Notes - 
public class DashCoolDown : MonoBehaviour
{
    private bool isCoolDown;
    private bool isCoolingDown;
    private float cooldownDuration = 10f;
    private float cooldownStartTime;

    public Dasher dasher;

    public Image Maxdash;
    public Image ability;
    public Image abilityCoolDown;
    public Slider cooldownSlider;
    public Slider zeroCooldownSlider;
    private void Start()
    {
        dasher = GetComponentInParent<Dasher>();
        isCoolDown = false;
        
        if (dasher.GetChargeTotal() == 2)
        {
            Maxdash.gameObject.SetActive(true);
            ability.gameObject.SetActive(false);
        }
        else
        {
            Maxdash.gameObject.SetActive(false);
            ability.gameObject.SetActive(true);
        }
        abilityCoolDown.gameObject.SetActive(false);
        zeroCooldownSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        CoolDown();
       

        if (!isCoolingDown)
        {
            cooldownSlider.value = 1f;
        }
    }
    public void CoolDown()
    {
        float currentCharge = dasher.GetChargeTotal();

        if (currentCharge >= 2)
        {
            isCoolDown = false;
            isCoolingDown = false;
            Maxdash.gameObject.SetActive(true);
            ability.gameObject.SetActive(false);
            abilityCoolDown.gameObject.SetActive(false);
            cooldownSlider.gameObject.SetActive(false);
            abilityCoolDown.gameObject.SetActive(false);
            return;
        }
        else if (currentCharge == 1)
        {
            isCoolDown = true;
            
            if (!isCoolingDown)
            {
                cooldownStartTime = Time.time;
                isCoolingDown = true;
                Maxdash.gameObject.SetActive(false);
                ability.gameObject.SetActive(true);
                abilityCoolDown.gameObject.SetActive(false);
                cooldownSlider.gameObject.SetActive(true);
                abilityCoolDown.gameObject.SetActive(false);
            }
            Debug.Log(currentCharge);
            
            float timePassed = Time.time - cooldownStartTime;
            float cooldownProgress = Mathf.Clamp01(timePassed / cooldownDuration);

            
                cooldownSlider.value = 1f - cooldownProgress;
            

            if (currentCharge == 0 || cooldownProgress >= 1f)
            {
                ability.gameObject.SetActive(false);
                abilityCoolDown.gameObject.SetActive(true);
                cooldownSlider.gameObject.SetActive(true);
                abilityCoolDown.gameObject.SetActive(false);
            }
            else
            {
                ability.gameObject.SetActive(true);
                abilityCoolDown.gameObject.SetActive(false);
                cooldownSlider.gameObject.SetActive(true);
                abilityCoolDown.gameObject.SetActive(false);
            }
        } 
        else
        {
            isCoolingDown = false;
            isCoolDown = true;
            ability.gameObject.SetActive(false);
            abilityCoolDown.gameObject.SetActive(true);
            Maxdash.gameObject.SetActive(false);
            cooldownSlider.value = 0f;
            cooldownSlider.gameObject.SetActive(false);
            zeroCooldownSlider.gameObject.SetActive(true);

            float timePassed = Time.time - cooldownStartTime;
            float cooldownProgress = Mathf.Clamp01(timePassed / cooldownDuration);

            zeroCooldownSlider.value = 1f - cooldownProgress;
        }
        
    }
    
}
