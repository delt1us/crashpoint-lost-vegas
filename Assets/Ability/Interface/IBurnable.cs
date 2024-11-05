/**************************************************************************************************************
* IBurnable
* 
* Interface for burn effect
*
* Created by Envy Cham 2023
* 
* Change Log:
*
*
*            
***************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IBurnable
{
  public bool IsBurning { get; set; }

    [ServerRpc]
  public void StartBurningServerRpc(int DamagePerSecond);

    [ServerRpc]
  public void StopBurningServerRpc();
}
