/**************************************************************************************************************
* FlameThrowerAttackRadius
* For flame thrower attack area
*
* Created by Envy Cham 2023
* 
* Change Log:
*       Dean - Made it work with multiplayer by simplifying everything
*              The stats (like damage per second and tick rate) are set from the FlameThrower script which gets its values from its weapon controller
*
*            
***************************************************************************************************************/


using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlameThrowerAttackRadius : NetworkBehaviour
{
  //public delegate void EnemyEnteredEvent(HealthManager Players);
  //public delegate void EnemyExitedEvent(HealthManager Players);

  //public EnemyEnteredEvent OnEnemyEnter;
  //public EnemyExitedEvent OnEnemyExit;

  //public HealthManager Player;
  private int dmgPerSec;

    // The burn time whilst the player is no longer in the flame
  private float dmgDuration;

    // How many times does the player take damage per second
  private float tickRate;

  private readonly List<HealthManager> enemiesInRadius = new();

    private int callerTeamId;

    public void ConfigureFlame(int dps, float duration, float tickRate, GameObject owner)
    {
        dmgPerSec = dps;
        dmgDuration = duration;
        this.tickRate = tickRate;

        callerTeamId = owner.GetComponentInParent<Player>().teamId.Value;
    }

    private void OnTriggerEnter(Collider other)
  {
        HealthManager player = other.GetComponentInParent<HealthManager>();

        if (!player) return;
        enemiesInRadius.Add(player);

        // since some cars have several colliders - they take additional damage (dividing by how many colliders there are so that it doesnt do additional damage)
        player.StartBurning(dmgPerSec / enemiesInRadius.Count, tickRate, callerTeamId);

  }

  private void OnTriggerExit(Collider other)
  {
        HealthManager carHealth = other.GetComponentInParent<HealthManager>();
        if (!carHealth) return;

        carHealth.StopBurning(dmgDuration);
        enemiesInRadius.Remove(carHealth);
    }


    public void RemoveFlame()
    {
        // Go through all the players that are in the range and initiate the end of the burn.
        foreach (HealthManager player in enemiesInRadius) player.StopBurning(dmgDuration);

        // After the loop remove all things from the list
        enemiesInRadius.Clear();
    }
}
