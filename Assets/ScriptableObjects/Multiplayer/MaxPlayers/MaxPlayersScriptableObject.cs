//*************************************************************************************************************
/*  Max players scriptable object
 *  Used in some scripts to determine how many players are allowed 
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "MaxPlayersScriptableObject", menuName = "Scriptable Objects/Multiplayer/Max Players")]
public class MaxPlayersScriptableObject : ScriptableObject
{
    public int maxPlayers;
    public int maxAi;
}