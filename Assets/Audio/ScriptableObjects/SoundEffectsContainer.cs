using UnityEngine;

[CreateAssetMenu(fileName = "NewSFX Container", menuName = "Scriptable Objects/Audio/Sound Effects Container")]
public class SoundEffectsContainer : ScriptableObject
{
    [Header("General Effects")]
    public AudioClip[] EngineSFX;
    public AudioClip IgnitionSFX;

    [Space]
    public AudioClip BoostSFX;

    [Header("Drift Boost SFX")]
    public AudioClip DriftBoostSFX;
    public AudioClip DriftingBoostSFX;

    [Space]
    public AudioClip[] ImpactSFX;

    [Header("Ground SFX")]
    public GroundSfxContainer[] GroundContainers;

    [Header("Movement Variant SFX (assignment not mandatory if variant doesn't apply))")]
    [Tooltip("Does not need to be assigned if it doesn't have the specified movement mechanic.")]
    public AudioClip DashSFX;

    [Tooltip("Does not need to be assigned if it doesn't have the specified movement mechanic.")]
    public AudioClip HoverSFX;

    [Tooltip("Does not need to be assigned if it doesn't have the specified movement mechanic.")]
    public AudioClip SirenSFX;

    [Tooltip("Does not need to be assigned if it doesn't have the specified movement mechanic.")]
    public AudioClip BoxThrowSFX;

    public AudioClip SpeedPackSFX;

    [Tooltip("Does not need to be assigned if it doesn't have the specified movement mechanic.")]
    public AudioClip GrappleSFX;
}
