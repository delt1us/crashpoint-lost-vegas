using UnityEngine;
using UnityEngine.UI;
// Change script name to DefensiveCoolDown
public class DefensiveCoolDown : MonoBehaviour
{
    private float coolDownDuration;
    private float currentCoolDownTime;

    private float sandbagsCooldown = 15f;
    private float oilSpillCooldown = 12f;

    [SerializeField] private Slider defensiveSlider;
 


    private void Start()
    {
        
        defensiveSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (currentCoolDownTime > 0)
        {
            currentCoolDownTime -= Time.deltaTime;
            if (currentCoolDownTime <= 0)
            {
                currentCoolDownTime = 0;
                defensiveSlider.gameObject.SetActive(false);

            }
            else
            {
                UpdateSliderVaule();
            }
        }
    }

    private void UpdateSliderVaule()
    {

        float progress = currentCoolDownTime / coolDownDuration;
        defensiveSlider.value = progress;


    }
    public void StartCoolDown(string abilityName)
    {
        currentCoolDownTime = GetCurrentCooldownDuration(abilityName);
        defensiveSlider.gameObject.SetActive(true);
        UpdateSliderVaule();
    }


    private float GetCurrentCooldownDuration(string abilityName)
    {
        if (abilityName == "OilSpill")
        {
            return oilSpillCooldown;
        }
        else if (abilityName == "Sandbags")
        {
            return sandbagsCooldown;
        }
        else
        {
            return 0f;
        }

       
    }

}

