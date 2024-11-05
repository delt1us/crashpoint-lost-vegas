//*************************************************************************************************************
/*  Blast Radius
 *  A script used to spawn an explosion vfx, sfx and to deal damage to enemies in a radius
 *  Used by a lot of other scripts that cause explosions
 *  
 *  Created by Dean Atkinson-Walker 2023
 *  Change log:
 *          Dean - 23/06/23 - Created file
 *          Armin - 09/08/23 - Added multiplayer support
 *          Armin - 13/08/23 - Removed friendly fire
 */ 
//*************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class BlastRadius : NetworkBehaviour
{
    private SphereCollider core;
    private NetworkVariable<float> explosionRadius = new NetworkVariable<float>(0);

    // When the player is in one of the colliders this is the amount of damage they take
    // (if they are in the centre of the explosion the damage is multiplied by however many cores there are).
    private int baseDamage;

    private float explosionForce;

    [SerializeField] private VisualEffect[] explosionVFX;

    [SerializeField] private AudioClip[] explosionSFX;
    private AudioSource explosionSrc;
    private int _teamId;
    private int _callerId;

    // Start is called before the first frame update
    private void Start()
    {
        explosionSrc = GetComponentInChildren<AudioSource>();

        int rndNum = Random.Range(0, explosionSFX.Length);
        explosionSrc.clip = explosionSFX[rndNum];

        // Removing the vfx from the cores gameObject
        foreach(VisualEffect effect in explosionVFX) effect.transform.SetParent(null);
        explosionSrc.gameObject.transform.SetParent(null);
        
        ConfigureCores();
        if (IsServer) Disarm();
    }

    private void ConfigureCores()
    {
        transform.SetParent(null);
        core = GetComponent<SphereCollider>();
        core.radius = explosionRadius.Value;

        // Make the size of the vfx a bit bigger than the core.
        foreach (VisualEffect effect in explosionVFX)
        {
            effect.transform.localScale = new(core.radius, core.radius, core.radius);
            effect.Play();
        }
        explosionSrc.Play();
    }
    
    public void SetExplosion(int damage, float size, float explosionForce, int callerTeamId = 0, int callerId = 0)
    {
        baseDamage = damage;
        explosionRadius.Value = size;
        this.explosionForce = explosionForce;
        _teamId = callerTeamId;
        _callerId = callerId;
    }

    private void Disarm()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(0.025f);
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius.Value);
        }
        else if (other.GetComponentInParent<Rigidbody>())
        {
            other.GetComponentInParent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius.Value);
        }
        else if (other.GetComponentInChildren<Rigidbody>())
        {
            other.GetComponentInChildren<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius.Value);
        }
;


        HealthManager health = other.GetComponentInParent<HealthManager>();

        if (!health) return;

        Vector3 hitPosition = transform.position;
        Vector3 targetsPos = other.transform.position;

        // Getting the distance between the centre of the explosion and the target
        float distance = Vector3.Distance(targetsPos, hitPosition);

        // Getting a percentage of how far the target is from the centre of the explosion
        float rangePercent = Mathf.Abs((distance / explosionRadius.Value) - 1);
        float damage = rangePercent * baseDamage;

        // Used to play 
        List<TeamMember> players = GameMode.Instance.Players;
        AudioManager playerSound = null;
        foreach (TeamMember p in players) if (p.id.Value == _callerId) playerSound = p.GetComponentInChildren<AudioManager>();

        // Take damage..... if the player died from the explosion, play an audio cue from the causer of the explosion.
        if (health.TakeDamage((int)Mathf.Round(damage), _teamId, _callerId)) if(playerSound) playerSound.KillVoiceLine();
    }

    public override void OnDestroy()
    {
        if (explosionSrc) Destroy(explosionSrc.gameObject, explosionSrc.clip.length);
        foreach (VisualEffect effect in explosionVFX) Destroy(effect.gameObject, 10);
        base.OnDestroy();
    }
}
