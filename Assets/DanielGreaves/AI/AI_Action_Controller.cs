/**************************************************************************************************************
* AI Action Controller
* Contains all of the code that resopnds to player inputs. The responses are anything that isn't movement related (Primarily used for the combat).
*
* Created by Daniel Greaves 2023
* 
* Change Log:
*   Daniel - Gave functionality to primary shooting, normal rockets, homing missle, "sonic boom", mines.
*   Dean -   Allowed these things to be initiated using Unity's input system.
*            Added aiming and changed how bullet projectiles worked (from spawning physical game objects to raycasts).
*            Removed the attacks that have been cut
*   Envy -   Changed the Secondary weapons system which now includes fire rate, reload time and ammo 
*   Dean -   Changed the Secondary Weapon action to swap weapons... Now you'll be able to aim secondary weapons.
*   
*   Daniel  Modified class to work with AI and currently working on this
*            
***************************************************************************************************************/

using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AI_Action_Controller : MonoBehaviour
{
   // private InputManager inputManager;
    private Rigidbody carBody;

    [SerializeField, Tooltip("Insert the prefab of the weapon.")]
    private GameObject primaryWeapon;

    // The currently selected weapon...
    private GameObject activeWeapon;
    private GameObject activeProjectile;

    [Header("Secondary Weapon")]
    [SerializeField] private Transform secondarySpawn;
    //[SerializeField] private Transform secondarySpawn2;
    [SerializeField] private GameObject secondaryWeapon;
    private bool hasProjectile;

    //[SerializeField] private SecondaryWeaponList SecondaryList;
    //[SerializeField] private float curSecondary;

    //private float MissileRaycastRange = 15000;

    // Secondary weapon cooldown
    private bool canSecondaryShoot = true;
    private bool b_onSecTimer;
    private float secondaryTimer;
    [SerializeField, Range(.25f, 5), Tooltip("Only for projectile weapons (Grenades, Missles, etc...) (in seconds).")]
    private float secondaryCooldown = 1;

    [Header("Defence Weapon")]
    [SerializeField] private Transform defenceSpawn;
    [SerializeField] private GameObject defenceWeapon;

    // Defensive weapon cooldown
    private bool canDefenceShoot = true;
    private bool b_onDefTimer;
    private float defenceTimer;
    private float defenceCooldown = 5;

    [Space]
    [SerializeField, Tooltip("The transform for the cars look point (Not the ADS Camera).")]
    private Transform lookPoint;

    private bool aiming;
   


    private void Start()
    {
        //inputManager = GetComponent<InputManager>();
        carBody = GetComponent<Rigidbody>();
       // vCam = GetComponentInChildren<CinemachineVirtualCamera>();

        activeWeapon = primaryWeapon;
         primaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(true);

      

        if (secondaryWeapon)
        {
           
            if (secondaryWeapon.GetComponent<AI_Weapon_Controller>()) secondaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(false);
        }

        //if (curSecondary == -1)
        //{
        //    curSecondary = SecondaryList.MaxAmmo;
        //}
    }

    private void Update()
    {
        DefensiveCooldown();
        SecondaryCooldown();
    }

   
    

    #region WeaponSwapping
    private void OnSwapWeapons()
    {
        // You can't swap weapons whilst aiming...
       // if (aiming || !secondaryWeapon || !primaryWeapon) return;

        // Toggles between the secondary and priamry weapon
        activeWeapon = activeWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        primaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(activeWeapon == primaryWeapon);
        if (!hasProjectile) secondaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(activeWeapon == secondaryWeapon);
    }

    private void OnToPrimary()
    {
        // Don't do anything if it's already the primary weapon
        if (activeWeapon == primaryWeapon) return;

        activeWeapon = primaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        primaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(true);
        if (!hasProjectile) secondaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(false);
    }

    private void OnToSecondary()
    {
        // Don't do anything if it's already the secondary weapon
        if (activeWeapon == secondaryWeapon) return;

        activeWeapon = secondaryWeapon;

        // Setting whether or not the gun is active based on the value of the active weapon.
        secondaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(true);
        primaryWeapon.GetComponent<AI_Weapon_Controller>().SetActiveWeapon(false);
    }

    #endregion

    private void SecondaryShoot()
    {
        //Instantiate(secondaryWeapon, secondarySpawn.position, secondarySpawn.rotation);
        //Instantiate(secondaryWeapon, secondarySpawn2.position, secondarySpawn2.rotation);

        //curSecondary--;
        // if (secondary.GetComponent<Missile_1>()) secondary.GetComponent<Missile_1>().SetCaller(gameObject);

        //else if (secondary.GetComponent<Heat_Seeking_Missile_1>()) secondary.GetComponent<Heat_Seeking_Missile_1>().SetCaller(gameObject);

        //else if (secondary.GetComponent<NadeScript>()) secondary.GetComponent<NadeScript>().Throw(carBody.velocity);
    }
    //private IEnumerator reloadSecondary()
    //{
    //  canSecondaryShoot = false;
    //  isReloading = true;

    //  yield return new WaitForSeconds(SecondaryList.reloadingTime);

    //  curSecondary = SecondaryList.MaxAmmo;
    //  canSecondaryShoot = true;
    //  isReloading = false;

    //}

    private void SecondaryCooldown()
    {
        if (!b_onSecTimer) return;

        secondaryTimer += Time.deltaTime;
        if (secondaryTimer > secondaryCooldown)
        {
            secondaryTimer = 0;
            canSecondaryShoot = true;

            b_onSecTimer = false;
        }
    }

    private void OnDefensiveWeapon()
    {
        if (!defenceWeapon) return;

        if (!canDefenceShoot) return;

        canDefenceShoot = false;
        b_onDefTimer = true;

        // Play sound
        //DropMineSound.Play();

        // Spawn mine
        if (!defenceWeapon) return;

        Instantiate(defenceWeapon, defenceSpawn);
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

    public GameObject GetActiveWeapon()
    {
        return activeWeapon;
    }

    
}
