using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ DamagePlayer ] 
// Purpose -                 [ Damages Player ] 
// Functions -               [ 1. Checks for Collision With the Player ]
// Dependencies -            [ None ]
// Notes -  Script to Test Players HealthBar [ Delete This Script ]
public class DamagePlayer : MonoBehaviour
{
    public short attackDamage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null)
            {
                Debug.Log("Car Collision - Player Hit");
                Debug.Log(attackDamage);
                health.TakeDamage(attackDamage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthManager health = collision.gameObject.GetComponent<HealthManager>();
            if (health != null)
            {
                Debug.Log("Car Collision - Player Hit");
                Debug.Log(attackDamage);
                health.TakeDamage(attackDamage);
          }
        }
    }
}
