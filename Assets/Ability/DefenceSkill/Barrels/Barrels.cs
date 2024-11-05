using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Barrels : MonoBehaviour
{
  public GameObject barrels;
  public Transform spawnTransforms;

  public float cooldownTime = 30f;
  private float nextFireTime;
    private bool spawned;

    private void OnDefensiveWeapon()
    {
        if (Time.time > nextFireTime)
        {
            Instantiate(barrels, spawnTransforms.position, Quaternion.identity);
            spawned = true;
        }
    }

  private void Update()
  {
    if (Time.time > nextFireTime)
    {
      //if (Input.GetKeyDown(KeyCode.B))
      //{
       // Instantiate(barrels, spawnTransforms.position, Quaternion.identity);
       // nextFireTime = Time.time + cooldownTime;
                spawned = false;
     // }
    }
   
  }

 
}
