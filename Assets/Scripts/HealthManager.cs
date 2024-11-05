/**************************************************************************************************************
* Health manager
* Used to manage the player's health. Includes functions to read, take away and give health.
*
* Created by Dean Atkinson-Walker 2023
* 
* Change Log:
*   Dean -  Setup basic functions like takeDamage and Death
*   Dean - Changed the health values from shorts to ints
*   Envy - Add FlameThrower damage system
*   Dean - Simplified how the FlameThrower works
*   Armin - 24/07/23 - Health works multiplayer
*   Armin - 02/08/23 - Implemented dying and death event
*   Armin - 07/08/23 - Fixed infinitely dying
*   Armin - 08/08/23 - Added support for explosive damage for multiplayer
***************************************************************************************************************/

using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] private HealthScriptableObject healthScriptableObject;

    [Space]

    [SerializeField] Healthbar healthbar; //c

    [SerializeField] GameObject killedPopUp; //c
    public KillCounter killCounter; //c

    public DeathMenu deathMenu; //c

    public NetworkVariable<int> currentHealthNetworkVariable = new NetworkVariable<int>();
    private int _currentHealth;

    public delegate void Death();
    public event Death DeathEvent;

    // Voicelines
    private AudioManager audioManager;

    //FlameThrower
    private bool isBurning;

  public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if(healthbar) healthbar.SetHealth(currentHealthNetworkVariable.Value); //c 
        }
    }
    public bool Dead { get; private set; }


    private void Start()
    {
        audioManager = GetComponent<AudioManager>();

        if (healthbar && healthScriptableObject) healthbar.SetMaxHealth(healthScriptableObject.maxHealth); //c
        if (killedPopUp) killedPopUp.SetActive(false); //c
        killCounter = FindObjectOfType<KillCounter>();//c
        deathMenu = FindObjectOfType<DeathMenu>(); //c

        // Set health to max health
        CurrentHealth = healthScriptableObject ? healthScriptableObject.maxHealth : 100;

        currentHealthNetworkVariable.OnValueChanged += _UpdateScriptableObjectHealth;
        if (!IsServer) return;
        // Set health to max health in scriptable object if it is referenced. If not then set it to 100
        currentHealthNetworkVariable.Value = CurrentHealth;
    }

    private void OnEnable()
    {
        if (IsServer || IsClient) currentHealthNetworkVariable.OnValueChanged += _UpdateScriptableObjectHealth;
    }

    private void OnDisable()
    {
        if (IsServer || IsClient) currentHealthNetworkVariable.OnValueChanged -= _UpdateScriptableObjectHealth;
    }

    private void Update()
    {
        // Kill the player if they somehow go below the map or too high up
        if (transform.position.y < -1000 || transform.position.y > 2000) InstantDeath();
    }

    // Deals damage and returns whether or not the object is dead in case the caller needs to do management tasks with that information..
    public bool TakeDamage(int damage, int damageSourceTeamId = 0, int damageSrcId = 0)
    {
        Player player = GetComponentInParent<Player>();

        TeamMember teamMember = player ?
            player : GetComponentInParent<NetworkAI>();

        // Since some none player objs have health managers
        if (teamMember)
        {
            if (teamMember.teamId.Value == damageSourceTeamId && teamMember.id.Value != damageSrcId)
            {
                print("tried to friendly fire, doing no damage");
                if (!Dead) return false;
            }
        }
        
            
        if (IsServer && currentHealthNetworkVariable.Value > 0)
        {
            currentHealthNetworkVariable.Value -= damage;
            audioManager.TakeDamageVoiceLine();

            // Dying is checked server side
            if (currentHealthNetworkVariable.Value <= 0)
            {
                currentHealthNetworkVariable.Value = 0;
                Die();
            }
        }
        
        return Dead;
    }

    public void InstantDeath()
    {
        TakeDamage(currentHealthNetworkVariable.Value);
    }

    // These ints are because the OnValueChanged event is actually an action and so needs the methods that subscribe
    // to it to take 2 ints as parameters.
    private void _UpdateScriptableObjectHealth(int oldValue, int newValue)
    {
        // Update scriptable object for debug UI
        if (IsOwner && healthScriptableObject)
        {
            healthScriptableObject.SetHealth(newValue);
        }

        CurrentHealth = newValue;
    }

    [ServerRpc]
    public void _GainHealthServerRpc(int health)
    {
        print("REGENED");

        // If player were to overcap health, set it to max health
        if (currentHealthNetworkVariable.Value + health > healthScriptableObject.maxHealth) CurrentHealth = healthScriptableObject.maxHealth;

        else currentHealthNetworkVariable.Value += health;
        healthbar.SetHealth(currentHealthNetworkVariable.Value); //c
    }

    public bool GainHealth(int health)
    {
        print($"previous health = {currentHealthNetworkVariable.Value}");
        print($"Is gaining health on the server = {IsServer}");
        if (!IsServer) return false;

        bool canPickup = currentHealthNetworkVariable.Value < healthScriptableObject.maxHealth;

        if (!canPickup) return false;

        // Able to pickup
        else
        {
            // If player were to overcap health, set it to max health
            if (currentHealthNetworkVariable.Value + health > healthScriptableObject.maxHealth) currentHealthNetworkVariable.Value = healthScriptableObject.maxHealth;

            else currentHealthNetworkVariable.Value += health;
            healthbar.SetHealth(currentHealthNetworkVariable.Value); //c
            return true;
        }
    }

    //c
    // make this its own script 

    public void KillEnemy(GameObject target)
    {
        // If killed pop up is not given in the serialize field in the editor
        if (!killedPopUp) return;

        // Get the KilledPopUp component from the holder gameobject
        KilledPopUp KilledPopUp = killedPopUp.GetComponent<KilledPopUp>();
        if (KilledPopUp != null)
        {
            KilledPopUp.SetText(target.name);
        }
        if (killCounter != null)
        {
            killCounter.IncreaseKillCount();
        }
    }
    
    private void Die()
    {
        _CreateExplosionOnPlayer();
        DieClientRpc();
        
        DeathEvent?.Invoke();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        // If this object is a target then set it to inactive
        if (CompareTag("Target")) gameObject.SetActive(false);
        
        // Dead boolean is also returned after Die() in TakeDamage()
        Dead = true;

        // Stops all of the player's audio sources
        AudioSource[] srcs = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource src in srcs) if(src) src.Stop(); 

        _LaunchPlayerInRandomDirection();
    }

    private void _LaunchPlayerInRandomDirection()
    {
        // If not owner and isnt singleplayer
        if (!IsOwner && (IsServer || IsClient)) return;
            
        float magnitude = Random.Range(healthScriptableObject.launchMagnitudeMin, healthScriptableObject.launchMagnitudeMax);
        float radius = healthScriptableObject.launchRangeRadius;
        // Get random angle between 0 and 359
        int randAngleDegrees = Random.Range(0, 360);
        float randAngleRadians = Mathf.Deg2Rad * randAngleDegrees;
        
        // Get x and z components of it
        float opposite = radius * Mathf.Sin(randAngleRadians);
        float adjacent = radius * Mathf.Cos(randAngleRadians);
        
        // Whether opposite or adjacent is x or z doesn't matter as it is random anyway
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(magnitude * rigidbody.mass * new Vector3(opposite, magnitude, adjacent), ForceMode.Impulse);
        
        // This is just here because I wanted it to spin, feel free to change
        rigidbody.AddTorque(magnitude * rigidbody.mass * new Vector3(opposite, magnitude, adjacent), ForceMode.Impulse);
    }

    private void _CreateExplosionOnPlayer()
    {
        //int teamId = GetComponentInParent<Player>() ? GetComponentInParent<Player>().teamId.Value : 
        //    GetComponentInParent<NetworkAI>().teamId.Value;
        
        GameObject explosion = Instantiate(healthScriptableObject.explosionPrefab, transform.position,
            transform.rotation, transform);
        explosion.GetComponent<BlastRadius>().SetExplosion(
            healthScriptableObject.damage, healthScriptableObject.blastRadius, healthScriptableObject.explosionForce);
        explosion.GetComponent<NetworkObject>().Spawn();
    }

    //////////////////////////////////////////// FireDamage ////////////////////////////////////////////

    public void StartBurning(int DamagePerSecond, float tickRate, int callerTeamId)
    {
        isBurning = true;
    
        StartCoroutine(Burn(DamagePerSecond, tickRate, true, callerTeamId));
    }

    private IEnumerator Burn(int DamagePerSecond, float tickRate, bool active = true, int callerTeamId = 0)
    {
        // do nothing if not active ... (this is done since the function is called regardless if the player is burning or not) (Dean)
        if (!active) yield return new();
        if (IsServer) TakeDamage(DamagePerSecond);

        while (isBurning)
        {
            yield return new WaitForSeconds(IsHost ? tickRate * 2 : tickRate);
            if (IsServer) TakeDamage(DamagePerSecond, callerTeamId);
        }
    }

    public void StopBurning(float burnTime)
    {
        // Since the health manager becomes a null reference if someone dies whlist burning
        if (!this) return;
        
        StartCoroutine(StopBurn(burnTime));
    }

    private IEnumerator StopBurn(float burnTime)
    {
        yield return new WaitForSeconds(burnTime);

        isBurning = false;

        // This turns off the burn effect
        StartCoroutine(Burn(0, 0, false));
    }

}
