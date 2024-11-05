/**************************************************************************************************************
* Skidmarker
* This is used to manage the interactions between the tire and the ground. It activates line renderers for each wheel to produce skidmarks as well as manages what tire sounds
* to play and when to play them.
*
* Created by Dean Atkinson-Walker 2023
*            
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class Skidmarker : NetworkBehaviour
{
    [SerializeField] private WheelCollider[] wheels;
    private Rigidbody carBody;

    // The tire spin the wheels have to endure in order to make skid marks.
    [SerializeField, Range(0, 100), Tooltip("How much force the wheel has to experience (going forward/backward) in order to skid. \n|| Lower number = more sensitive")] 
    private float skidThreshold_fwd = 40;

    [SerializeField, Range(0, 100), Tooltip("How much force the wheel has to experience (going sideward) in order to skid. \n|| Lower number = more sensitive")] 
    private float skidThreshold_side = 35;

    private bool drifting;
    private bool squeeling;
    private const float sfxSpeedThreshold = 5;

    private AudioManager audioManager;

    private TireTerrainManager ttManager;
    private string currentLayer;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();

        audioManager = GetComponent<AudioManager>();

        audioManager.CreateAudioSource("drift");
        audioManager.CreateAudioSource("tire");

        ttManager = new();
    }

    // Update is called once per frame
    private void Update()
    {

        foreach(WheelCollider wheel in wheels)
        {
            wheel.GetGroundHit(out WheelHit newHit);
            if(newHit.collider) TireGroundSound(newHit);

            // If skidding either sidewards or forwards ... and on the floor, make a skid mark.
            if ((SkiddingForward(newHit) || SkiddingSidewards(newHit)) && wheel.isGrounded)
            {
                if (wheel.gameObject.GetComponentInChildren<TrailRenderer>()) wheel.gameObject.GetComponentInChildren<TrailRenderer>().emitting = true;
            }

            // Otherwise stop
            else
            {
                if (wheel.gameObject.GetComponentInChildren<TrailRenderer>()) wheel.gameObject.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }

        if (squeeling) audioManager.PlayDrift();
        else if(audioManager) audioManager.StopDrift();
    }

    private bool SkiddingForward(WheelHit newHit)
    {
        drifting = Mathf.Abs(newHit.forwardSlip * 100) > skidThreshold_fwd;
        squeeling = drifting && carBody.velocity.magnitude > sfxSpeedThreshold;
        return drifting;
    }

    private bool SkiddingSidewards(WheelHit newHit)
    {
        drifting = Mathf.Abs(newHit.sidewaysSlip * 100) > skidThreshold_side;

        squeeling = drifting && carBody.velocity.magnitude > sfxSpeedThreshold;
        return drifting;
    }

    private void TireGroundSound(WheelHit hit)
    {
        if (hit.collider.GetComponent<Terrain>())
        {
            Terrain t = hit.collider.GetComponent<Terrain>();

            if (currentLayer != ttManager.GetLayerName(transform.position, t)) return;

            foreach (GroundSfxContainer container in audioManager.GetSfxContainer().GroundContainers)
            {
                if (currentLayer == container.name) audioManager.EditTerrainSFX(container);
            }
        }
    }

}



// https://www.youtube.com/watch?v=wXcjxeetg70&t=888s&ab_channel=NattyGameDev
public class TireTerrainManager
{
    private float[] FindTerrain(Vector3 playerPos, Terrain t)
    {
        Vector3 terrainPos = t.transform.position;
        TerrainData terrainData = t.terrainData;

        int mapX = (int)(playerPos.x - terrainPos.x) / terrainData.alphamapWidth;
        int mapZ = (int)(playerPos.z - terrainPos.z) / terrainData.alphamapHeight;

        float[,,] splatData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellMix = new float[splatData.GetUpperBound(2) + 1];
        for (int i = 0; i < cellMix.Length; i++)
        {
            cellMix[i] = splatData[0, 0, i];
        }

        return cellMix;
    }

    public string GetLayerName(Vector3 playerPos, Terrain t)
    {
        float[] cellMix = FindTerrain(playerPos, t);

        // The amount of the strongest blend
        float strongestBlend = 0;

        // The index of the strongest blend
        int maxBlendIndex = 0;

        for(int i = 0; i < cellMix.Length;i++)
        {
            if (cellMix[i] > strongestBlend)
            {
                maxBlendIndex = i;
                strongestBlend = cellMix[i];
            }
        }

        return t.terrainData.terrainLayers[maxBlendIndex].name;
    }
}