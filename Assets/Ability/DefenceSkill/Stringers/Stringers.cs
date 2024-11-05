/**************************************************************************************************************
* Stringers
* For stringers physic and effect
*
* Created by Envy Cham 2023
* 
* Change Log:
*       Envy - 7/31 Decease duration time 8f --> 5f drag 2 --> 1.2
*              Added orignalDrag and NewDrag for restore value
*       Dean - Added an editable drag multiplier - the drag needs to be multiplied since all the cars have different drag values. 
*              Also added the duration of the effect to the script (instead of in the movement controller)
*            
***************************************************************************************************************/


using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Stringers: NetworkBehaviour
{
    [SerializeField, Range(0, 30)] private int lifeSpan = 10;
    [SerializeField, Range(1, 10)] private float dragMultiplier = 3;
    [SerializeField, Range(.1f, 10)] private float duration = 5;

  void Start()
  {
        if (!IsServer) return;

        StartCoroutine(LifeTimer());
        IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(lifeSpan);
            GetComponentInParent<NetworkObject>().Despawn();
        }
  }

    private void OnTriggerEnter(Collider other)
    {
        MovementController moveController = other.GetComponentInParent<MovementController>();
        if (!moveController) return;

        // If the drag hasn't already been increased... increase it
        if(moveController.GetComponent<Rigidbody>().drag == moveController.GetMoveStats().Drag) moveController.StartSlowDown(dragMultiplier, duration);
    }
}
