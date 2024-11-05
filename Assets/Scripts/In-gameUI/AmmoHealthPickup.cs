/**************************************************************************************************************
* Ammo/Health Pickup
* Used in the health and ammo pickups to make them standout and more visible to the player by making the it spin and bob.
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class AmmoHealthPickup : NetworkBehaviour
{
    private bool active = true;

    private enum PickupType { Health, Ammo }

    [SerializeField, Tooltip("In seconds")] 
    private int respawnTime = 30;
    [Header("Pickups")]
    [SerializeField] private PickupType pickupType;

    [Space]
    [SerializeField, Tooltip("The amount of health the player should gain")]
    private int healthAmount = 500;

    [Header("Bob")]
    [SerializeField] private Transform[] moveTransforms;
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private float spinSpeed = .3f;
    private int index;

    // How close the indicator has to be in order to go to the next point
    private const float moveThreshold = .2f;

    [Header("Icons")]
    [SerializeField] private GameObject healthIcon;
    [SerializeField] private GameObject ammoIcon;

    private void Start()
    {
        switch (pickupType)
        {
            case PickupType.Health:
                healthIcon.SetActive(true);
                ammoIcon.SetActive(false);
                break;

            case PickupType.Ammo:
                ammoIcon.SetActive(true);
                healthIcon.SetActive(false);
                break;
        }
    }

    private void FixedUpdate()
    {
        if(!active) return;

        Bob();
    }

    private void OnTriggerEnter(Collider other)
    {
        MovementController moveController = other.GetComponentInParent<MovementController>();
        if (!moveController) return; 

        bool interacted = false;
        // Depending on whether the pickup is health or ammo
        switch (pickupType)
        {
            case PickupType.Health:
                HealthManager health = other.GetComponentInParent<HealthManager>();

                if (health && IsServer)
                {
                    if(health.GainHealth(healthAmount)) interacted = true;
                }
                break;

            case PickupType.Ammo:
                WeaponController[] weapons = moveController.gameObject.GetComponentsInChildren<WeaponController>();
                   
                // If the thing that entered has a weapon controller
                if (weapons[0])
                {
                    foreach (WeaponController weapon in weapons)
                    {
                        // if the weapon doesn't use the overheat ammo type, refill ammo
                        if (weapon.GetAmmoType() == WeaponStats.AmmoType.Ammo)
                        {
                            if (weapon.RefillAmmo()) interacted = true;
                        }
                    }
                }
                break;
        }

        //if (interacted) _DeactivateServerRpc();
        if (interacted) DeactivateClientRpc();
    }

    private void Bob()
    {
        // slowly rotate the the cubes
        transform.localEulerAngles = new(transform.localEulerAngles.x, transform.localEulerAngles.y + spinSpeed, transform.localEulerAngles.z);

        Vector3 targetPos = moveTransforms[index].position;
        healthIcon.transform.position = Vector3.Lerp(healthIcon.transform.position, targetPos, Time.deltaTime * lerpSpeed);
        ammoIcon.transform.position = healthIcon.transform.position;

        if (Vector3.Distance(healthIcon.transform.position, targetPos) < moveThreshold)
        {
            index++;
            index = index == moveTransforms.Length ? 0 : index;
        }
    }

    [ClientRpc]
    private void DeactivateClientRpc()
    {
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        active = false;
        healthIcon.SetActive(false);
        ammoIcon.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        active = true;
        switch (pickupType)
        {
            case PickupType.Health:
                healthIcon.SetActive(true);
                break;

            case PickupType.Ammo:
                ammoIcon.SetActive(true);
                break;
        }
    }
}
