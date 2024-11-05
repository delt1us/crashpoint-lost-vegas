/**************************************************************************************************************
* ClownHead
* For "ClownHead" the punching toy physic 
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

public class ClownHead : MonoBehaviour
{
 private LineRenderer lineRenderer;
  private Rigidbody rb;

  private void Start()
  {
    
    rb = GetComponent<Rigidbody>();
  }

  private void Update()
  {
    lineRenderer = GetComponent<LineRenderer>();
    transform.Translate(Vector3.forward * 80 * Time.deltaTime);
   // rb.AddForce(transform.forward * 50 *Time.deltaTime, ForceMode.Acceleration);

    DestroyObjectDelayed();
  }

  private void DestroyObjectDelayed()
  {
    Destroy(gameObject, 0.2f);
  }
}
