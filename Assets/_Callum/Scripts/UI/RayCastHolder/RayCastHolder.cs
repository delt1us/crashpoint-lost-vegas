using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHolder : MonoBehaviour
{
    public LayerMask raycastMask;

    public void CastRay()
    {
       
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
        {
           
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
        }
    }
}
