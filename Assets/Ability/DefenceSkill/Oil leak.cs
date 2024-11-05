using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.CompilerServices;

public class OilLeak : MonoBehaviour
{
  private float speed = -1000;
 // private bool isSlowDown = false;

  private float duartionTime = 15;
  private float endTime = 0;
  private float time = 5;
 

  Rigidbody rb;

  private void Awake()
  {
    
  }
  private void OnCollisionEnter(Collision collision)
  {
    //if ((LayerMask.GetMask("PlayerCar") & 1 << collision.gameObject.layer) > 0)
      // if(other.tag == "Player")
      //Debug.Log("Suceess");

    //other.gameObject.GetComponent<MovementController>().enabled = false;
    //StartCoroutine(SlowMovement(other.gameObject));
    // other.gameObject.transform.Translate(Vector3.forward * Time.time * speed);
    //collision.transform.position += new Vector3(0, 0, 1000);
    //other.transform.position += new Vector3(0, 0, 1000);
  }
  private void OnTriggerEnter(Collider other)
  {
    if ((LayerMask.GetMask("PlayerCar") & 1 << other.gameObject.layer) > 0)
   // if(other.tag == "Player")
      Debug.Log("Suceess");

    //other.gameObject.GetComponent<MovementController>().enabled = false;
    //StartCoroutine(SlowMovement(other.gameObject));
     other.gameObject.transform.Translate(Vector3.forward * Time.time * speed);
    //other.GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
    //other.transform.position += new Vector3(0, 0, 1000);






    //SlowDown(other);

  }

  private void SlowDown(Collider other)
  {
      
      other.gameObject.transform.Translate(Vector3.forward * speed);
    
  }

  private void OnTriggerExit(Collider other)
  {
    if ((LayerMask.GetMask("PlayerCar") & 1 << other.gameObject.layer) > 0)
      Debug.Log("Out");
    
  }
 

  private void Update()
  {
    Destroy(gameObject, 15f);


    
  }

  private IEnumerator SlowMovement(GameObject other)
  {
    if(other.GetComponent<Rigidbody>())
    {
      Rigidbody rb = other.GetComponent<Rigidbody>();
      rb.velocity = new Vector3( 0, 100, 0);
    }
    endTime = Time.time + duartionTime;

    yield return null;
    
  }
 
}
