/**************************************************************************************************************
* Speed Delivery
* The movement ability for the delivery truck. It spawns a speed box and manages the ability's cooldown and availabilty.
*
* Created by Dean Atkinson-Walker 2023
*
*
***************************************************************************************************************/

using System;
using Unity.Netcode;
using UnityEngine;

public class SpeedDelivery : NetworkBehaviour
{
    private MovementController moveController;

    [Header("Spawning")]
    [SerializeField, Tooltip("Input Speed Pack prefab")] private GameObject speedPack;
    [SerializeField] private Transform spawnPoint;
    private Vector3 playerVelocity;

    [Header("Box Properties")]
    [SerializeField] private float throwForce = 50;
    [SerializeField] private byte by_maxCharges = 2;
    private byte by_charges;

    [Header("Recharging")]
    [SerializeField, Tooltip("The time in seconds it takes to regain one box."), Range(1, 60)] 
    private float chargeDelay = 12;
    private float chargeTimer;

    //// Cooldown
    private bool timerOn;
    private float throwTimer;
    [SerializeField, Tooltip("The time in seconds it takes to allow the player to throw another box."), Range(1, 60)] 
    private float throwCooldown = 5;

    private bool canThrow = true; 

    private void Start()
    {
        by_charges = by_maxCharges;

        moveController = GetComponent<MovementController>();
    }

    private void Update()
    {
        Cooldown();
        ChargeRegen();
    }

    private void OnMovementAbility()
    {
        if (by_charges < 1 || !canThrow) return;
        
        // The SpeedPack gets spawned on the server
        _ThrowServerRpc();

        by_charges--;
        canThrow = false;
        timerOn = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void _ThrowServerRpc()
    {
        playerVelocity = moveController.GetVelocity();

        // Spawning the object on the server
        GameObject box = Instantiate(speedPack, spawnPoint.position, spawnPoint.rotation);

        // Throwing the box....
        SpeedPack newSpeedPack = box.GetComponent<SpeedPack>();
        box.GetComponent<NetworkObject>().Spawn();

        newSpeedPack.Launch(playerVelocity, throwForce);

        // Setting the despawn lifespan on the server so that it can be destroyed.
        newSpeedPack._DespawnTimeServerRpc();
    }

    private void Cooldown()
    {
        if (!timerOn) return;

        throwTimer += Time.deltaTime;
        if (throwTimer > throwCooldown)
        {
            throwTimer = 0;
            canThrow = true;

            timerOn = false;
        }
    }

    private void ChargeRegen()
    {
        if (by_charges == by_maxCharges) return;

        if (chargeTimer > chargeDelay)
        {
            by_charges++;
            chargeTimer = 0;
        }

        chargeTimer += Time.deltaTime;
    }


    //////////////////////////////////////// DEBUGGING ////////////////////////////////////////


    public byte GetMaxCharges()
    {
        return by_maxCharges;
    }

    public byte GetCharges()
    {
        return by_charges;
    }

    public float GetCooldown()
    {
        return throwCooldown;
    }

    public float GetCurrentThrowTimer()
    {
        return throwTimer;
    }

    public float GetChargeDelay()
    {
        return chargeDelay;
    }

    public float GetChargeTimer()
    {
        return chargeTimer;
    }

}

//public class Cooldown
//{
//    public event EventHandler elapsed;

//    private bool timerOn = true;
//    private float timer;
//    private readonly float duration;

//    protected virtual void OnElapsed()
//    {
//        if (elapsed != null) elapsed(this, EventArgs.Empty);
//    }

//    public Cooldown(float duration)
//    {
//        this.duration = duration;
//    }

//    public void Update()
//    {
//        if (!timerOn) return;

//        timer += Time.deltaTime;
//        if (timer > duration)
//        {
//            timer = 0;
//            timerOn = false;

//            OnElapsed();
//        }
//    }
//}