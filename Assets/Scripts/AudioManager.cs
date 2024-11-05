/**************************************************************************************************************
* Audio Manager
* Manages the all the different types of audio the vehicles can play - it excludes weapon sounds. Used to play different sfx (called by various componenets).
* The file also contains the VoicelineManager which is responsible for playing the voicelines which are provided in the inputted voiceline container.
*
* Created by Dean Atkinson-Walker 2023
*            
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundEffectsContainer sfxContainer;
    private GroundSfxContainer currentGroundSFX;

    private const string engine = "ENGINE";
    private const string ignition = "IGNITION";
    private const string drift = "DRIFT";
    private const string boost = "BOOST";
    private const string driftBoost = "DRIFTBOOST";
    private const string driftingBoost = "DRIFTINGBOOST";
    private const string impact = "IMPACT";
    private const string speedPack = "SPEEDPACK";
    private const string tire = "TIRE";

    // Variants
    private const string dash = "DASH";
    private const string hover = "HOVER";
    private const string siren = "SIREN";
    private const string boxThrow = "BOXTHROW";
    private const string grapple = "GRAPPLE";

    private AudioSource engineSource;
    private AudioSource ignitionSource;
    private AudioSource driftSource;
    private AudioSource boostSource;
    private AudioSource driftBoostSource;
    private AudioSource impactSource;
    private AudioSource speedPackSource;
    private AudioSource tireSource;

    // Variants
    private AudioSource dashSource;
    private AudioSource hoverSource;
    private AudioSource sirenSource;
    private AudioSource boxThrowSource;
    private AudioSource grappleSource;

    // The sound that plays whilst the player is able to drift boost.
    private AudioSource driftingBoostSource;


    // Voicelines
    [Space]
    [SerializeField] private AudioMixerGroup dialogueGroup;
    [SerializeField] private VoiceLineContainer vlContainer;
    private VoicelineManager vlManager;
    protected AudioSource vlSource;
    private const string voice = "VOICE";

    private void Awake()
    {
        // Concrete is the default ground container
        currentGroundSFX = sfxContainer.GroundContainers[0];
    }

    private void Start()
    {
        //if(GetComponentInChildren<MusicManager>()) GetComponentInChildren<MusicManager>().PlaySong("game", true);
        CreateVLManager();
    }

    private void CreateVLManager()
    {
        vlSource = CreateAudioSource("voice");
        vlSource.spatialBlend = 1;
        vlSource.outputAudioMixerGroup = dialogueGroup;
        vlSource.minDistance = 6;
        vlSource.maxDistance = 11;

        vlManager = new(vlContainer, vlSource);
    }

    // An optional tag that can be given
    public AudioSource CreateAudioSource(string tag)
    {
        // Using the source that's already on the main game object as a reference to set its audio properties
        AudioSource ogSource = GetComponent<AudioSource>();

        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.dopplerLevel = ogSource.dopplerLevel;
        src.spatialBlend = ogSource.spatialBlend;
        src.spread = ogSource.spread;
        src.minDistance = ogSource.minDistance;
        src.maxDistance = ogSource.maxDistance;
        src.outputAudioMixerGroup = ogSource.outputAudioMixerGroup;
        tag = tag.ToUpper();

        // Assigning the audio clips depending on the tag
        switch (tag)
        {
            case engine:
                engineSource = src;
                engineSource.clip = sfxContainer.EngineSFX[0];
                break;

            case ignition:
                ignitionSource = src;
                ignitionSource.clip = sfxContainer.IgnitionSFX;
                break;

            case drift:
                driftSource = src;
                driftSource.clip = currentGroundSFX.Skidding;
                break;

            case boost:
                boostSource = src;
                boostSource.clip = sfxContainer.BoostSFX;
                break;

            case driftBoost:
                driftBoostSource = src;
                driftBoostSource.clip = sfxContainer.DriftBoostSFX;
                break;

            case driftingBoost:
                driftingBoostSource = src;
                driftingBoostSource.clip = sfxContainer.DriftingBoostSFX;
                break;

            case impact:
                impactSource = src;
                impactSource.clip = GetRandomSFX(sfxContainer.ImpactSFX);
                break;

            case speedPack:
                speedPackSource = src;
                speedPackSource.clip = sfxContainer.SpeedPackSFX;
                break;

            case tire:
                tireSource = src;
                //tireSource.clip = sfxContainer.ConcreteTireSFX[0];
                break;


            // Variants
            case dash:
                dashSource = src;
                dashSource.clip = sfxContainer.DashSFX;
                break;

            case hover:
                hoverSource = src;
                hoverSource.clip = sfxContainer.HoverSFX;
                break;

            case siren:
                sirenSource = src;
                sirenSource.clip = sfxContainer.SirenSFX;
                break;

            case grapple:
                grappleSource = src;
                grappleSource.clip = sfxContainer.GrappleSFX;
                break;

            case voice:
                vlSource = src;
                break;
        }

        // If the new source has one of these tags, it should be looped.
        if (tag == engine || tag == drift || tag == boost || tag == driftingBoost || tag == siren || tag == tire || tag == hover) src.loop = true;

        return src;
    }


    private AudioClip GetRandomSFX(AudioClip[] clips)
    {
        byte index = (byte)Random.Range(0, clips.Length);
        return clips[index];
    }

    /////////// Play Audio
    public void PlayEngine()
    {
        if (engineSource.isPlaying) return;

        engineSource.Play();
    }


    public void PlayDrift()
    {
        if (driftSource.isPlaying) return;
        
        driftSource.Play();
    }

    public void PlayBoost()
    {
        if (boostSource.isPlaying) return;

        boostSource.Play();
    }

    public void PlayDriftingBoost()
    {
        if (driftingBoostSource.isPlaying) return;

        driftingBoostSource.Play();
    }

    public void PlayHover()
    {
        if (hoverSource.isPlaying) return;

        hoverSource.Play();
    }

    public void PlaySiren()
    {
        if (sirenSource.isPlaying) return;

        sirenSource.Play();
    }

    public void PlayTires()
    {
        if (tireSource.isPlaying) return;

        tireSource.Play();
    }

    public void PlayImpact()
    {
        // Since the volume will be set to 0 if it bounces with a low force... (won't be able to hear anything)
        if (impactSource.isPlaying) return;

        impactSource.clip = GetRandomSFX(sfxContainer.ImpactSFX);
        impactSource.Play();
    }

    public void PlayIgnition() { engineSource.Play(); }

    public void PlayDriftBoost() { driftBoostSource.Play(); }

    public void PlaySpeedPack() { speedPackSource.Play(); }

    public void PlayDash() { dashSource.Play(); }

    public void PlayBoxThrow() { boxThrowSource.Play(); }

    public void PlayGrapple() { grappleSource.Play(); }


    /////////// Voicelines
    public void AttackVoiceLine() { vlManager.PlayAttack(); }
    public void TakeDamageVoiceLine() { vlManager.PlayDamage(); }
    public void KillVoiceLine() { vlManager.PlayKill(); }



    /////////// Stop Audio
    public void StopEngine() { engineSource.Stop(); }

    public void StopDrift() { if(driftSource.isPlaying) driftSource.Stop(); }

    public void StopBoost() { if (boostSource.isPlaying) boostSource.Stop(); }

    public void StopDriftBoost() { driftBoostSource.Stop(); }

    public void StopDriftingBoost() { if(driftBoostSource) driftingBoostSource.Stop(); }

    public void StopHover() { if (hoverSource.isPlaying) hoverSource.Stop(); }

    public void StopSiren() { if (sirenSource.isPlaying) sirenSource.Stop(); }

    public void StopTires() {  tireSource.Stop(); }

    public void StopGrapple() { grappleSource.Stop(); }



    /////////// Edit Audio
    public void EditPitch(string tag, float newPitch)
    {
        tag = tag.ToUpper();

        switch (tag)
        {
            case engine:
                if (engineSource) engineSource.pitch = newPitch;
                break;

            case ignition:
                ignitionSource.pitch = newPitch;
                break;

            case drift:
                driftSource.pitch = newPitch;
                break;

            case boost:
                boostSource.pitch = newPitch;
                break;

            case driftBoost:
                driftBoostSource.pitch = newPitch;
                break;

            case driftingBoost:
                driftingBoostSource.pitch = newPitch;
                break;

            case impact:
                impactSource.pitch = newPitch;
                break;

            case speedPack:
                speedPackSource.pitch = newPitch;
                break;

            case tire:
                tireSource.pitch = newPitch; 
                break;


            // Variants
            case dash:
                dashSource.pitch = newPitch;
                break;

            case hover:
                hoverSource.pitch = newPitch;
                break;

            case siren:
                sirenSource.pitch = newPitch;
                break;

            case boxThrow:
                boxThrowSource.pitch = newPitch;
                break;

            case grapple:
                grappleSource.pitch = newPitch;
                break;
        }
    }

    public void EditVolume(string tag, float newVolume)
    {
        tag = tag.ToUpper();

        switch (tag)
        {
            case engine:
                engineSource.volume = newVolume;
                break;

            case ignition:
                ignitionSource.volume = newVolume;
                break;

            case drift:
                driftSource.volume = newVolume;
                break;

            case boost:
                boostSource.volume = newVolume;
                break;

            case driftBoost:
                driftBoostSource.volume = newVolume;
                break;

            case driftingBoost:
                driftingBoostSource.volume = newVolume;
                break;

            case impact:
                if(impactSource) impactSource.volume = newVolume;
                break;

            case speedPack:
                speedPackSource.volume = newVolume;
                break;

            case tire:
                tireSource.volume = newVolume;
                break;


            // Variants
            case dash:
                dashSource.volume = newVolume;
                break;

            case hover:
                hoverSource.volume = newVolume;
                break;

            case siren:
                sirenSource.volume = newVolume;
                break;

            case boxThrow:
                boxThrowSource.volume = newVolume;
                break;

            case grapple:
                grappleSource.volume = newVolume;
                break;
        }
    }

    public void EditClip(string tag, AudioClip clip)
    {
        tag = tag.ToUpper();

        switch (tag)
        {
            case engine:
                engineSource.clip = clip;
                break;

            case ignition:
                ignitionSource.clip = clip;
                break;

            case drift:
                driftSource.clip = clip;
                break;

            case boost:
                boostSource.clip = clip;
                break;

            case driftBoost:
                driftBoostSource.clip = clip;
                break;

            case driftingBoost:
                driftingBoostSource.clip = clip;
                break;

            case impact:
                impactSource.clip = clip;
                break;

            case speedPack:
                speedPackSource.clip = clip;
                break;

            case tire:
                tireSource.clip = clip;
                break;


            // Variants
            case dash:
                dashSource.clip = clip;
                break;

            case hover:
                hoverSource.clip = clip;
                break;

            case siren:
                sirenSource.clip = clip;
                break;

            case boxThrow:
                boxThrowSource.clip = clip;
                break;

            case grapple:
                grappleSource.clip = clip;
                break;
        }
    }


    public void EditTerrainSFX(GroundSfxContainer container)
    {
        currentGroundSFX = container;
    }

    public float GetEnginePitch()
    {
        if (!engineSource) return 0;
        return engineSource.pitch;
    }

    public SoundEffectsContainer GetSfxContainer()
    {
        return sfxContainer;
    }
}

public class VoicelineManager
{
    private readonly AudioSource vlSource;
    private readonly VoiceLineContainer vlContainer;

    // The chance of the voiceline playing
    private const int killChance = 6;
    private const int damageChance = 41;
    private const int attackChance = 41;

    public VoicelineManager(VoiceLineContainer _vlContainer, AudioSource src )
    {
        vlContainer = _vlContainer;
        vlSource = src;
    }

    public void PlayKill()
    {
        bool play = Random.Range(0, killChance) == 1;
        if (!play) return;

        vlSource.clip = vlContainer.EnemyKill;
        vlSource.Play();
    }

    public void PlayDamage()
    {
        bool play = Random.Range(0, damageChance) == 1;
        if (!play) return;

        vlSource.clip = vlContainer.ReceiveDamage;
        vlSource.Play();
    }

    public void PlayAttack()
    {
        bool play = Random.Range(0, attackChance) == 1;
        if (!play) return;

        int rndNum = Random.Range(0, vlContainer.Attacks.Length);
        vlSource.clip = vlContainer.Attacks[rndNum];
        vlSource.Play();
    }
}