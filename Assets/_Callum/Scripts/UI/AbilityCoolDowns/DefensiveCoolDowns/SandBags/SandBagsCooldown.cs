using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandBagsCooldown : MonoBehaviour
{
    private float coolDownDuration = 10;
    private float currentCoolDownTime = 0;


    [SerializeField] private Slider sandbagSlider;


    private void Start()
    {
        sandbagSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (currentCoolDownTime > 0)
        {
            currentCoolDownTime -= Time.deltaTime;
            if (currentCoolDownTime <= 0)
            {
                currentCoolDownTime = 0;
                sandbagSlider.gameObject.SetActive(false);

            }
            else
            {
                UpdateSliderVaule();
            }
        }
        Debug.Log(currentCoolDownTime);
    }

    private void UpdateSliderVaule()
    {

        float progress = currentCoolDownTime / coolDownDuration;
        sandbagSlider.value = progress;


    }

    public void CoolDown()
    {

        currentCoolDownTime = coolDownDuration;

        sandbagSlider.gameObject.SetActive(true);
        UpdateSliderVaule();



    }
}
