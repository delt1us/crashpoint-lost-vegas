using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    // Adjust this value to control the knockback force
    public float knockbackForce = 1000f;

    private void OnCollisionStay(Collision collision)
    {
        //Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();

        //if (otherRigidbody != null)
        //{
        //    // Calculate the direction of knockback
        //    Vector3 knockbackDirection = collision.transform.position - transform.position;
        //    knockbackDirection.y = 0;

        //    // Apply the knockback force to the collided object
        //    otherRigidbody.AddForce(knockbackDirection.normalized * knockbackForce * 10f, ForceMode.Impulse);
        //}

        Rigidbody otherRigidbody = collision.collider.GetComponentInParent<Rigidbody>();

        if (otherRigidbody != null)
        {
            // Apply the knockback force to the collided object
            otherRigidbody.AddForce(-knockbackForce * 10f * collision.contacts[0].normal - otherRigidbody.transform.forward, ForceMode.Impulse);
        }
    }
}

// add a force which * the mass of the rigid bodies of the car