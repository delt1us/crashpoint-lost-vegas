/**************************************************************************************************************
* Action Controller
* Contains all of the code that resopnds to player inputs. The responses are anything that isn't movement related (Primarily used for the combat).
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Daniel - Gave functionality to primary shooting, normal rockets, homing missle, "sonic boom", mines.
*   Dean -   Allowed these things to be initiated using Unity's input system.
*            Added aiming and changed how bullet projectiles worked (from spawning physical game objects to raycasts).
*            Removed the attacks that have been cut
*   Envy -   Changed the Secondary weapons system which now includes fire rate, reload time and ammo 
*   Dean -   The weapon controller is responsible for shooting all weapons. Using scriptable objects that determine the weapons stats
*   Dean -   Changed the Secondary Weapon action to swap weapons... Now you'll be able to aim secondary weapons.
*   Dean -   Made the defence weapons work in multiplayer
*   Armin - 02/08/23 - Added take damage debug button
***************************************************************************************************************/

using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : NetworkBehaviour
{   
    private InputManager inputManager;
    
    // The currently selected weapon...
    private GameObject activeWeapon;

    [Header("Primary Weapon")]
    [SerializeField, Tooltip("Insert the prefab of the weapon.")]
    private GameObject primaryWeapon;

    [Header("Secondary Weapon")]
    [SerializeField] private GameObject secondaryWeapon;

    [Header("Defence Weapon")]
    [SerializeField] private Transform defenceSpawn;
    [SerializeField] private GameObject defenceWeapon;

    // Defensive weapon cooldown
    private bool canDefenceShoot = true;
    private bool b_onDefTimer;
    private float defenceTimer;
    [SerializeField, Range(1, 30)] private float defenceCooldown = 5;

    [Space]
    [SerializeField, Tooltip("The transform for the cars look point (Not the ADS Camera).")]
    private Transform lookPoint;

    private bool aiming;
    private CinemachineVirtualCamera vCam;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();

        activeWeapon = primaryWeapon;
        primaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(true);

        if (!secondaryWeapon) return;

        secondaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(false);
    }

    private void Update()
    {
        // Change this to only have isowner check
        if (!IsOwner && (IsClient || IsServer)) return; 
        Aiming();
        DefensiveCooldown();
    }

    #region Aiming
    // Used whenever the player is using toggled aim.
    private void OnAim()
    {
        if (!inputManager.AimToggle) return;

        aiming = !aiming;
    }

    private void Aiming()
    {
        // Only read the aim input if toggle is off.
        if (!inputManager.AimToggle) aiming = inputManager.AimAction.ReadValue<float>() > 0;

        if (aiming) lookPoint.transform.rotation = new();

        // If there isn't an active weapon, don't do anything
        if (!(activeWeapon == primaryWeapon || activeWeapon == secondaryWeapon)) return;

        // As soon as the aim button is pressed, get rid of the look at target since the ADS camera has a new target 
        // (This removes the glitch/jitteriness when entering ADS).
        if (activeWeapon.GetComponentInChildren<CinemachineVirtualCamera>())
        {
            vCam.LookAt = aiming ? null : lookPoint;
            vCam.Follow = aiming ? null : lookPoint;

            // When aiming, the default camera has less priority than the ads camera...
            vCam.Priority = aiming ? 8 : 10;
        }

        // If the weapon has a reticle show it whenever it's aiming.
        if(activeWeapon.GetComponentInChildren<Image>()) activeWeapon.GetComponentInChildren<Image>().enabled = aiming;

        if (activeWeapon.GetComponent<GunAnimator>()) activeWeapon.GetComponent<GunAnimator>().SetAiming(aiming);
    }

    public bool IsAiming()
    {
        return aiming;
    }

    #endregion

    private void OnFlipCamera()
    {
        // Don't do anything if the player is aiming...
        if (aiming) return;

        // Flip the forward rotation of the look-at-point each time the input is pressed.
        lookPoint.transform.Rotate(0, 180, 0);
    }


    #region WeaponSwapping
    private void OnSwapWeapons()
    {
        // You can't swap weapons whilst aiming...
        if(aiming || !secondaryWeapon || !primaryWeapon) return;

        if (!IsServer) _SwapWeapons();
        _SwapWeaponsServerRpc();
    }

    [ServerRpc]
    private void _SwapWeaponsServerRpc()
    {
        _SwapWeapons();
    }

    private void _SwapWeapons()
    {
        // Toggles between the secondary and priamry weapon
        activeWeapon = activeWeapon == primaryWeapon? secondaryWeapon: primaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        primaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(activeWeapon == primaryWeapon);
        secondaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(activeWeapon == secondaryWeapon);
    }
    
    // Called by the secondary weapon whenever it runs out of ammo and the player tries to shoot it.
    // (It's the same as "OnSwapWeapons" except there's no checks)
    public void AmmoSwap()
    {
        // Toggles between the secondary and priamry weapon
        activeWeapon = activeWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        primaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(activeWeapon == primaryWeapon);
        secondaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(activeWeapon == secondaryWeapon);
    }

    private void OnToPrimary()
    {
        // Don't do anything if it's already the primary weapon
        if (activeWeapon == primaryWeapon) return;

        activeWeapon = primaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        primaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(true);
        secondaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(false);
    }

    private void OnToSecondary()
    {
        // Don't do anything if it's already the secondary weapon
        if (activeWeapon == secondaryWeapon) return;

        activeWeapon = secondaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        secondaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(true);
        primaryWeapon.GetComponent<WeaponController>().SetActiveWeapon(false);
    }
    #endregion


    private void OnDefensiveWeapon()
    {
        if (!defenceWeapon) return;

        if (!canDefenceShoot) return;

        canDefenceShoot = false;
        b_onDefTimer = true;

        // Play sound
        //DropMineSound.Play();

        if (!defenceWeapon) return;
        _SpawnDefenceServerRpc();
    }

    [ServerRpc]
    private void _SpawnDefenceServerRpc()
    {
        GameObject newDef = Instantiate(defenceWeapon, defenceSpawn.position, defenceSpawn.rotation);
        newDef.GetComponent<NetworkObject>().Spawn();
    }

    private void DefensiveCooldown()
    {
        if (!b_onDefTimer) return;

        defenceTimer += Time.deltaTime;
        if (defenceTimer > defenceCooldown)
        {
            defenceTimer = 0;
            canDefenceShoot = true;

            b_onDefTimer = false;
        }
    }

    public float GetDefenceCooldown()
    {
        return defenceCooldown;
    }

    public float GetDefenceTimer()
    {
        return defenceTimer;
    }

    public GameObject GetActiveWeapon()
    {
        return activeWeapon;
    }

    public void OnTakeDamageDebug()
    {
        // _healthManager.TakeDamage(400);
        // Debug.Log($"Took 40 damage\nHealth: {_healthManager.CurrentHealth}");
    }
}

