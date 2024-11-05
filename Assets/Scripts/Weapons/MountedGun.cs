/**************************************************************************************************************
* Mounted Gun
* A controller for the mounted gun. It reads inputs from the input manager to rotate the turret - the sole purpose of this is to rotate the gun (not shoot)
* Also has aim assist which can be adjusted by the lock-on strength.
*
* Created by Dean Atkinson-Walker 2023
*
* (UNUSED)
*
***************************************************************************************************************/

using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class MountedGun : MonoBehaviour
{
    // Also used as a link to get the transform of the actual car...
    private InputManager inputManager;

    [SerializeField] Camera cam;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform lookPoint;

    private bool aiming;
    private ActionController ac;

    private Vector3 rot;

    [Header("Rotation")]
    [SerializeField, Range(10, 45), Tooltip("The maximum angle the turret is allowed to rotate from the center (Horizontally).")]
    private float rotConstraint_h = 45;
    [SerializeField, Range(0, 5), Tooltip("The maximum angle the turret is allowed to rotate from the center (Vertically).")]
    private float upConstraint = 2;
    [SerializeField, Range(0, 10), Tooltip("The maximum angle the turret is allowed to rotate from the center (Vertically).")]
    private float downConstraint = 2;

    private readonly List<GameObject> objsInRange = new();
    private GameObject target;

    [Header("Lock-on")]
    [SerializeField, Range(0.1f, 15)] 
    private int lockOnStrength = 10;
    [SerializeField,Tooltip("The layers that will block lock-on scans (if the player is behind an object of this layer, they can't be locked onto).")] 
    private LayerMask blockingLayers;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponentInParent<InputManager>();
        ac = GetComponentInParent<ActionController>();
    }

    // Update is called once per frame
    private void Update()
    {
        aiming = ac.IsAiming();
        RotateGun();
        LockOn();

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
        // If the player has locked onto a target, dampen the look input.
        Vector2 input = aiming ? (target? inputManager.AimLookAction.ReadValue<Vector2>() * .15f: inputManager.AimLookAction.ReadValue<Vector2>()) : inputManager.LookAction.ReadValue<Vector2>();

        // If aiming... read the values for x and y... otherwise just get the x value (horizontal).
        rot = aiming ? new(input.y * -.6f, input.x, 0) : new(input.y * -.6f, 0, 0);

        Vector3 aimRot = new(Clamp((transform.localEulerAngles + rot).x, -upConstraint, downConstraint), Clamp((transform.localEulerAngles + rot).y, -rotConstraint_h, rotConstraint_h), 0);
        Vector3 lookRot = new(Clamp(transform.localEulerAngles.x + rot.x, -upConstraint, downConstraint), Clamp(cam.transform.localEulerAngles.y, -rotConstraint_h, rotConstraint_h), 0);

        transform.localEulerAngles = aiming ? aimRot : lookRot;
    }

    private void LockOn()
    {
        if (!aiming) return;

        // If there isn't a target already, automatically select the first thing from the list
        if (objsInRange.Count > 0 && !target) target = objsInRange[0];

        bool insight;
        if(target) insight = !Physics.Linecast(muzzle.position, target.transform.position, blockingLayers);
        else insight = true;

        target = insight ? target: null;

        foreach (GameObject obj in objsInRange)
        {
            float distance1 = 0;
            float distance2 = 0;

            if(target) distance1 = Vector3.Distance(inputManager.transform.position, target.transform.position);
            if(obj) distance2 = Vector3.Distance(inputManager.transform.position, obj.transform.position);

            // Target the closest player
            if (distance2 < distance1) target = obj;
        }

        // Remove the target if it's no longer in range
        if (!objsInRange.Contains(target)) target = null;

        if(!target) return;

        Quaternion rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lockOnStrength);

        // Removes jittering (sets the look at point as the target whenever there is one)...
        vCam.LookAt = target? target.transform: lookPoint;
    }

    public void AddObjInRange(GameObject obj)
    {
        objsInRange.Add(obj);
    }

    public void RemoveObjInRange(GameObject obj)
    {
        objsInRange.Remove(obj);
    }
}

