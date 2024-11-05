/**************************************************************************************************************
* Weapon Stats
* A scriptable object that stores the properties of the weapon (weapon type, damage, firerate, ammo...).
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Dean -  Stores damage, horizontal and vertical spread, critical hit multiplier.
*           Removed critical hit multiplier, added ammo properties and allows the weapon type to be selected from here.
*            
***************************************************************************************************************/

using UnityEngine;

[CreateAssetMenu(fileName = "NewGunStats", menuName = "Scriptable Objects/Weapons/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public string Name;

    [Header("Damage")]
    public int BaseDamage;
    [Tooltip("The force to apply to rigid bodies whenever one of its bullets hit.")] 
    public float ImpactForce = 100;
    [Tooltip("The time (in seconds) between each shot")]
    public float FireRate;


    [Header("Special Weapons")]
    [Tooltip("Only applies to the special weapon type.")] 
    public int SpecialDPS;
    [Tooltip("How long special damage effects last (like burning).")] 
    public float SpecialDuration = 3;
    [Tooltip("How many times a player can take damage per second whilst under the effect of the special weapon.")]
    public float TickRate = .2f;


    public enum AmmoType { Overheat, Ammo }
    [Header("Ammo")]
    public AmmoType ammoType;

    [Tooltip("This has a different purpose depending on how the weapon works...\n" +
        "Overheating- The seconds the player has to be shooting in order for the gun to overheat\n" +
        "Ammo - The actual magazine capacity.")] 
    public int MagCapacity;

    [Range(.5f, 20), Tooltip("A multiplier that controls how fast the player overheats.")] 
    public float OverheatRate = 2;

    [Header("Reloading")]
    [Tooltip("The time in seconds before the gun starts to cool down.")]
    [Range(.01f, 5)] public float ShortCooldownDelay = 2;

    [Range(.01f, 10), Tooltip("The time in seconds before the gun is allowed to cooldown once the gun has overheated")] 
    public float LongCooldownDelay = 6;

    [Range(.1f, 100), Tooltip("")] public float ReloadSpeed = 3;

    [Tooltip("The multiplier to apply to the short and long cooldowns whenever the weapon isn't equipped."), Range(.01f, 1)] 
    public float StowedDelayMultiplier = .8f;

    [Header("Accuracy")] 
    [Tooltip("How far the bullets are allowed to stray away from a direct/straight shot.\n|| Horizontally")]
    [Range(0, 2)] public float HorizontalSpread;

    [Tooltip("How far the bullets are allowed to stray away from a direct/straight shot.\n|| Vertically")]
    [Range(0, 2)] public float VerticalSpread;

    [Min(10), Tooltip("It's the distance that the bullet raycasts travel (in meters). The raycast won't do anything if this value is too small")] 
    public float Range;

    private void OnEnable()
    {
        Name = name + "_stats";
    }
}
