/**************************************************************************************************************
* Ground Hook
* Used to control what happens whenever the grapple is spawned. Tells the GrappleGround script to start pulling and deals damage on player impacts. 
*
* Created by Envy Cham 2023
* 
* Change Log:
*   Envy -  Created functionality     
*   Dean -  Made it work with multiplayer
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class GroundHook : NetworkBehaviour
{
  [SerializeField] private float hookForce = 25f;
        private int damage = 10;

  GrappleGround Ggrapple;
  Rigidbody rb;
  LineRenderer lineRenderer;

    private NetworkObject netObj;

    private GameObject hitCar;
    private int callerTeamId;

    private void Start()
    {
        netObj = GetComponent<NetworkObject>();
    }


    public void Initialize(GrappleGround grapple, int damage, int callerTeam)
  {
        this.damage = damage;

        callerTeamId = callerTeam;

    Ggrapple = grapple;
    rb = GetComponent<Rigidbody>();
    lineRenderer = GetComponent<LineRenderer>();
    rb.AddForce(transform.forward * hookForce, ForceMode.Impulse);
  }


    void Update()
    {
        if (!IsServer) return;
        
        SyncHookClientRpc();
    }

    [ClientRpc]
    private void SyncHookClientRpc()
    {
        if(hitCar) transform.position = hitCar.transform.position;

        if (!Ggrapple) return;
        Vector3[] positions = new Vector3[] { transform.position, Ggrapple.GetSpawnTransform()};
        lineRenderer.SetPositions(positions);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) return;

        HealthManager health = other.GetComponentInParent<HealthManager>();
        // If the player manages to shoot themselves with the grapple, do nothing
        if (health)
        {
            if(netObj.OwnerClientId == other.GetComponentInParent<NetworkObject>().OwnerClientId) return;
        }

        rb.useGravity = false;
        rb.isKinematic = true;


        Ggrapple.StartPull();

        // Deal damage on server ++ If the hit object is a player (Dean)
        if (!IsServer || !health) return;

        hitCar = health.gameObject;
        health.TakeDamage(damage, callerTeamId);
    }
}
