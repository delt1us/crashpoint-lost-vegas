using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SecondaryWeaponList : ScriptableObject
{
  public string Name;
  public string Description;
  public float MaxAmmo;
  public float reloadingTime;
  public float firingRate;

  public Sprite Visual; //UI Image
}

