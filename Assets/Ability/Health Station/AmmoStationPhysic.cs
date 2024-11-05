/**************************************************************************************************************
* AmmoStationPhysic
* 
* Effect for ammo station 
*
* Created by Envy Cham 2023
* 
* Change Log:
*
*
*            
***************************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoStationPhysic : MonoBehaviour
{
   WeaponController controller;
    [SerializeField] private float ammo = 100;
    

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<WeaponController>())
        {
            controller = other.gameObject.GetComponentInParent<WeaponController>();

            controller.RefillAmmo();
        }


    }
}
