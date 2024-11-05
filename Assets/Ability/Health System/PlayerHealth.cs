using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHeal
{
  [SerializeField] private GameObject Player;

  [SerializeField] private bool _IsHealing;
  public float maxHealth;
  public float currentHealth;

  private Coroutine HealCoroutine;
  public float playerHealth { get => currentHealth; set => currentHealth = value; }
  public bool IsHealing { get => _IsHealing; set => _IsHealing = value; }
  

  void Start()
  {
    currentHealth = 100;
    
  }

  public void TakeDamage(float damageAmount)
  {
    currentHealth -= damageAmount;

    if (currentHealth <= 0)
    {
      Debug.Log("You are dead");

    }
  }
  public void TakeHeal(float healAmount)
  {
    currentHealth += healAmount;

    if (currentHealth > maxHealth)
    {
      currentHealth = maxHealth;
    }
  }

  public void StartHealing(int HealPerSecond)
  {
    IsHealing = true;
    if ( HealCoroutine!= null)
    {
      StopCoroutine(HealCoroutine);

    }

    HealCoroutine = StartCoroutine(Heal(HealPerSecond));
  }

  private IEnumerator Heal(int HealPerSecond)
  {
    float minTimeToHeal = 1f / HealPerSecond;
    WaitForSeconds wait = new WaitForSeconds(minTimeToHeal);
    int healPerSecond = Mathf.FloorToInt(minTimeToHeal) + 10;

    TakeHeal(healPerSecond);
    while (IsHealing)
    {
      yield return wait;
      TakeHeal(healPerSecond);
    }
  }

  public void StopHealing()
  {
    IsHealing = false;
    if (HealCoroutine != null)
    {
      StopCoroutine(HealCoroutine);
    }
  }


}



