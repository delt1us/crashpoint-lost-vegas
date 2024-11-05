using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour
{
    public WeaponController primaryWC;
    public WeaponController secondaryWC;
    public Image ammoCircle;
    public Image speicalAmmoCircle;



   
    private void Update()
    {
        if (primaryWC != null && ammoCircle != null)
        {
            float fillAmmount = primaryWC.GetCurrentMag() / primaryWC.GetMaxAmmo();
            ammoCircle.fillAmount = fillAmmount;
        }

        if (secondaryWC != null && speicalAmmoCircle != null)
        {
            float specialFillAmmount = secondaryWC.GetCurrentMag() / secondaryWC.GetMaxAmmo();
            speicalAmmoCircle.fillAmount = specialFillAmmount;
        }
        
    }
}
