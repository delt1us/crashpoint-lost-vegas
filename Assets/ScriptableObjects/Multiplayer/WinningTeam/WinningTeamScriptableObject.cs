//*************************************************************************************************************
/*  Winning team scriptable object
 *  A scriptable object used to store which team won the previous game
 *  It is used to move data between scenes
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "WinningTeamScriptableObject", menuName = "Scriptable Objects/Multiplayer/Winning team")]
public class WinningTeamScriptableObject : ScriptableObject
{
    public int winningTeam;
    public int thisPlayersTeam;

    public Color teamOneColour;
    public Color teamTwoColour;
    private void OnEnable()
    {
        winningTeam = 0;
        thisPlayersTeam = 0;
    }
}
