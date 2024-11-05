//*************************************************************************************************************
/*  Hunted Manager
 *  A subclass to handle Hunted specific functions:
 *      Assigning the hunted players
 *      Giving points to the players who outlive their bounty
 *      Adding score for killing players
 *      Checking if game is over
 *  
 *  Created by Armin Raad 2023
 *  Change log (I lost the version history for this file when I refactored the game mode system):
 *      Armin - unknown date - Made hunted manager
 *      Armin - unknown date - Added support for score values scriptable object
 *      Armin - 11/08/23 - Added bounty expiration feature
 */
//*************************************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HuntedManager : GameMode
{
    private TeamMember _teamOneHunted;
    private TeamMember _teamTwoHunted;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        playersReadyScriptableObject.AllPlayersReadyEvent += _AssignHuntedStart;
        teamOneScoreScriptableObject.maxScore = scoreValuesScriptableObject.maxScoreHunted;
        teamTwoScoreScriptableObject.maxScore = scoreValuesScriptableObject.maxScoreHunted;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playersReadyScriptableObject.AllPlayersReadyEvent -= _AssignHuntedStart;
    }

    protected override void Update()
    {
        base.Update();
        _HuntedCycleTimer(_teamOneHunted);        
    _HuntedCycleTimer(_teamTwoHunted);        
    }

    private void _HuntedCycleTimer(TeamMember hunted)
    {
        hunted.huntedTimer += Time.deltaTime;
        if (hunted.huntedTimer >= timePerGamemodeCycleScriptableObject.secondsPerCycle)
        {
            bool teamOne = hunted.teamId.Value == 1;
            bool teamTwo = hunted.teamId.Value == 2;
            if (teamOne)
            {
                teamOneScore.Value += scoreValuesScriptableObject.scoreForKillingHunted;
            }
            else if (teamTwo)
            {
                teamTwoScore.Value += scoreValuesScriptableObject.scoreForKillingHunted;
            }
            hunted.isHunted.Value = false;
            _AssignHuntedPlayers(teamOne, teamTwo);
        }
    }
    
    private void _AssignHuntedStart()
    {
        _AssignHuntedPlayers(true, true);
    }
    
    private void _AssignHuntedPlayers(bool teamOne, bool teamTwo)
    {
        // Assign random player on each team as hunted
        List<TeamMember> teamOnePlayers = new List<TeamMember>();
        List<TeamMember> teamTwoPlayers = new List<TeamMember>();
        // Sort players into teams
        foreach (var player in Players)
        {
            if (player.teamId.Value == 1) teamOnePlayers.Add(player);
            else teamTwoPlayers.Add(player);
        }
        // Choose random player in each team to be hunted
        if (teamOnePlayers.Count > 0 && teamOne) teamOnePlayers[Random.Range(0, teamOnePlayers.Count - 1)].isHunted.Value = true;
        if (teamTwoPlayers.Count > 0 && teamTwo) teamTwoPlayers[Random.Range(0, teamTwoPlayers.Count - 1)].isHunted.Value = true;
    }
    
    // Should only be called from the server
    public override void KillPlayer(TeamMember teamMember)
    {
        int scoreToAdd;
        if (teamMember.isHunted.Value) scoreToAdd = scoreValuesScriptableObject.scoreForKillingHunted;
        else scoreToAdd = scoreValuesScriptableObject.scoreForKillingNormal;

        if (teamMember.teamId.Value == 1)
        {
            teamTwoScore.Value += scoreToAdd;
        }
        else
        {
            teamOneScore.Value += scoreToAdd;
        }
        _CheckIfGameOver();
    }
    
    protected override void _CheckIfGameOver()
    {
        base._CheckIfGameOver();

        if (teamOneScore.Value >= scoreValuesScriptableObject.maxScoreHunted) _TeamWins(1);
        else if (teamTwoScore.Value >= scoreValuesScriptableObject.maxScoreHunted) _TeamWins(2);
    }
    
    protected override void _UpdateDebugInfo()
    {
        return;
        base._UpdateDebugInfo();
        debugInfoText.text += $"\n" +
                                 $"Hunted: {thisClientsPlayer.isHunted.Value}";
    }
}
