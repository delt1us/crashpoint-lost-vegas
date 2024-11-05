using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillImage;
    private Slider slider;
            

    void Awake()
    {
        slider = GetComponent<Slider>();    
    }

    
    void Update()
    {
        if ((slider.value <= slider.minValue))
        {
            fillImage.enabled = false;
        }
            
        if((slider.value >= slider.minValue && !fillImage.enabled))
        {
            fillImage.enabled = true;
        }

        float fillvalue = playerHealth.currentHealth / playerHealth.maxHealth;
      
        if(fillvalue <= slider.maxValue / 3) 
        {
            fillImage.color = Color.yellow;
        }

        if (fillvalue >= slider.maxValue / 3)
        {
            fillImage.color = Color.red;
        }

        slider.value = fillvalue;
    }
}
