/**************************************************************************************************************
* Gun Camera Controller
* Sole purpose is to smoothen the trasnsition between ADS and the freelook camera.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using Cinemachine;
using UnityEngine;

public class GunCamController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField, Tooltip("If the ADS look point is further infront than the freelook point, Uncheck Inherit position in the Virtual Camera.")] 
    private Transform adsPoint;
    private WeaponController controller;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
    }

    private void Update()
    {
        if(!controller.Active) return;

        bool aiming = GetComponentInParent<ActionController>().IsAiming();

        vCam.LookAt = aiming ? adsPoint : null;
        vCam.Follow = aiming ? adsPoint : null;
    }

}
