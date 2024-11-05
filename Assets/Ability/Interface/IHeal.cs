using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeal 
{
  public bool IsHealing { get; set; }

  public void TakeHeal(float healAmount);

  public void StartHealing(int HealPerSeconds);

  public void StopHealing();
}
