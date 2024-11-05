//*************************************************************************************************************
/*  End game controller
 *  A script to handle all the end game screen UI elements
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private WinningTeamScriptableObject winningTeamScriptableObject;
    [SerializeField] private TeamScoreScriptableObject teamOneScoreScriptableObject;
    [SerializeField] private TeamScoreScriptableObject teamTwoScoreScriptableObject;
    
    private Button _mainMenuButton;
    private Label _teamOneScore;
    private Label _teamTwoScore;
    private Label _victoryText;
    private Label _vsText;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _mainMenuButton = root.Q<Button>("exit-button");
        _mainMenuButton.clicked += _GoToMainMenu;
        
        _teamOneScore = root.Q<Label>("team-one-score");
        _teamOneScore.text = teamOneScoreScriptableObject.Score.ToString();
        
        _teamTwoScore = root.Q<Label>("team-two-score");
        _teamTwoScore.text = teamTwoScoreScriptableObject.Score.ToString();
        
        // Set correct text
        _victoryText = root.Q<Label>("victory-label");
        _victoryText.text = winningTeamScriptableObject.winningTeam == winningTeamScriptableObject.thisPlayersTeam ? 
            "victory" : "defeat";
        
        _vsText = root.Q<Label>("vs-label");

        // Set colours of relevant elements
        Color winnerTextColour = winningTeamScriptableObject.winningTeam == 1 ? 
            winningTeamScriptableObject.teamOneColour : winningTeamScriptableObject.teamTwoColour;
        _vsText.style.color = winnerTextColour;
        _victoryText.style.color = winnerTextColour;
    }

    private void OnDisable()
    {
        _mainMenuButton.clicked -= _GoToMainMenu;
    }

    private void _GoToMainMenu()
    {
        Destroy(GameObject.Find("Map GameObject"));
        SceneManager.LoadScene("NewMainMenu", LoadSceneMode.Single);
    }
}
