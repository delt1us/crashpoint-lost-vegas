// Created by Daniel Greaves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBoom : MonoBehaviour
{
    private Rigidbody bulletBody;
    private float speed = 1000;

    // Floats
    private float LifeSpan = 4;

    // Game objects
    [SerializeField] private GameObject Bullet_Hit_Particle_System;

    void Awake()
    {
        // Set this actor life span
        Destroy(gameObject, LifeSpan);

        bulletBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bulletBody.velocity = transform.forward * speed;
    }

    // Collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag)) return;

        switch (other.tag)
        {
            case "Target":
                Destroy(other.gameObject);
                break;
        }


        Instantiate(Bullet_Hit_Particle_System, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
