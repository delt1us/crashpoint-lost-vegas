//*************************************************************************************************************
/*  AI array scriptable obejct
 *  A scriptable object that holds an array of AI prefabs. Allows the server to choose a random one to spawn
 *  Also holds the networkAIPrefab which is used to spawn a holder for the ai cars
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "AIArrayScriptableObject", menuName = "Scriptable Objects/Multiplayer/AI Array")]
public class AIArrayScriptableObject : ScriptableObject
{
    public GameObject networkAiPrefab;
    public GameObject[] aiPrefabsArray;
}