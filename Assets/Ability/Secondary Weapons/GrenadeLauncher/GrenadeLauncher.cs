/**************************************************************************************************************
* GrenadeLauncher
* 
* 
* Modified grenade for grenade launcher
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

public class GrenadeLauncher : MonoBehaviour
{
  [SerializeField] private ProjectileStats projStats;
  [SerializeField] private GameObject explosion;

  [SerializeField] private Rigidbody GrenadeBody;

 
  private void Start()
  {
    
    Destroy(gameObject, projStats.LifeSpan);
    GrenadeBody.AddForce(transform.forward * projStats.LaunchForce, ForceMode.Impulse);
  }

  public void Throw(Vector3 velocity)
  {
    GrenadeBody.velocity += velocity;
  }

  private void FixedUpdate()
  {
    
    GrenadeBody.AddForce(Physics.gravity * projStats.GravityMultiplier);
  }

  
  private void OnCollisionEnter(Collision collision)
  {
   if (collision.gameObject.tag == "VFX") return;
    Explode();
    Destroy(this.gameObject);
  }

  private void Explode()
  {

   
    GameObject newExplo = Instantiate(explosion, transform);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
    //Destroy(this.gameObject);



  }
}
