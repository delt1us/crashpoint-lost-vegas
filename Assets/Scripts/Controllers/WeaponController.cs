/**************************************************************************************************************
* Weapon Controller
* Used to manage primary and secondary weapons. It's able to deactivate weapons and is responsible for dealing damage and creating sfx/vfx.
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Dean  - The primary weapons are controlled by this script - only one weapon can be active at once - this allows for ADSing
*   Dean  - The weapon controller works with secondary all secondary weapons (including special and projectile) 
*   Armin - 21/07/23 - Shooting works over multiplayer
*   Armin - 01/08/23 - Players cannot shoot before the countdown is over
*   Armin - 01/08/23 - Fixed not being able to shoot in singleplayer
*   Armin - 09/08/23 - Explosives work over multiplayer
*   Armin - 10/08/23 - Homing explosives work over multiplayer
*   Armin - 10/08/23 - Removed friendly fire
***************************************************************************************************************/

using Cinemachine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static WeaponStats;

public class WeaponController : NetworkBehaviour
{
    private GameObject playerOwner;

    public bool Active { get; private set; } = false;

    private NetworkVariable<bool> _isShooting = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private enum WeaponType { Bullet, Special, Projectile }
    [SerializeField] private WeaponStats stats;

    [Space]
    [SerializeField] private WeaponType weaponType;

    private AmmoType ammoType;

    private int magCapacity;
    private float currenetMag;

    // Special stats 
    private int specialDPS;
    private float specialDuration;
    private float tickRate;
    
    [Space]
    [SerializeField] private Transform adsPoint;

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

    private InputManager inputManager;

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


    private AudioManager playerAudio;
    private AudioSource[] shootSources;
    private AudioSource overheatSource;
    private AudioSource warningSource;

    private GunAnimator gunAnim;

    [Header("Projectile")]
    [SerializeField, Tooltip("If the weapon launches a projectile, insert its prefab (only necessary for Projectile weapon types.)")] 
    private GameObject projectile;


    private FlameThrower flame;

    [Space]
    [SerializeField] private PlayerHasControlBoolScriptableObject playerHasControlBoolScriptableObject;
    
    private void Start()
    {
        inputManager = GetComponentInParent<InputManager>();
        playerOwner = inputManager.gameObject;
        playerAudio = playerOwner.GetComponent<AudioManager>();

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
    }

    private void ConfigureGun()
    {
        // If the weapon is the flame thrower
        if (GetComponent<FlameThrower>()) flame = GetComponent<FlameThrower>();

        magCapacity = stats.MagCapacity;
        currenetMag = magCapacity;

        ammoType = stats.ammoType;
        shortCooldown = stats.ShortCooldownDelay;
        longCooldown = stats.LongCooldownDelay;

        if (flame)
        {
            specialDPS = stats.SpecialDPS;
            specialDuration = stats.SpecialDuration;
            tickRate = stats.TickRate;

            shootSources[0].clip = shootSfx[0];

            flame.ConfigureFlame(specialDPS, specialDuration, tickRate, playerOwner);
        }
        

        muzzleFlashVFX = GetComponentsInChildren<VisualEffect>();
        foreach (VisualEffect fx in muzzleFlashVFX) fx.Stop();

        if(GetComponent<GunAnimator>()) gunAnim = GetComponent<GunAnimator>();
    }

    private void Update()
    {
        Cooldown();

        DelayCooldown();
        DelayReload();

        if (!Active) return;
        if (!playerHasControlBoolScriptableObject.hasControl) return;

        FullAutoShoot();
        LoseAmmo();
    }

    private void FullAutoShoot()
    {
        if (inputManager) shooting = inputManager.PrimaryShootAction.ReadValue<float>() > 0;

        // Swap weapons if the secondary runs out of ammo.
        if (shooting && currenetMag <= 0 && ammoType == AmmoType.Ammo)
        {
            if (weaponType == WeaponType.Special) StopSpecialShoot();
            playerOwner.GetComponent<ActionController>().AmmoSwap();
        }

        // Don't read fire inputs if the gun has overheated
        if (overheated && shooting)
        {
            if (!warningSource.isPlaying && IsOwner) warningSource.Play();
            return;
        }
        if(warningSource.isPlaying && IsOwner && warningSource) warningSource.Stop();


        // If it is different, change the value
        if (shooting != _isShooting.Value && IsOwner) _isShooting.Value = shooting;

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
            // Reset the whether the player can shoot depending on the time between each spawn
            if (shootTimer > stats.FireRate) canShoot = true;
               
            shootTimer += Time.deltaTime;

            if (_isShooting.Value && canShoot)
            {
                if (weaponType == WeaponType.Bullet) Shoot();
                else ProjShoot();
                canShoot = false;
                shootTimer = 0;
            }

            // Rotates the gun's barrel
            if (gunAnim && shooting && !overheated) gunAnim.SetShooting(true);
            else if(gunAnim) gunAnim.SetShooting(false);
        }

        void specialShoot()
        {
            if (!flame) return;
            SpecialShoot(_isShooting.Value && canShoot);
        }

    }

    private void SpecialShoot(bool shouldShoot)
    {
        if (shouldShoot)
        {
            currenetMag -= Time.deltaTime * stats.OverheatRate;

            flame.GetFlameCollider().SetActive(true);
            FlameOnClientRpc();

            //if (!flame.Active) flame.ActivateFlameClientRpc();
            SpecialSFX();
        }

        else StopSpecialShoot();
    }

    [ClientRpc]
    private void FlameOnClientRpc() { flame.GetFlameVFX().Play(); }

    [ClientRpc]
    private void FlameOffClientRpc() { flame.GetFlameVFX().Stop(); }

    private void StopSpecialShoot()
    {
        if(!flame) return;

        flame.GetFlameCollider().SetActive(false);
        FlameOffClientRpc();
        flame.RemoveFlame();
        StopSpecialSFX();
    }

    
    private void LoseAmmo()
    {
        if (!(shooting && ammoType == AmmoType.Overheat)) return;
        currenetMag -= Time.deltaTime * stats.OverheatRate;

        if (currenetMag <= 0)
        {
            canShoot = false;
            shooting = false;
            _isShooting.Value = false;

            overheated = true;
            currenetMag = 0;

            // If the overheat sfx isn't playing and hasn't already played
            if(!overheatSource.isPlaying && cooldownTimer < 1) overheatSource.Play();

            b_onOverheatTimer = true;
            b_onCoolTimer = false;
        }
    }

    private void Cooldown()
    {
        // If the gun is inactive... lower the cooldown delay
        shortCooldown = Active ? stats.ShortCooldownDelay : stats.ShortCooldownDelay * stats.StowedDelayMultiplier;
        longCooldown = Active ? stats.LongCooldownDelay: stats.LongCooldownDelay * stats.StowedDelayMultiplier;

        if(!canReload || ammoType == AmmoType.Ammo) return;

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

    private void Shoot()
    {
        if ((ammoType == AmmoType.Ammo && currenetMag < 1) || overheated) return;

        b_onCoolTimer = true;
        b_onOverheatTimer = false;
        canReload = false;

        // Reset the cooldown timer after each shot
        cooldownTimer = 0;

        GameObject hitObj;

        // Get a random value within the range of the spread amounts
        float spreadX = Random.Range(-stats.VerticalSpread, stats.VerticalSpread);
        float spreadY = Random.Range(-stats.HorizontalSpread, stats.HorizontalSpread);

        playerAudio.AttackVoiceLine();

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

            // Raycasts is the bullet - it does the interacting. 
            if (!Physics.Raycast(muzzles[i].position, muzzles[i].forward, out projectileHit, stats.Range)) continue;


            hitObj = projectileHit.collider.gameObject;

            // Spawn the bullet impact vfx if the reference is valid and that raycast doesn't hit the owner
            Instantiate(ImpactVFX, projectileHit.point, new());

            // Adds a force to rigid bodies to simulate bullets hit it.
            ImpactForce(hitObj, projectileHit);

            // if the player shoots themselves, do nothing
            if (hitObj.GetComponentInParent<MovementController>() == playerOwner) continue;

            HealthManager health = hitObj.GetComponentInParent<HealthManager>();

            if (!IsServer || !health) continue;
            
            TeamMember teamMember = GetComponentInParent<Player>(); 

            // Take damage returns whether or not the hit player has died from the shot... if they have play the kill voiceline.
            if(health.TakeDamage(stats.BaseDamage, teamMember.teamId.Value)) playerAudio.KillVoiceLine();
            
            // TeamMember hitObjTeamMember = hitObj.GetComponentInParent<Player>() ? hitObj.GetComponentInParent<Player>() 
            //     : hitObj.GetComponentInParent<NetworkAI>();

            // If not on the same team 
            // if (hitObjTeamMember.teamId.Value != teamMember.teamId.Value)
            // {
                // Deals damage and returns whether the hit player is dead
                //if (health.TakeDamage(stats.BaseDamage) && topGunScript) topGunScript.RemoveObjInRange(hitObj);
            // }
        }
    }

    private void ProjShoot()
    {
        // Don't do anything if they have no ammo
        if (currenetMag < 1) return;

        currenetMag--;
        bool empty = currenetMag == 0 && shootSfx.Length > 1;

        foreach (VisualEffect fx in muzzleFlashVFX) fx.Play();
        
        // Projectile weapons will only have one audio source.
        shootSources[0].clip = shootSfx[empty? 1: 0];
        ShootSFX(0);

        if (IsServer)
        {
            GameObject newProj = Instantiate(projectile, muzzles[0].position, muzzles[0].rotation);
            newProj.GetComponent<NetworkObject>().Spawn();

            if(newProj.GetComponent<NadeScript>()) newProj.GetComponent<NadeScript>().Throw(GetComponentInParent<Rigidbody>().velocity, playerOwner);
            else if (newProj.GetComponent<Heat_Seeking_Missile_1>()) newProj.GetComponent<Heat_Seeking_Missile_1>().SetCaller(playerOwner);
        }
        canShoot = false;
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
        _isShooting.Value = false;

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

        if (gunAnim) gunAnim.SetShooting(false);
        StopSpecialShoot();
    }

    public void SetActiveWeapon(bool newActive)
    {
        Active = newActive;

        canShoot = Active && shootTimer == 0;

        if(cooldownTimer == 0) canReload = true;

        if (!Active) StopShoot();

        // Toggling the priority of the primary and secondary weapon...
        if (IsOwner)
        {
            if (topGunScript) topGunScript.ToggleActivation(Active);
            if (GetComponentInChildren<CinemachineVirtualCamera>()) GetComponentInChildren<CinemachineVirtualCamera>().Priority = Active ? 9 : 8;
        }
    }

    // Returns whether or not the player is able to pick up
    public bool RefillAmmo()
    {
        bool canPickup = currenetMag < stats.MagCapacity;

        if(!canPickup) return false;

        else
        {
            currenetMag = stats.MagCapacity;
            return true;
        }
    }


    /////////////////////////////////////////// SFX ///////////////////////////////////////////


    private AudioClip GetRandomSFX(AudioClip[] clips)
    {
        byte index = (byte)Random.Range(0, clips.Length);
        return clips[index];
    }

    private void ShootSFX(int i)
    {
        // Since the grenade launcher has a unique sfx for when it's the last shot
        if(weaponType != WeaponType.Projectile) shootSources[i].clip = GetRandomSFX(shootSfx);

        float rndPitch = Random.Range(minPitchDeviation, maxPitchDeviation);
        shootSources[i].pitch = rndPitch;
        
        shootSources[i].PlayOneShot(shootSources[i].clip);
    }

    private void SpecialSFX()
    {
        if (!shootSources[0].isPlaying) shootSources[0].Play();
    }

    private void StopSpecialSFX()
    {
        if (shootSources[0].isPlaying) shootSources[0].Stop();
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
        src.outputAudioMixerGroup = ogSource.outputAudioMixerGroup;

        return src;
    }



    public AmmoType GetAmmoType()
    {
        return ammoType;
    }



    /////////////////////////////////////////// DEBUGGING ///////////////////////////////////////////


    public int GetMaxAmmo()
    {
        return magCapacity;
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
