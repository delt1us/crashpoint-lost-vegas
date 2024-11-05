//*************************************************************************************************************
/*  Player ID handler scriptable object
 *  Used to store the value for the next player's ID
 *  No longer used since it was unnecessary to begin with
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIDHandlerScriptableObject", menuName = "Scriptable Objects/Multiplayer/Player ID Handler")]
public class PlayerIDHandlerScriptableObject : ScriptableObject
{
    public int nextPlayerId;

    private void OnEnable()
    {
        nextPlayerId = 1;
    }
}