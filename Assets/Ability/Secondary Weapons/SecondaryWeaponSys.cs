using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryWeaponSys : MonoBehaviour
{
    [SerializeField] private GameObject secondaryWeapon;
    [SerializeField] private Transform secondarySpawn1;
  [SerializeField] private Transform secondarySpawn2;
    [SerializeField] private float secondaryReloadTime;
    [SerializeField] private float secondaryMaxAmmo;
    [SerializeField] private float secondaryfireRate;

    [SerializeField] private float secondaryCurAmmo;
  [SerializeField] private float secondaryFireRange = 250;

  private ActionController controller;
  Rigidbody rb;
    [SerializeField] private bool isReloading;
    private float nextFireTime = 0;

    private void Start()
    {
        if( secondaryCurAmmo == -1)
        
        secondaryCurAmmo = secondaryMaxAmmo;

        rb = secondaryWeapon.GetComponent<Rigidbody>();

        


    }

    private IEnumerator reloadSecondary()
    {
        isReloading = true;
        yield return new WaitForSeconds(secondaryReloadTime);

        secondaryCurAmmo = secondaryMaxAmmo;
        isReloading = false;

    }
    private void shootSecondary()
    {
        Instantiate(secondaryWeapon, secondarySpawn1.position, secondarySpawn1.rotation);
        Instantiate(secondaryWeapon, secondarySpawn2.position, secondarySpawn2.rotation);
        //rb.AddForce(transform.forward * secondaryFireRange,  ForceMode.Impulse);
        secondaryCurAmmo--;
    }

  private void OnSecondaryWeapon()
  {
    nextFireTime = Time.time + 1f / secondaryfireRate;
    shootSecondary();
  }
    private void Update()
    {
    if (!secondaryWeapon) return;

        if (isReloading)
            return;
        
        if (secondaryCurAmmo <= 0)
        {
           
            StartCoroutine(reloadSecondary());
            return;
        
        }

        //if (Input.GetKeyDown(KeyCode.R) && Time.time >= nextFireTime)
       // {
           // nextFireTime = Time.time + 1f / secondaryfireRate;
           // shootSecondary();
       // }

        OnSecondaryWeapon();

    }
}
