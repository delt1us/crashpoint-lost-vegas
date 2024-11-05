/**************************************************************************************************************
* Heat-Seeking Missle
* Similar functionality to the "Missle" script except it follows a target.
*
* Created by Daniel Greeves 2023
* 
* Change Log:
*   Daniel - All the players are able to be targetted - no matter the range. The missle flies towards it.
*   Dean -   Implemented damage depending on the distance from the center of the explosion (using the "Explosion" prefab).
*            Created a scriptable object that will store the values that make up the explosion (blast radius, damage....).
*            Missles always go to the closest player and are limited to a defined range.
*   Dean -   The player has to be facing the direction of the target for the target to be valid
*   Armin - 10/08/23 - Made homing work (with help from Dean)
*   Armin - 13/08/23 - Removed friendly fire
***************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Heat_Seeking_Missile_1 : NetworkBehaviour
{
    private GameObject playerCaller;

    // Floats
    [SerializeField] private ProjectileStats projStats;

    // Body
    private Rigidbody body;

    // Target to find
    public  GameObject Target { get; private set; }
    [SerializeField] private float targetRange = 200;

    private float homeTimer;
    private float homeDelay = .5f;
    private bool homeOn;

    // Game objects
    [SerializeField] private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public void SetCaller(GameObject caller)
    {
        playerCaller = caller.GetComponentInChildren<MovementController>().gameObject;
        // Get caller ClientId
        // Get client rpc params for the owner
        ulong[] target = new ulong[1];
        target[0] = playerCaller.GetComponentInParent<NetworkObject>().OwnerClientId;
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = target
            }
        };
        // Run FindTarget() on caller
        StartCoroutine(LockOnTimerCoroutine());
        
        IEnumerator LockOnTimerCoroutine()
        {
            yield return new WaitForSeconds(homeDelay);
            FindTargetClientRpc(clientRpcParams);
            homeOn = true;
        }
    }

    // Find target on game start
    [ClientRpc]
    private void FindTargetClientRpc(ClientRpcParams clientRpcParams = default)
    {
        // Getting the player caller from the instance owner's instance
        playerCaller = GameObject.Find("ThisClientsPlayer").GetComponentInChildren<InputManager>().gameObject;

        List<GameObject> potTargets = new();

        Player netPlayer = playerCaller.GetComponentInParent<Player>();
        GameObject playerHolder = GameObject.FindGameObjectWithTag("PlayerHolder");
        for (int i = 0; i < playerHolder.transform.childCount; i++)
        {
            Player netTarget = playerHolder.transform.GetChild(i).GetComponent<Player>();
            GameObject target = netTarget.GetComponentInChildren<MovementController>().gameObject;
            if (netTarget.GetComponentInChildren<MovementController>()
                && netTarget.teamId.Value != netPlayer.teamId.Value)
            {
                potTargets.Add(target);
            }
        }

        float shortDistance = 999999;

        // A starting point.
        foreach (GameObject target in potTargets)
        {
            Collider targetCollider = null;
            if(target.GetComponentInChildren<MeshCollider>()) targetCollider = target.GetComponentInChildren<MeshCollider>();
            else if(target.GetComponentInChildren<BoxCollider>()) targetCollider = target.GetComponentInChildren<BoxCollider>();
            else if(target.GetComponent<BoxCollider>()) targetCollider = target.GetComponent<MeshCollider>();
            else if(target.GetComponent<BoxCollider>()) targetCollider = target.GetComponent<BoxCollider>();

            // If the target isn't in the player's view, go to the next iteration
            //if (targetCollider.bounds == null) continue;
            if (!InSight(targetCollider.bounds)) continue;

            float distance = Vector3.Distance(target.transform.position, playerCaller.transform.position);

            // Find the shortest distance
            if(distance < shortDistance) shortDistance = distance;
            else continue;
        }

        if (shortDistance < targetRange)
        {
            foreach(GameObject target in potTargets)
            {
                // Match the distance with the target
                if (Mathf.Approximately(shortDistance, Vector3.Distance(target.transform.position, playerCaller.transform.position)))
                {
                    // If it matches, it'll be the new target
                    Target = target;
                    break;
                }
            }
        }

        if (!Target) return;
        
        // Get ClientId of Target
        ulong targetClientId = Target.GetComponentInParent<NetworkObject>().OwnerClientId;
        // Send ClientId of Target back to server
        _SetTargetServerRpc(targetClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void _SetTargetServerRpc(ulong targetClientId)
    {
        // Set Target on server
        GameObject playerHolderGameObject = GameObject.FindWithTag("PlayerHolder");
        GameObject target = null;
        for (int i = 0; i < playerHolderGameObject.transform.childCount; i++)
        {
            if (playerHolderGameObject.transform.GetChild(i).GetComponent<NetworkObject>().OwnerClientId == targetClientId)
            {
                target = playerHolderGameObject.transform.GetChild(i).GetComponentInChildren<MovementController>().gameObject;
            }
        }
        
        Target = target;
    }

    private bool InSight(Bounds bounds)
    {
        Camera playerCam = playerCaller.GetComponentInChildren<Camera>();
        Plane[] camFrustrum = GeometryUtility.CalculateFrustumPlanes(playerCam);

        // Whether or not the inputted parameter is within the camera's frustrum
        bool inSight = GeometryUtility.TestPlanesAABB(camFrustrum, bounds);

        return inSight;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!IsServer) return;

        // Makes the forward axis point towards the target...
        if (Target && homeOn)
        {
            transform.LookAt(Target.transform.position);
        }
        // The missle moves along the forward axis.
        body.velocity = transform.forward * projStats.LaunchForce;
    }

    // Collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VFX") || other.gameObject.layer == LayerMask.NameToLayer("UI") || other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) return;

        Explode();
    }

    private void Explode()
    {
        if (!IsServer) return;

        Player _player = playerCaller.GetComponentInParent<Player>();

        int teamId = _player.teamId.Value;
        int callerId = _player.id.Value;

        GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
        newExplo.GetComponent<NetworkObject>().Spawn();
        newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius, projStats.ExplosionForce, teamId, callerId);

        GetComponent<NetworkObject>().Despawn();
    }

}
