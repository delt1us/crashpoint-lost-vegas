/**************************************************************************************************************
* RocketMissiles
* 
* Modified version of missile
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

public class RocketMissiles : MonoBehaviour
{
  private Rigidbody missleBody;

  [SerializeField] private ProjectileStats projStats;
  [SerializeField] private GameObject explosion;

  //[SerializeField] private AudioSource MissileExplosionSound;

  private bool impact = false;  
  private GameObject playerCaller;

  void Start()
  {


    missleBody = GetComponent<Rigidbody>();
  }

  private void Update()
  {
    missleBody.AddForce(transform.forward * projStats.LaunchForce, ForceMode.Impulse);
  }



  private void OnTriggerEnter(Collider other)
  {

    if (other.CompareTag(gameObject.tag) || other.CompareTag("VFX")) return;

    {
      if(!impact)
      {
        Explode();
      }
     
    }



  }

 // private void OnCollisionEnter(Collision collision)
  //{

    //if (collision.gameObject.tag == "VFX") return;

    //{
      //if(!impact)

     // Explode();
    //}
  //}
    private void Explode()
  {
    impact = true;

    GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
    
    Destroy(gameObject);

  }


   // GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
   // Destroy(gameObject);
 // }
}
