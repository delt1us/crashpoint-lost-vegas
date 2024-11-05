//Created by Daniel Greaves. Still working on this

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;



public class AI_Machine_Gun_Controller : MonoBehaviour
{
    public GameObject RaycastSpawn;
    private float RaycastRange = 0.0f;
    private bool bDetectionRaycast;

    private float FireProjectileDelay = .2f;

    // Target to find
    public GameObject Target;


    float Rotation = 0;

    private float BulletSpeed = 0;


    [SerializeField] private Transform muzzle;



    // Bullet prefab
    public GameObject bulletPrefab;
   

    private bool bFiringProjectile;


 


    void Start()
    {
       
    }
    public float maxAngle = 35.0f;
    private Quaternion baseRotation;
    private Quaternion TargetRotation;


    private void FixedUpdate()
    {
       
        FindTarget();

        // If there is no target
        if (Target == null)
        {
            // If there is no target then find another target
            FindTarget();
        }



        //Target rotation
        //Quaternion TargetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation,  Rotation * Time.deltaTime);


        Vector3 look = Target.transform.position - transform.position;
        look.z = 0;

        Quaternion q = Quaternion.LookRotation(look);
        if (Quaternion.Angle(q, baseRotation) <= maxAngle)
            TargetRotation = q;

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * 2.0f);



        // Create raycast to detect walls infront of the AI car. When this happens, The AI will apply its breaks and slow down
        Vector3 Detection = Vector3.forward;
        Ray LineTrace = new Ray(RaycastSpawn.transform.position, RaycastSpawn.transform.TransformDirection(Detection * RaycastRange));


        // Draw raycast line
        Debug.DrawRay(RaycastSpawn.transform.position, RaycastSpawn.transform.TransformDirection(Detection * RaycastRange));

        // Raycast collision
        if (Physics.Raycast(LineTrace, out RaycastHit WallHit, RaycastRange))
        {
            // If raycast hits a wall
            if (WallHit.collider.tag == "Player")
            {

                bDetectionRaycast = true;

                // If raycast is true
                if (bDetectionRaycast == true)
                {
                    // Set delay
                    if (bFiringProjectile) return;
                    bFiringProjectile = true;

                    // Then call this function
                    Invoke("ResetProjectileFire_2", FireProjectileDelay);


                    // Spawn machine gun turrent function
                    var MountedGun = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
                    MountedGun.GetComponent<Rigidbody>().velocity = muzzle.forward * BulletSpeed;

                }

                // Else if the raycast has no target 
                else if (WallHit.collider.tag == null)
                {

                }

            }

        }

      
    }


    // Find target on game start
    void FindTarget()
    {
        Target = GameObject.FindWithTag("Player");
    }
    void ResetProjectileFire_2()
    {
        bFiringProjectile = false;
    }


}


