// Class created by Dean and modified by Daniel to work with AI firing weapons

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Secondary_WeaponSys : MonoBehaviour
{
    [SerializeField] public GameObject WeaponRaycast;
    [SerializeField] private GameObject secondaryWeapon;
    [SerializeField] private Transform secondarySpawn1;
    [SerializeField] private Transform secondarySpawn2;
    [SerializeField] private float secondaryReloadTime;
    [SerializeField] private float secondaryMaxAmmo;
    [SerializeField] private float secondaryfireRate;

    [SerializeField] private float secondaryCurAmmo;
    [SerializeField] private float secondaryFireRange;
    private float nextFireTime = 0;
    private float WeaponRaycastRange = 60;

    Rigidbody body;
    [SerializeField] private bool isReloading;
   
    private bool bWeaponRaycast;

    private void Start()
    {
        if (secondaryCurAmmo == -1)

        secondaryCurAmmo = secondaryMaxAmmo;

        body = secondaryWeapon.GetComponent<Rigidbody>();
    }


    // Update function
    private void FixedUpdate()
    {
        // Create raycast to detect other players and then fire on detection
        Vector3 AIDetection = Vector3.forward;
        Ray LineTrace = new Ray(WeaponRaycast.transform.position, WeaponRaycast.transform.TransformDirection(AIDetection * WeaponRaycastRange));

        // Draw raycast line
        Debug.DrawRay(WeaponRaycast.transform.position, WeaponRaycast.transform.TransformDirection(AIDetection * WeaponRaycastRange));

        // Raycast collision
        if (Physics.Raycast(LineTrace, out RaycastHit WallHit, WeaponRaycastRange))
        {
            // If raycast hits a player
            if (WallHit.collider.tag == "Player")
            {
                bWeaponRaycast = true;

                // If raycast is true
                if (bWeaponRaycast == true)
                {
                    shootSecondary();

                   
                }
            }
            // Else if false
            else if (WallHit.collider.tag == null)
            {
                bWeaponRaycast = false;
            }
        }

       
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

        OnSecondaryWeapon();
    }
}
