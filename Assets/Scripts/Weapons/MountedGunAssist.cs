/**************************************************************************************************************
* Mounted Gun Assist
* Used to manage the players that are in the lock-on range.
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Dean -   When something lock-on-able enters the trigger it adds the game object to a list in the "MountedGun" script.
*            
* (UNUSED)
*            
***************************************************************************************************************/

using UnityEngine;

public class MountedGunAssist : MonoBehaviour
{
    private MountedGun gunScript;
    private TopGun topGunScript;

    private void Start()
    {
        if(GetComponentInChildren<MountedGun>()) gunScript = GetComponentInChildren<MountedGun>();
        if (GetComponentInChildren<TopGun>()) topGunScript = GetComponentInChildren<TopGun>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only add the game object if its one of the player types.
        if (!(other.CompareTag("Player") || other.CompareTag("Target") || other.CompareTag("AIPlayer")) || other.gameObject == gameObject) return;

        if(gunScript) gunScript.AddObjInRange(other.gameObject);
        if(topGunScript) topGunScript.AddObjInRange(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if(gunScript) gunScript.RemoveObjInRange(other.gameObject);
        if(topGunScript) topGunScript.RemoveObjInRange(other.gameObject);
    }
}
