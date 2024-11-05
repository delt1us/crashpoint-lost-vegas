/**************************************************************************************************************
* Firework
* 
* Modified version of missile to create random path
*
* Created by Envy Cham 2023
* 
* Change Log:
*
*
*            
***************************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
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
    transform.Rotate(Random.Range(-2f,2f), 0,Random.Range(-3f,3f));
    missleBody.AddForce(transform.forward * projStats.LaunchForce, ForceMode.Impulse);
  }

  public void SetCaller(GameObject caller)
  {
    playerCaller = caller;
  }

  // Collisions
  private void OnTriggerEnter(Collider other)
  {
    if (other == playerCaller || other.CompareTag(gameObject.tag) || other.CompareTag("VFX")) return;

    Explore();

   
  }

  private void OnCollisionEnter(Collision collision)
  {
    Explore();
  }
  private void Explore()
  {
    GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
    Destroy(gameObject);
  }
}
