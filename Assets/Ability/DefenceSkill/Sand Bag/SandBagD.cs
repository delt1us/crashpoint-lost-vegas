/**************************************************************************************************************
* SandBagD
* For sand bags physic and effect
*
* Created by Envy Cham 2023
* 
* Change Log:
*       Envy - 8/1 Change sand effect from particle system to VFX 
*       Dean - The game object is destroyed after its lifespan elapses and instead of hooking the visual effect componenent in the inspector, its gotten in script.
*       Dean - Made it work with multiplayer
*
*            
***************************************************************************************************************/


using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class SandBagD : NetworkBehaviour
{
  //[SerializeField] private GameObject Dust;
  private VisualEffect sandBagVFX;
  [SerializeField] LayerMask layerMask;
    [SerializeField] private int lifeSpan = 10;

  private void Start()
  {
        // Remove the need to assign the effect (Dean)
        sandBagVFX = GetComponentInChildren<VisualEffect>();
        sandBagVFX.Stop();

  }

  void OnCollisionEnter(Collision other)
  {
      if ((layerMask & 1 << other.gameObject.layer) > 0)
    {
      sandBagVFX.Play();
            
        if (!IsServer) return;
        // Set a timer on the server to destroy all of the sandbags (from the main game object)
            StartCoroutine(LifeTimer());
            IEnumerator LifeTimer()
            {
                yield return new WaitForSeconds(lifeSpan);
                GetComponentInParent<NetworkObject>().Despawn();
            }
        }
  }

}
