/**************************************************************************************************************
* BarrelsPhysic
* For barrels physic 
*
* Created by Envy Cham 2023
* 
* Change Log:
*       Envy - 8/3 Added Health Manager, install destroy after damage
*       Dean - Added sfx on impact
*       Dean - Removed the lifespan of the object (it stays in the scene until it's destroyed. 
*              Only 1 barrel is spawned (made despawning them after being shot easier)...
*            
***************************************************************************************************************/


using Unity.Netcode;
using UnityEngine;

public class BarrelPhysic : NetworkBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private int lifeSpan = 30;
    HealthManager healthManager;

    private void Start()
    {
        healthManager = GetComponent<HealthManager>(); 
    }

    private void Update()
    {
        if (!IsServer) return;

        // Destroy on the server once it has no health (since the weapon controller doesn't know what to do when it hits a barrel)
        if (healthManager.Dead) GetComponent<NetworkObject>().Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioSource src = GetComponent<AudioSource>();
        src.PlayOneShot(src.clip);
    }

}
