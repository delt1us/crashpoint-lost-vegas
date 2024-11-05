/**************************************************************************************************************
* Crate Camera.
* Used to give the colliders in each of the crates functionality.
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using UnityEngine;

public class CrateCam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject carObj = null;
        GameObject parent = GetComponentInParent<MeshRenderer>().gameObject;
        if(other.GetComponentInParent<MovementController>()) carObj = other.GetComponentInParent<MovementController>().gameObject;

        // Since the camera controller is on the virtual camera, its only parent is the main car game object... Needs to cast to that to get the camera script.
        // If the crate is on a steep hill... the camera would need to go lower than the default value (passes the angle of head gameObject
        if (carObj) if(carObj.GetComponentInChildren<CameraController>()) carObj.GetComponentInChildren<CameraController>().EnterCrate(parent.transform.eulerAngles.x);
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject carObj = null;
        if (other.GetComponentInParent<MovementController>()) carObj = other.GetComponentInParent<MovementController>().gameObject;

        // Since the camera controller is on the virtual camera, its only parent is the main car game object... Needs to cast to that to get the camera script.
        if (carObj) if (carObj.GetComponentInChildren<CameraController>()) carObj.GetComponentInChildren<CameraController>().ExitCrate();
    }

}
