/**************************************************************************************************************
* Top Gun
* A controller for the Top gun prefab. This script is almost identical to MountedGun. This script is needed since the new gun models has a more complex hierarchy.
* Reads inputs from the input manager to rotate the turret - the sole purpose of this is to rotate the gun (not shoot)
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class TopGun : MonoBehaviour
{
    // Also used as a link to get the transform of the actual car...
    private InputManager inputManager;

    private bool active;
    private int axisInversion = -1;

    [SerializeField] Camera cam;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform lookPoint;

    [Header("Rotators")]
    [SerializeField] private Transform vertRotator;
    [SerializeField] private Transform horiRotator;

    private bool aiming;
    private ActionController ac;

    private Vector3 rot;

    [Header("Rotation")]
    [SerializeField, Range(10, 45), Tooltip("The maximum angle the turret is allowed to rotate from the center (Horizontally).")]
    private float rotConstraint_h = 45;

    [SerializeField, Range(0, 10), Tooltip("The maximum angle the turret is allowed to rotate from the center (Vertically).")]
    private float upConstraint = 2;

    [SerializeField, Range(0, 45), Tooltip("The maximum angle the turret is allowed to rotate from the center (Vertically).")]
    private float downConstraint = 2;



    private readonly List<GameObject> objsInRange = new();
    private GameObject target;

    [Header("Lock-on")]
    [SerializeField, Range(0.1f, 15)]
    private int lockOnStrength = 10;
    [SerializeField, Tooltip("The layers that will block lock-on scans (if the player is behind an object of this layer, they can't be locked onto).")]
    private LayerMask blockingLayers;

    // Start is called before the first frame update
    void Start()
    {
        WeaponController wc = GetComponentInParent<WeaponController>();

        // Since the grenade launcher's barrel rotates in the opposite direction to the mounted gun's
        axisInversion = wc.CompareTag("MountedGun") ? -1 : 1;

        inputManager = GetComponentInParent<InputManager>();
        ac = GetComponentInParent<ActionController>();

        active = wc.Active;
    }

    // Update is called once per frame
    private void Update()
    {
        aiming = ac.IsAiming();
        RotateGun();
        //LockOn();

        // As soon as the aim button is pressed, get rid of the look at target since the ADS camera has a new target 
        // (This removes the glitch/jitteriness when entering ADS).
        vCam.LookAt = aiming ? lookPoint : null;
        vCam.Follow = aiming ? lookPoint : null;
    }

    // Using a custom clamp since the rotations wrap (doesn't go into minuses).
    /// <summary>
    /// https://discussions.unity.com/t/how-do-i-clamp-my-rotation/98687
    /// </summary>
    private float Clamp(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;

        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    private void RotateGun()
    {
        // Animate the guns rotating to a resting state.. then do nothing else
        if (!active) return;
           
        // If the player has locked onto a target, dampen the look input.
        Vector2 input = aiming ? (target ? inputManager.AimLookAction.ReadValue<Vector2>() * .15f : inputManager.AimLookAction.ReadValue<Vector2>()) : inputManager.LookAction.ReadValue<Vector2>();

        // If aiming... read the values for x and y... otherwise just get the x value (horizontal).
        rot = aiming ? new(input.y * .6f * axisInversion, input.x, 0) : new(input.y * .6f * axisInversion, 0, 0);

        Vector3 aimRot = new(Clamp((vertRotator.localEulerAngles + rot).x, -upConstraint, downConstraint), Clamp((cam.transform.localEulerAngles + rot).y, -rotConstraint_h, rotConstraint_h), 0);
        Vector3 lookRot = new(Clamp(vertRotator.localEulerAngles.x + rot.x, -upConstraint, downConstraint), Clamp(cam.transform.localEulerAngles.y, -rotConstraint_h, rotConstraint_h), 0);

        vertRotator.localEulerAngles = aiming ? 
            new(aimRot.x, 0, 0) : 
            new(lookRot.x, 0, 0);

        horiRotator.localEulerAngles = aiming ? 
            new(0, 0, aimRot.y) : 
            new(0, 0, lookRot.y);
    }

    //private void LockOn()
    //{
    //    if (!aiming) return;

    //    // If there isn't a target already, automatically select the first thing from the list
    //    if (objsInRange.Count > 0 && !target) target = objsInRange[0];

    //    bool insight;
    //    if (target) insight = !Physics.Linecast(muzzle.position, target.transform.position, blockingLayers);
    //    else insight = true;

    //    target = insight ? target : null;

    //    foreach (GameObject obj in objsInRange)
    //    {
    //        float distance1 = 0;
    //        float distance2 = 0;

    //        if (target) distance1 = Vector3.Distance(inputManager.transform.position, target.transform.position);
    //        if (obj) distance2 = Vector3.Distance(inputManager.transform.position, obj.transform.position);

    //        // Target the closest player
    //        if (distance2 < distance1) target = obj;
    //    }

    //    // Remove the target if it's no longer in range
    //    if (!objsInRange.Contains(target)) target = null;

    //    if (!target) return;

    //    Quaternion vertRot = Quaternion.LookRotation(target.transform.position - transform.position);
    //    Quaternion horiRot = Quaternion.LookRotation(target.transform.position - transform.position);

    //    vertRotator.rotation = Quaternion.Lerp(vertRotator.rotation, new(vertRot.x, 0, 0, 0), Time.deltaTime * lockOnStrength);
    //    //horiRotator.localRotation = Quaternion.Lerp(horiRotator.localRotation, new(0,0,horiRot.y*.5f, 0), Time.deltaTime * lockOnStrength);

    //    // Removes jittering (sets the look at point as the target whenever there is one)...
    //    vCam.LookAt = target ? target.transform : lookPoint;
    //}

    public void AddObjInRange(GameObject obj)
    {
        objsInRange.Add(obj);
    }

    public void RemoveObjInRange(GameObject obj)
    {
        objsInRange.Remove(obj);
    }

    public void ToggleActivation(bool active)
    {
        this.active = active;

        if (active) return;
        horiRotator.localEulerAngles = new();
        vertRotator.localEulerAngles = new();
    }
}
