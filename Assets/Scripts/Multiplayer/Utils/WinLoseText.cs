//*************************************************************************************************************
/*  Win Lose Text
 *  A script to display whether you won or lost in the post game screen
 *  Was originally made to be a placeholder but it is what it is
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 05/08/23 - Added temporary post game screen
 *      Armin - 06/08/23 - Added support for hunted mode
 */
//*************************************************************************************************************

using TMPro;
using UnityEngine;

public class WinLoseText : MonoBehaviour
{
    [SerializeField] private WinningTeamScriptableObject winningTeamScriptableObject;
    [SerializeField] private TMP_Text tmpText;
    // Start is called before the first frame update
    void Start()
    {
        string text;
        if (winningTeamScriptableObject.winningTeam == 0) text = "Draw";
        else if (winningTeamScriptableObject.winningTeam == winningTeamScriptableObject.thisPlayersTeam) text = "You win";
        else text = "You lose";
        tmpText.text = text;
    }
}
