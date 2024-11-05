/**************************************************************************************************************
* Ground Ground
* Allows players to spawn in the grapple and performs the reeling in action. Also used to destroy the hook. 
*
* Created by Envy Cham 2023
* 
* Change Log:
*   Envy -  Created functionality     
*   Dean -  Made it work with multiplayer
*   Dean -  Put a cooldown on the grapple
***************************************************************************************************************/

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GrappleGround : NetworkBehaviour
{
    [SerializeField] private WeaponStats stats;

    [Space]
  [SerializeField] float pullSpeed = 0.5f;
  [SerializeField] float stopDistance = 4f;
  [SerializeField] GameObject hookPrefab;
  [SerializeField] Transform shootTransform;

  GroundHook gHook;
  bool pulling;
  Rigidbody rb;
    private AudioManager audioManager;


    private bool canShoot = true;

    // Cooldown
    private bool cooldownOn;
    private float cooldownTimer;
    private bool canceled;
    private float cooldown;


  void Start()
  {
    rb = GetComponent<Rigidbody>();
    pulling = false;
        audioManager = GetComponent<AudioManager>();
        audioManager.CreateAudioSource("grapple");
  }


    private void OnMovementAbility()
    {
        if (!canShoot) return;

        canceled = false;

        if (!gHook)
        {
            StopAllCoroutines();
            pulling = false;

            _SpawnGrappleServerRpc();

            StartCoroutine(DestroyHookAfterLifetime());
            audioManager.PlayGrapple();
            canShoot = false;
            cooldownOn = true;
        }
    }

    [ServerRpc]
    private void _SpawnGrappleServerRpc()
    {
        GameObject newGrapple = Instantiate(hookPrefab, shootTransform.position, shootTransform.rotation);

        Player ownerPlayer = GetComponentInParent<Player>();

        // Get the network obj and spawn it to everyone... using the ownerId from the Player script to set the owner 
        newGrapple.GetComponent<NetworkObject>().SpawnWithOwnership(ownerPlayer.OwnerClientId);

        SpawnGrappleClientRpc(newGrapple.name, ownerPlayer.teamId.Value);
    }

    [ClientRpc]
    private void SpawnGrappleClientRpc(string grappleName, int callerTeam)
    {
        GameObject newGrapple = GameObject.Find(grappleName);

        gHook = newGrapple.GetComponent<GroundHook>();
        gHook.Initialize(this, stats.BaseDamage, callerTeam);
    }

    private void OnDetachGrapple()
    {
        canceled = true;
        DestroyHook();
    }


    private void Update()
    {
        Cooldown();

        if (!pulling || !gHook) return;

        if (Vector3.Distance(transform.position, gHook.transform.position) <= stopDistance)
        {
            DestroyHook();
        }
        else
        {
            rb.AddForce((gHook .transform.position - transform.position).normalized * pullSpeed, ForceMode.VelocityChange);
        }
    }

    private void Cooldown()
    {
        cooldown = canceled ? stats.ShortCooldownDelay : stats.LongCooldownDelay;

        // Only run if the short cooldown timer is on and the long cooldown is off.
        if (!cooldownOn) return;

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > cooldown)
        {
            cooldownTimer = 0;
            canShoot = true;

            cooldownOn = false;
        }
    }

    public Vector3 GetSpawnTransform()
    {
        return shootTransform.position;
    }


  public void StartPull()
  {
    pulling = true;
  }

  private void DestroyHook()
  {
    if (!gHook) return;

    pulling = false;

    if (!IsOwner) return;
    _DespawnHookServerRpc();
  }

    [ServerRpc]
    private void _DespawnHookServerRpc()
    {
        gHook.GetComponent<NetworkObject>().Despawn();
    }

    private IEnumerator DestroyHookAfterLifetime()
    {
        yield return new WaitForSeconds(5f);

        DestroyHook();
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public float GetCooldownTimer()
    {
        if (gHook) return 0; 
        return cooldownTimer;
    }
}
