/**************************************************************************************************************
* Projectile Stats
* A scriptable object that stores the properties of the projectile weapon - these weapons are all explosive (damage, blast radius, fuse time...).
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

[CreateAssetMenu(fileName = "NewProjStats", menuName = "Scriptable Objects/Weapons/Projectile Stats")]
public class ProjectileStats : ScriptableObject
{
    public string Name;

    [Header("Damage"), Range(0, 2000)]
    public short Damage;
    [Range(1, 1000)] public float BlastRadius;
    [Range(1, 100000)] public float ExplosionForce = 3000;

    [Tooltip("The explosive will detonate after this time has elapsed...\n" +
        "If this is set to 0, the projectile will blow up on impact."), Range(0, 20)] 
    public float FuseTime;

    [Header("Movement")]
    [Tooltip("If the projectile is lobbed, this is the force it experiences whenever it spawns.\n" +
        "If the projectile moves in a relatively straight line, this is the force that's continuously applied to it.")]
    public float LaunchForce;
    public float GravityMultiplier;

    [Tooltip("When this time elapses, the object will be destroyed.")] 
    public float LifeSpan = 10;

    private void OnEnable()
    {
        Name = name;
    }
}
