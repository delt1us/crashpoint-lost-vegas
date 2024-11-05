using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ None ] 
// Purpose -                 [ Gives Player Health ] 
// Functions -               [  ]
// Dependencies -            [ None ]
// Notes -  Script to Test Players HealthBar [ Delete This Script ]
public class Health : MonoBehaviour
{
    public short currentHealth;
    public short maxHealth = 100;

    private bool isDead;

    public Healthbar healthbar;



    public void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(short damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);

        if (currentHealth < 0 && !isDead)
        {
            isDead = true;
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(short amount)
    {
        currentHealth += amount;
        healthbar.SetHealth(currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void Die()
    {
        isDead = true;
       
        Destroy(gameObject);
       
    }

   

}

