using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBlock : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      if (other.GetComponent<PickupSkill>().HeldPickup == -1 && other.GetComponent<PickupSkill>().CanPickup)
      {
        other.GetComponent<PickupSkill>().StartPickup();
        Destroy(this.gameObject); 
      }
    }
  }
}
