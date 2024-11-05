//*************************************************************************************************************
/*  Player authenticated scriptable object
 *  Used to determine if the player is already authenticated in the matchmaking script
 *  Needed because the matchmaking script is destroyed and remade at the end of a game
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAuthenticatedScriptableObject", menuName = "Scriptable Objects/Multiplayer/Player Authenticated")]
public class PlayerAuthenticatedScriptableObject : ScriptableObject
{
    public bool authenticated;

    private void OnEnable()
    {
        authenticated = false;
    }
}
