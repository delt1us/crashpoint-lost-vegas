/**************************************************************************************************************
* Impact Spawner
* Selects a random VFX from the array of possible vfx then destroys itself after a certain amount of time.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ImpactSpawner : MonoBehaviour
{
    [SerializeField] private VisualEffectAsset[] impactFX;

    private void Start()
    {
        int rndNum = Random.Range(0, impactFX.Length);

        VisualEffect vfxPlayer = GetComponent<VisualEffect>();
        vfxPlayer.visualEffectAsset = impactFX[rndNum];
        vfxPlayer.Play();

        // Despawn the explosion after 5 seconds of it spawning
        StartCoroutine(despawn());

        IEnumerator despawn()
        {
            yield return new WaitForSeconds(5);
            Destroy(transform.parent.gameObject);
        }

    }

}
