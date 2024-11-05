/**************************************************************************************************************
* AI_Weapon_Controller based on Weapon controler created by Dean
* 
* Used to manage primary and secondary weapons. It's able to deactivate weapons and is responsible for dealing damage and creating sfx/vfx.
*
* Created by Dean Atkinson-Walker 2023
* 
* Edited by Daniel Greaves to work with AI 2023
*
* Change Log:
*     Armin - 11/08/23 - Added multiplayer support
*     Armin - 13/03/23 - Removed friendly fire
*            
***************************************************************************************************************/


//using Cinemachine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static WeaponStats;
using UnityEngine.Animations;

public class AI_Weapon_Controller : NetworkBehaviour
{
    public bool Active { get; private set; } = false;
    private NetworkVariable<bool> _isShooting = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private enum WeaponType { Bullet, Special, Projectile }
    [SerializeField] private WeaponStats stats;

    [Space]
    [SerializeField] private WeaponType weaponType;

    private AmmoType ammoType;

    public int MagCapacity { get; private set; }
    private float currenetMag;

    // Special stats 
    public short SpecialDPS { get; private set; }
    public float SpecialDuration { get; private set; }

    [Space]
    [SerializeField] private Transform adsPoint;

    private MountedGun mountedGunScript;
    private TopGun topGunScript;

    // Using an array since some weapons will have several points of fire that shoot simultaneously...
    [SerializeField, Tooltip("Using an array since some weapons will have several points of fire that shoot simultaneously...\n " +
        "Input the transform of where to begin the RayTrace (Presumably where the muzzle is). \n" +
        "The direction you want the bullet to travel in should be pointing on the Z-axis (The blue arrow).")]
    private Transform[] muzzles;

    [SerializeField, Tooltip("Raycasts (bullets) will only be registered if the hit object has this layer.")]
    private LayerMask shootables;

    [Header("VFX")]
    [SerializeField] private GameObject bulletVFX;
    [SerializeField] private GameObject ImpactVFX;
    [SerializeField] private VisualEffect[] muzzleFlashVFX;

    private readonly List<Quaternion> defaultRots = new();

    private RaycastHit projectileHit;

    //private InputManager inputManager;

    private float shootTimer;
    private bool shooting;
    private bool canShoot = true;

    // Overheating
    private bool overheated = false;
    private bool b_onOverheatTimer;
    private bool b_onCoolTimer;
    private float cooldownTimer;
    private bool canReload;

    private float shortCooldown;
    private float longCooldown;



    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] shootSfx;
    [SerializeField] private AudioClip overheatSfx;
    [SerializeField] private AudioClip overheatWarnSfx;

    private const float minPitchDeviation = .8f;
    private const float maxPitchDeviation = 1.2f;

    private AudioSource[] shootSources;
    private AudioSource overheatSource;
    private AudioSource warningSource;

    private GunAnimator gunAnim;

    [Header("Projectile")]
    [SerializeField, Tooltip("If the weapon launches a projectile, insert its prefab (only necessary for Projectile weapon types.)")]
    private GameObject projectile;

    //public DamageText DamagePrefab;//c
    //public string NumberFormat = "N0";//c
    //private ObjectPool DamageTextPool;//c

    [SerializeField] public GameObject WeaponRaycast;
    private float WeaponRaycastRange = 90;

    public NetworkVariable<bool> aiHasControl = new NetworkVariable<bool>(false);
    
    private void Start()
    {
        // inputManager = GetComponentInParent<InputManager>();
        //  GetComponentInParent<PlayerInput>();

        // Getting all the audio sources of the actual guns
        shootSources = GetComponentsInChildren<AudioSource>();

        overheatSource = CreateSFX();
        overheatSource.clip = overheatSfx;

        warningSource = CreateSFX();
        warningSource.clip = overheatWarnSfx;
        warningSource.volume = .4f;

        ConfigureGun();

        if (GetComponentInChildren<TopGun>()) topGunScript = GetComponentInChildren<TopGun>();

        if (weaponType != WeaponType.Bullet) return;

        foreach (Transform muzzle in muzzles) defaultRots.Add(muzzle.localRotation);

        if (GetComponentInChildren<MountedGun>()) mountedGunScript = GetComponentInChildren<MountedGun>();
    }

    ////c
    //private void Awake()
    //{
    //    DamageTextPool = ObjectPool.CreateInstance(DamagePrefab, 50); 
    //}

    private void ConfigureGun()
    {
        MagCapacity = stats.MagCapacity;
        currenetMag = MagCapacity;

        ammoType = stats.ammoType;
        shortCooldown = stats.ShortCooldownDelay;
        longCooldown = stats.LongCooldownDelay;

        muzzleFlashVFX = GetComponentsInChildren<VisualEffect>();
        foreach (VisualEffect fx in muzzleFlashVFX) fx.Stop();

        if (GetComponent<GunAnimator>()) gunAnim = GetComponent<GunAnimator>();
    }

    private void Update()
    {
        Cooldown();
        DelayCooldown();
        DelayReload();

        if (!Active) return;
        if ((IsServer || IsClient) && !aiHasControl.Value) return;

        FullAutoShoot();
        LoseAmmo();

        // Rest is done on server only
        if (!IsServer) return;
        _DetermineIfAiShouldShoot();
    }

    private void _DetermineIfAiShouldShoot()
    {
        // Create raycast to detect other players and then fire on detection
        Vector3 AIDetection = Vector3.forward;
        Ray LineTrace = new Ray(WeaponRaycast.transform.position, WeaponRaycast.transform.TransformDirection(AIDetection * WeaponRaycastRange));

        // Draw raycast line
        Debug.DrawRay(WeaponRaycast.transform.position, WeaponRaycast.transform.TransformDirection(AIDetection * WeaponRaycastRange));

        // Raycast collision
        if (Physics.Raycast(LineTrace, out RaycastHit WallHit, WeaponRaycastRange))
        {
            bool healthManagerPresent = WallHit.collider.transform.GetComponentInParent<HealthManager>() ? true : false;
            // If raycast hits a player
            if (healthManagerPresent)
            {
                shooting = true;
            }
            else
            {
                shooting = false;
            }
        }
    }

    private void FullAutoShoot()
    {
        switch (weaponType)
        {
            case WeaponType.Bullet:
                shoot();
                break;

            case WeaponType.Special:
                specialShoot();
                break;

            case WeaponType.Projectile:
                shoot();
                break;
        }

        void shoot()
        {
            // If it is different then change the value
            if (shooting != _isShooting.Value && IsServer)
            {
                _isShooting.Value = shooting;
            }

            shootTimer += Time.deltaTime;

            // Reset the whether the player can shoot depending on the time between each spawn
            if (shootTimer > stats.FireRate)
            {
                canShoot = true;
                shootTimer = 0;
            }

            if (_isShooting.Value && canShoot)
            {
                if (weaponType == WeaponType.Bullet) PrimaryShoot();
                canShoot = false;
            }

            // Rotates the gun's barrel
            if (gunAnim && shooting && !overheated) gunAnim.SetShooting(true);
            else if (gunAnim) gunAnim.SetShooting(false);
        }

        void specialShoot()
        {
            SpecialShoot(shooting && canShoot);
        }

    }

    private void SpecialShoot(bool shouldShoot)
    {
        // Possible special weapon scripts
        FlameThrower flame;

        if (shouldShoot)
        {
            if (GetComponent<FlameThrower>() && Active)
            {
                flame = GetComponent<FlameThrower>();
               // if (!flame.Active) flame.Activate();
                SpecialSFX();
            }
        }
        else StopSpecialShoot();
    }

    public void StopSpecialShoot()
    {
        FlameThrower flame;

        if (GetComponent<FlameThrower>())
        {
            flame = GetComponent<FlameThrower>();
            //flame.Deactivate();
        }

        StopSpecialSFX();
    }


    private void LoseAmmo()
    {
        if (!(shooting && ammoType == AmmoType.Overheat)) return;
        currenetMag -= Time.deltaTime * stats.OverheatRate;

        if (currenetMag < 0)
        {
            canShoot = false;
            shooting = false;
            overheated = true;
            currenetMag = 0;

            if (!overheatSource.isPlaying) overheatSource.Play();

            b_onOverheatTimer = true;
            b_onCoolTimer = false;
        }
    }

    private void Cooldown()
    {
        // If the gun is inactive... lower the cooldown delay
        shortCooldown = Active ? stats.ShortCooldownDelay : stats.ShortCooldownDelay * stats.StowedDelayMultiplier;
        longCooldown = Active ? stats.LongCooldownDelay : stats.LongCooldownDelay * stats.StowedDelayMultiplier;

        if (!canReload || ammoType == AmmoType.Ammo) return;

        // If the gun is started to cool down, the gun isn't overheated...
        overheated = false;
        currenetMag += Time.deltaTime * stats.ReloadSpeed;

        if (currenetMag > stats.MagCapacity)
        {
            currenetMag = stats.MagCapacity;
            return;
        }
    }

    // Used when the player stops shooting but still has ammo in the mag
    private void DelayCooldown()
    {
        // Only run if the short cooldown timer is on and the long cooldown is off.
        if (!b_onCoolTimer || b_onOverheatTimer || ammoType == AmmoType.Ammo) return;

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > shortCooldown)
        {
            cooldownTimer = 0;
            canReload = true;

            b_onCoolTimer = false;
        }
    }

    // Used whenever the current mag reaches 0 (the long cooldown)
    private void DelayReload()
    {
        // Only run if the short cooldown timer is on and the long cooldown is off.
        if (!b_onOverheatTimer || b_onCoolTimer || ammoType == AmmoType.Ammo) return;

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > longCooldown)
        {
            cooldownTimer = 0;
            overheated = false;
            canReload = true;

            b_onOverheatTimer = false;
        }
    }

    private void PrimaryShoot()
    {
        print("primary shoot");
        if (ammoType == AmmoType.Ammo && currenetMag < 1) return; 

        b_onCoolTimer = true;
        b_onOverheatTimer = false;
        canReload = false;

        GameObject hitObj;

        // Get a random value within the range of the spread amounts
        float spreadX = Random.Range(-stats.VerticalSpread, stats.VerticalSpread);
        float spreadY = Random.Range(-stats.HorizontalSpread, stats.HorizontalSpread);

        for(int i  = 0; i < muzzles.Length; i++)
        {
            // Centering the rotation before each shot  
            muzzles[i].localRotation = defaultRots[i];
            muzzles[i].Rotate(transform.right, spreadX);
            muzzles[i].Rotate(transform.up, spreadY);

            muzzleFlashVFX[i].Stop();
            muzzleFlashVFX[i].Play();

            // Play for each muzzle
            ShootSFX(i);

            // Spawn a bullet effect that has no physics nor does interactions.
            if (bulletVFX) Instantiate(bulletVFX, muzzles[i].position, muzzles[i].rotation);
            if (ammoType == AmmoType.Ammo) currenetMag--;

            /// Have to nest since it interrupts the loop... 
            // Raycasts is the bullet - it does the interacting. 
            if (Physics.Raycast(muzzles[i].position, muzzles[i].forward, out projectileHit, stats.Range))
            {
                hitObj = projectileHit.collider.gameObject;

                // Spawn the bullet impact vfx if the reference is valid and that raycast doesn't hit the owner
                if (ImpactVFX && hitObj.GetComponentInParent<MovementController>() != gameObject) 
                    Instantiate(ImpactVFX, projectileHit.point, new());

                // Adds a force to rigid bodies to simulate bullets hit it.
                ImpactForce(hitObj, projectileHit);

                // if the player shoots themselves, do nothing
                if (hitObj.GetComponentInParent<MovementController>() == gameObject) continue; 

                HealthManager health;

                // For players, the health manager will always be in its parent.
                if (hitObj.GetComponentInParent<HealthManager>() && IsServer)
                {
                    health = hitObj.GetComponentInParent<HealthManager>();
                    int teamId = GetComponentInParent<NetworkAI>().teamId.Value;
                    
                    // Deals damage and returns whether the hit player is dead
                    if (health.TakeDamage(stats.BaseDamage, teamId) && topGunScript) topGunScript.RemoveObjInRange(hitObj);
                }
            }
        }
    }

    private void ImpactForce(GameObject obj, RaycastHit hit)
    {
        if (obj.GetComponent<Rigidbody>()) obj.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * stats.ImpactForce, hit.point);
        else if (obj.GetComponentInParent<Rigidbody>()) obj.GetComponentInParent<Rigidbody>().AddForceAtPosition(-hit.normal * stats.ImpactForce, hit.point);
    }
    
    private void StopShoot()
    {
        canShoot = false;
        shooting = false;

        if (currenetMag > 0 && currenetMag < stats.MagCapacity)
        {
            b_onCoolTimer = true;
            b_onOverheatTimer = false;
        }

        else if (currenetMag <= 0)
        {
            b_onOverheatTimer = true;
            b_onCoolTimer = false;
        }

        StopSpecialShoot();
    }

    public void SetActiveWeapon(bool newActive)
    {
        Active = newActive;
        canShoot = Active;


        if (cooldownTimer == 0) canReload = true;

        if (!Active) StopShoot();

        // Toggling the priority of the primary and secondary weapon...
        if (IsOwner)
        {
            if (topGunScript) topGunScript.ToggleActivation(Active);
            //if (GetComponentInChildren<CinemachineVirtualCamera>()) GetComponentInChildren<CinemachineVirtualCamera>().Priority = Active ? 9 : 8;
        }

        else if (!IsServer || !IsClient)
        {
            if (topGunScript) topGunScript.ToggleActivation(Active);
           // if (GetComponentInChildren<CinemachineVirtualCamera>()) GetComponentInChildren<CinemachineVirtualCamera>().Priority = Active ? 9 : 8;
        }
    }

    public void ReceiveAmmo(float ammo)
    {
        currenetMag += ammo;
    }


    /////////////////////////////////////////// SFX ///////////////////////////////////////////


    private AudioClip GetRandomSFX(AudioClip[] clips)
    {
        byte index = (byte)Random.Range(0, clips.Length);
        return clips[index];
    }

    private void ShootSFX(int i)
    {
        shootSources[i].clip = GetRandomSFX(shootSfx);

        float rndPitch = Random.Range(minPitchDeviation, maxPitchDeviation);
        shootSources[i].pitch = rndPitch;

        shootSources[i].Play();
    }

    private void SpecialSFX()
    {
        //if(!shootSource.isPlaying) shootSource.Play();
    }

    private void StopSpecialSFX()
    {
        //if(shootSources.Length > 0) foreach(AudioSource src in shootSources) src.Stop();
    }

    private AudioSource CreateSFX()
    {
        // Uses the audio source in the Unity inspector as a base to make other audio components (it takes its essential settings).
        AudioSource ogSource = GetComponent<AudioSource>();

        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.dopplerLevel = ogSource.dopplerLevel;
        src.spatialBlend = ogSource.spatialBlend;
        src.spread = ogSource.spread;
        src.minDistance = ogSource.minDistance;
        src.maxDistance = ogSource.maxDistance;

        return src;
    }

    /////////////////////////////////////////// DEBUGGING ///////////////////////////////////////////


    public int GetMaxAmmo()
    {
        return MagCapacity;
    }

    public float GetCurrentMag()
    {
        return currenetMag;
    }

    public float GetOverheatRate()
    {
        return stats.OverheatRate;
    }

    public float GetCooldown()
    {
        return cooldownTimer;
    }

    public float GetShortDelay()
    {
        return shortCooldown;
    }

    public float GetLongDelay()
    {
        return longCooldown;
    }
}

