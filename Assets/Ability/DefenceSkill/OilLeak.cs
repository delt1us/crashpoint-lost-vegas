using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Oilleak : MonoBehaviour
{
  private float speed = -2f;

  private void OnTriggerEnter(Collider other)
  {
    transform.Translate(Vector3.forward * Time.deltaTime * speed);
  }

  private void OnTriggerExit(Collider other)
  {
    
  }

  private void Update()
  {
    Destroy(gameObject, 45f);
  }
  
    
  
}
