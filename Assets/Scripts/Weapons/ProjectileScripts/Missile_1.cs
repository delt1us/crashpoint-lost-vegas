/**************************************************************************************************************
* Missle
* Gives functionality to the missle weapon
*
* Created by Daniel Greeves 2023
* 
* Change Log:
*   Daniel - Missiles produce sound and vfx whenever they hit a something.
*   Dean -   Implemented damage depending on the distance from the center of the explosion (using the "Explosion" prefab).
*            Created a scriptable object that will store the values that make up the explosion (blast radius, damage....).
*            
*                                                   (UNUSED)
*            
***************************************************************************************************************/

using UnityEngine;

public class Missile_1 : MonoBehaviour
{
    private Rigidbody missleBody;

    [SerializeField] private ProjectileStats projStats;
    [SerializeField] private GameObject explosion;
        
    //[SerializeField] private AudioSource MissileExplosionSound;
    private GameObject playerCaller;

    void Start()
    {
        // Set this actor life span
        Destroy(gameObject, projStats.LifeSpan);

        missleBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        missleBody.velocity = transform.forward * projStats.LaunchForce;
    }

    public void SetCaller(GameObject caller)
    {
        playerCaller = caller;
    }

    // Collisions
    private void OnTriggerEnter (Collider other)
    {
        if (other == playerCaller || other.CompareTag(gameObject.tag) || other.CompareTag("VFX")) return;

        Vector3 spawnOffset = (transform.up * 10) + transform.forward * -10;

        GameObject newExplo = Instantiate(explosion, transform.position + spawnOffset, transform.rotation);
        //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
        Destroy(gameObject);
    }
}
