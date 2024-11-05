//*************************************************************************************************************
/*  King of the Hill Manager
 *  A subclass to handle KotH specific functions:
 *      Moving the capture point
 *      Removing dead players from capture point
 *      Adding score for killing players
 *      Checking if game is over
 *      Updating team score
 *  
 *  Created by Armin Raad 2023
 *  Change log (I lost the version history for this file when I refactored the game mode system):
 *      Armin - unknown date - Made ControlPointManager
 *      Armin - unknown date - Lot of fixes
 *      Armin - unknown date - Added support for score values scriptable object
 *      Armin - 11/08/23 - Added capture point moving around feature
 */
//*************************************************************************************************************

using System.Collections;
using UnityEngine;

public class KingOfTheHillManager : GameMode
{
    private GameObject _capturePointHolder;
    private CapturePoint _capturePoint;

    protected override void OnEnable()
    {
        base.OnEnable();
        teamOneScoreScriptableObject.maxScore = scoreValuesScriptableObject.maxScoreKotH;
        teamTwoScoreScriptableObject.maxScore = scoreValuesScriptableObject.maxScoreKotH;
    }

    private void Start()
    {
        _capturePointHolder = GameObject.FindWithTag("CapturePoint");

        // Capture points need to start enabled for IsServer to work properly
        for (int i = 0; i < _capturePointHolder.transform.childCount; i++)
        {
            _capturePointHolder.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        StartCoroutine(CycleCapturePointCoroutine());
        
        IEnumerator CycleCapturePointCoroutine()
        {
            int i = 0;
            while (!GameTimer.finished)
            {
                _capturePoint = _capturePointHolder.transform.GetChild(i).GetComponent<CapturePoint>();
                _capturePoint.gameObject.SetActive(true);
                
                i++;
                if (i == _capturePointHolder.transform.childCount) i = 0;

                yield return new WaitForSeconds(timePerGamemodeCycleScriptableObject.secondsPerCycle);
                _capturePoint.gameObject.SetActive(false);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (IsServer) _UpdateTeamScores();
    }
    
    public override void KillPlayer(TeamMember teamMember)
    {
        _capturePoint.RemovePlayerFromAreaList(teamMember);
        if (teamMember.teamId.Value == 1)
        {
            teamTwoScore.Value += scoreValuesScriptableObject.scoreForKillingKotH;
        }
        else
        {
            teamOneScore.Value += scoreValuesScriptableObject.scoreForKillingKotH;
        }
        _CheckIfGameOver();
    }
    
    protected override void _CheckIfGameOver()
    {
        base._CheckIfGameOver();
        if (teamOneScore.Value >= scoreValuesScriptableObject.maxScoreKotH) _TeamWins(1);        
        if (teamTwoScore.Value >= scoreValuesScriptableObject.maxScoreKotH) _TeamWins(2);        
    }
    
    private void _UpdateTeamScores()
    {
        if (_capturePoint.ownedByTeamId.Value == 0) return;
        
        int scoreToAdd = _capturePoint.beingContested.Value ? 
            scoreValuesScriptableObject.scorePerSecondKotHContested : scoreValuesScriptableObject.scorePerSecondKotH;
        
        if (_capturePoint.ownedByTeamId.Value == 1)
        {
            teamOneScore.Value += scoreToAdd * Time.deltaTime;
            _CheckIfGameOver();
        }
        else if (_capturePoint.ownedByTeamId.Value == 2)
        {
            teamTwoScore.Value += scoreToAdd * Time.deltaTime;
            _CheckIfGameOver();
        }
    }
    
    protected override void _UpdateDebugInfo()
    {
        return;
        base._UpdateDebugInfo();
        debugInfoText.text += $"\n" +
                              $"Owned by team: {_capturePoint.ownedByTeamId.Value}\n" +
                              $"Being captured by: {_capturePoint.beingCapturedByTeamId.Value}\n" +
                              $"Being contested: {_capturePoint.beingContested.Value}\n" +
                              $"Capture progress: {(int)(_capturePoint.captureProgress.Value / CapturePoint.CaptureTime * 100)}%";
    }
}