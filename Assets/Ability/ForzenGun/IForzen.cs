using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IForzen 
{
  public bool IsForzen { get; set; }

  public void StartForzen(int DamagePerSecond);

  
  public void StopForzen();
}
