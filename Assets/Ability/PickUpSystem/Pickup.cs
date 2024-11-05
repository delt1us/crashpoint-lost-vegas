using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Pickup : ScriptableObject
{
  public string Name;
  public string Description;
  public int Uses;

  public PickupBoostFunction[] Boost;

  [Space]

  public PickupHealFunction[] Heal; 

  public Sprite Visual; //UI Image
}
