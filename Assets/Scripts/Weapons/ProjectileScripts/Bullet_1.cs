// Created by Daniel Greaves

using UnityEngine;

public class Bullet_1 : MonoBehaviour
{
    private Rigidbody bulletBody;
    [SerializeField] private float bulletSpeed = 1000;
    private short damage;

    private GameObject playerCaller;

    // Floats
    [SerializeField] private float LifeSpan = 4;

    // Game objects
    [SerializeField] private GameObject Bullet_Hit_Particle_System;

    void Awake()
    {
        // Set this actor life span
        Destroy(gameObject, LifeSpan);

        bulletBody = GetComponent<Rigidbody>();
    }

    public void SetCaller(GameObject caller)
    {
        playerCaller = caller;
    }

    public void SetDamage(short damage)
    {
        this.damage = damage;
    }

    private void FixedUpdate()
    {
        bulletBody.velocity = transform.forward * bulletSpeed;
    }

    // Collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag) || playerCaller? other.CompareTag(playerCaller.tag) : false) return;

        int teamId = playerCaller.GetComponentInParent<Player>() ? 
            playerCaller.GetComponentInParent<Player>().teamId.Value : 
            playerCaller.GetComponentInParent<NetworkAI>().teamId.Value;
        
        switch (other.tag) 
        {
            case "Player":
                other.GetComponentInChildren<HealthManager>().TakeDamage(damage, teamId);
                break;

            case "AIPlayer":
                other.GetComponentInChildren<HealthManager>().TakeDamage(damage, teamId);
                break;

            case "Target":
                Destroy(other.gameObject);
                break;
        }


        Instantiate(Bullet_Hit_Particle_System, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
