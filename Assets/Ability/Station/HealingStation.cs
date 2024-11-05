using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingStation : MonoBehaviour
{
  [SerializeField] private HealStationRadius HealRadius;
  [SerializeField] private int HealingDPS = 5;
  [SerializeField] private float HealDuration = 3f;

  private void Awake()
  {
    HealRadius.OnPlayerEnter += StartHealingPlayer;
    HealRadius.OnPlayerExit += StopHealingPlayer;

    HealRadius.gameObject.SetActive(true);
  }

  private void StartHealingPlayer(Player player)
  {
    if (player.TryGetComponent<IHeal>(out IHeal heal))
    {
      heal.StartHealing(HealingDPS);

    
    }
  }

  private IEnumerator DelayedDisableHeal(Player player, float Duration)
  {
   
    yield return new WaitForSeconds(Duration);
    
    if (player.TryGetComponent<IHeal>(out IHeal heal))
    {
      heal.StopHealing();
    }
  }

  private void StopHealingPlayer(Player player)
  {

    
      StartCoroutine(DelayedDisableHeal(player, HealDuration));
     
    
  }

}
