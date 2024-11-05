//*************************************************************************************************************
/*  Oil Leak Cooldown
 *  Used to display how much time is left of the defenec ability cooldowns
 *  
 *  Created by Callum Dourneen 2023
 *  Change log:
 *      Callum - Started it
 *      Dean   - Made the script work with any defence ability by getting the cooldown of the ability and the current time of its cooldown 
 */
//*************************************************************************************************************

using UnityEngine;
using UnityEngine.UI;
// Change script name to DefensiveCoolDown
public class OilLeakCoolDown : MonoBehaviour
{
    private float coolDownDuration = 11;
    private float currentCoolDownTime = 0;

    private ActionController ac;

    [SerializeField] private Slider oilLeakSlider;
    

    private void Start()
    {
        ac = GetComponentInParent<ActionController>();

        coolDownDuration = ac.GetDefenceCooldown();
        oilLeakSlider.gameObject.SetActive(true);

    }
    private void Update()
    {
        UpdateSliderVaule();
    }

    private void UpdateSliderVaule()
    {
        currentCoolDownTime = ac.GetDefenceTimer();

            float progress = currentCoolDownTime / coolDownDuration;
        oilLeakSlider.value = progress;
    }

}
