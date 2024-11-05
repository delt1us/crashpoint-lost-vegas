/**************************************************************************************************************
* ShockMissile
* 
* Modified missile with shock effect using sleep mode in RigidBody 
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

public class ShockMissile : MonoBehaviour
{
  private Rigidbody missleBody;
  Rigidbody rb;
  [SerializeField] private ProjectileStats projStats;
  [SerializeField] private GameObject explosion;

  //[SerializeField] private AudioSource MissileExplosionSound;
  private GameObject playerCaller;
  private bool IsSleeping;

 [SerializeField] private float sleepDuration = 5f;
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
  private void OnTriggerEnter(Collider other)
  {
    if (other == playerCaller || other.CompareTag(gameObject.tag) || other.CompareTag("VFX")) return;

    Debug.Log("Success");
    rb = other.GetComponentInParent<Rigidbody>();
    rb.sleepThreshold = 0.01f;
    StartCoroutine(SleepForDuration());
    

    GameObject newExplo = Instantiate(explosion, transform.position, transform.rotation);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
    Destroy(gameObject);
  }

  private IEnumerator SleepForDuration()
  {
    IsSleeping = true;
    rb.Sleep(); 
    //Debug.Log("Rigidbody is sleeping for " + sleepDuration + " seconds.");
    yield return new WaitForSeconds(sleepDuration);
    rb.WakeUp();
    //Debug.Log("Rigidbody is awake now.");
    IsSleeping = false;
  }
}

  
