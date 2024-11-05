/**************************************************************************************************************
* Speed Pack
* The script used to give functionality to the speed boxes that are spawned by the delivery van.
*
* Created by Dean Atkinson-Walker 2023
*
*
***************************************************************************************************************/

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpeedPack : NetworkBehaviour
{
    private Rigidbody boxBody;

    private bool active;
    private float activeTimer;
    private const float activeDelay = .5f;
    private const float maxSpeed = 400;

    // Default lifespan 5 minutes (Range = 2 minutes - 10 minutes).
    [SerializeField, Range(120, 600)] private int lifeSpan = 300;

    private void Awake()
    {
        boxBody = GetComponent<Rigidbody>();
    }

    // The box can't be interacted with instantly to prevent the spawner from instantly collecting it
    private void Update()
    {
        if (active) return;

        activeTimer += Time.deltaTime;
        if (activeTimer > activeDelay) active = true;
    }

    [ClientRpc]
    public void LaunchClientRpc(Vector3 velocity, float throwForce, float boostForce)
    {
        boxBody.velocity = velocity;
        boxBody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    public void Launch(Vector3 velocity, float throwForce)
    {
        boxBody.velocity = velocity;
        boxBody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }


    // Despawns the object after its lifespan elapses
    [ServerRpc]
    public void _DespawnTimeServerRpc()
    {
        // Despawn the pack as soon as its initialised.
        StartCoroutine(LifeSpanCoroutine());

        IEnumerator LifeSpanCoroutine()
        {
            yield return new WaitForSeconds(lifeSpan);

            GetComponent<NetworkObject>().Despawn();
        }
    }

    [ServerRpc]
    private void _DespawnServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!active) return;

        MovementController moveController = other.GetComponentInParent<MovementController>();

        if (!moveController) return;

        // Can't boost anymore if the collider is going too faster
        //if(moveController.GetSpeed() < maxSpeed) moveController.SpeedPackClientRpc(boostForce);
        print($"car speed = {moveController.GetSpeed()}");
        if(moveController.GetSpeed() < maxSpeed) moveController.SpeedBoost();
        
        
        if(!IsOwner) return;
        // Destroy the box regardless
        _DespawnServerRpc();
    }
}
