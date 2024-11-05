// Created by Daniel Greaves

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive_Prop : MonoBehaviour
{

    // Game objects
    // Particle system
    public GameObject Explosion_Particle_System;

    // Sound effects
    public AudioSource BarrelExplosionSound;


    // Collisions
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            // Play sound effect
            BarrelExplosionSound.Play();

            // Create particle system
            Instantiate(Explosion_Particle_System, transform.position, Quaternion.identity);

            // Destroy this game object
            Destroy(gameObject);
        }


        if (other.gameObject.tag == "Bullet_1")
        {

           // Play sound effect
           BarrelExplosionSound.Play();

           // Create particle system
           Instantiate(Explosion_Particle_System, transform.position, Quaternion.identity);

            // Destroy other game object
            Destroy(other.gameObject);

            // Destroy this game object
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Missile_1")
        {

            // Play sound effect
            BarrelExplosionSound.Play();

            // Create particle system
            Instantiate(Explosion_Particle_System, transform.position, Quaternion.identity);

            // Destroy other game object
            Destroy(other.gameObject);

            // Destroy this game object
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "HeatSeekingMissile")
        {

            // Play sound effect
            BarrelExplosionSound.Play();

            // Create particle system
            Instantiate(Explosion_Particle_System, transform.position, Quaternion.identity);

            // Destroy other game object
            Destroy(other.gameObject);

            // Destroy this game object
            Destroy(gameObject);
        }
    }
}
