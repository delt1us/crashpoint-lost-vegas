//*************************************************************************************************************
/*  Health scriptable object
 *  A scriptable object to hold data about the current players health:
 *      Stores the explosions prefab
 *      Stores configurable numbers about launching the player when they die
 *      Stores respawn timer default value
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Dean - 11/08/23 - Added damage field
 *      Armin - 24/07/23 - Health works
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "HealthScriptableObject", menuName = "Scriptable Objects/Health")]
public class HealthScriptableObject : ScriptableObject
{
    public string objName;
    public int maxHealth = 100;
    public int Health { get; private set; }
    public int RespawnTimeSeconds;
    
    public delegate void HealthChanged();
    public event HealthChanged HealthChangedEvent;

    public bool isDead = false;
    public delegate void Died();
    public event Died DiedEvent;

    public GameObject explosionPrefab;
    public float blastRadius;
    public int damage; // I added damage on explosions for the explosive barrels (Dean)
    public int explosionForce = 1000; // I added damage on explosions for the explosive barrels (Dean)
    public float launchMagnitudeMin;
    public float launchMagnitudeMax;
    public float launchRangeRadius;
    
    private void OnEnable()
    {
        Health = maxHealth;
        objName = "health_" + name;
    }

    public void SetHealth(int newHealth)
    {
        Health = newHealth;
        HealthChangedEvent?.Invoke();
    }

    public void SetDead(bool deadBool = true)
    {
        isDead = deadBool;
        if (isDead) DiedEvent?.Invoke();
    }
}
