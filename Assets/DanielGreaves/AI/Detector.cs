using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerStay;
    public UnityEvent<Collider> onTriggerExit;

    // Wall collision on trigger enter
    private void OnTriggerEnter(Collider other)
    {
       // if (other.gameObject.tag == "Wall")
       // {
            //onTriggerEnter?.Invoke(other);
       // }
    }

    // Wall collision on trigger stay
    private void OnTriggerStay(Collider other)
    {
       // if (other.gameObject.tag == "Wall")
       // {
       //     //onTriggerStay?.Invoke(other);
       // }
    }

    // Wall collision on exit
    private void OnTriggerExit(Collider other)
    {
       // if (other.gameObject.tag == "Wall")
       // {
            //onTriggerStay?.Invoke(other);
       // }
    }
}
