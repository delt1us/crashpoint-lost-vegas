/**************************************************************************************************************
* Grenade script
* The script used to give the grenade prefab its functionality. It spawns the explosion prefab whenever it blows up. It will blow up on impact 
* if the projectile stats tell it too otherwise it'd use the fuse time which is also set in the projectile stats.
*
* Created by Dean Atkinson-Walker 2023
*
* Change log:
*       Dean  - The grenades can either explode on impact or by using the fuse which is set in the projectile stats scriptable object
*       Armin - 09/08/23 - Added multiplayer support
*       Armin - 13/08/23 - Removed friendly fire
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class NadeScript : NetworkBehaviour
{
    private GameObject playerCaller;

    [SerializeField] private ProjectileStats projStats;
    [SerializeField] private GameObject explosion;

    private bool impact;
    private float timer;
    private bool timerOn;
    private bool blown;

    private Rigidbody nadeBody;

    private void Awake()
    {
        nadeBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        impact = projStats.FuseTime <= 0;
        timerOn = !impact;

        nadeBody.AddForce(transform.forward * projStats.LaunchForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if (impact || !timerOn || !IsServer) return;

        timer += Time.deltaTime;
        if (timer > projStats.FuseTime)
        {
            timerOn = false;
            Explode();
        }
    }

    // Adds the velocity of the player's car to the object to make the movement more realistic.
    public void Throw(Vector3 velocity, GameObject caller)
    {
        playerCaller = caller;
        nadeBody.velocity += velocity;
    }

    private void FixedUpdate()
    {
        // Additional gravity to make the arc more apparent.
        nadeBody.AddForce(Physics.gravity * projStats.GravityMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) return;
        if (!impact || !IsServer) return;

        // Ignore if it hits anything in the player caller's gameObject
        HealthManager healthManager = other.GetComponentInParent<HealthManager>();
        bool isOtherCallerOfThisNade = false;

        if (healthManager) isOtherCallerOfThisNade = healthManager.gameObject == playerCaller;

        if (!healthManager || !isOtherCallerOfThisNade) Explode();
    }

    private void Explode()
    {
        if (blown) return;

        Player _player = playerCaller.GetComponentInParent<Player>();

        int teamId = _player.teamId.Value;
        int callerId = _player.id.Value;

        
        GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
        newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius, projStats.ExplosionForce, teamId, callerId);
        newExplo.GetComponent<NetworkObject>().Spawn();
        GetComponent<NetworkObject>().Despawn();

        blown = true; 
    }
}
