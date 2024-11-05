/**************************************************************************************************************
* HealthStationPhysic
* 
* Effect For health station
*
* Created by Envy Cham 2023
* 
* Change Log:
* 8/1 Heal amount 500 --> 600
*
*            
***************************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStationPhysic : MonoBehaviour
{
    [SerializeField] public int health = 600;

    HealthManager healthManager;


    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<HealthManager>())
        {
           
            
            healthManager = other.gameObject.GetComponentInParent<HealthManager>();

            healthManager.GainHealth(health);
        }


    }
}
