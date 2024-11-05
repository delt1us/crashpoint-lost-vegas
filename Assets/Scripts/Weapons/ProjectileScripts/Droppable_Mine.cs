/**************************************************************************************************************
* Droppable Mine
* A script the controls the behaviour of mines and gives them functionality.
* 
*  -- Used in the mines that are placed around the level (the player doesn't spawn them in). --
*
* Created by Daniel Greaves 2023
* 
* Change Log:
*   Daniel -  Added functionality to the mines (blows up whenever a player touches it).
*   Dean   -  Made the mine stick to the floor whenever it lands.
*   Dean   -  Made it work in multiplayer.
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class Droppable_Mine : NetworkBehaviour
{
    [SerializeField] private short damage;
    [SerializeField] private short blastRadius;

    // Particle system
    [SerializeField] private GameObject explosion;

    // Collisions
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !other.GetComponentInParent<MovementController>()) return;

        GameObject newExplo = Instantiate(explosion, transform.position, Quaternion.identity);

        // Spawn the explosion
        newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(damage, blastRadius, 4000);
        newExplo.GetComponent<NetworkObject>().Spawn();

        // Despawn the mine
        GetComponent<NetworkObject>().Despawn();
    }
}
