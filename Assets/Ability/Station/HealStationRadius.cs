using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStationRadius : MonoBehaviour
{
  private PlayerHealth playerHealth;
  private float healCooldown;

  public delegate void PlayerEnteredEvent(Player player);
  public delegate void PlayerExitedEvent(Player player);

  public PlayerEnteredEvent OnPlayerEnter;
  public PlayerExitedEvent OnPlayerExit;


  private List<Player> PlayersInRadius = new List<Player>();

  private void OnTriggerEnter(Collider other)
  {
    if (other.TryGetComponent<Player>(out Player player))
    {
      PlayersInRadius.Add(player);
      OnPlayerEnter?.Invoke(player);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.TryGetComponent<Player>(out Player player))
    {
      PlayersInRadius.Remove(player);
      OnPlayerExit?.Invoke(player);
    }
  }

  private void OnDisable()
  {
    foreach (Player player in PlayersInRadius)
    {
      OnPlayerExit?.Invoke(player);
    }
    PlayersInRadius.Clear();
  }

}
