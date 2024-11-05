//*************************************************************************************************************
/*  Team score scriptable object
 *  A scriptable object used to keep track of the team's score (client side)
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "TeamScoreScriptableObject", menuName = "Scriptable Objects/Multiplayer/Team Score")]
public class TeamScoreScriptableObject : ScriptableObject
{
    public int maxScore;
    private int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            ScoreUpdatedEvent?.Invoke();
        }
    }
    public delegate void ScoreUpdated();
    public event ScoreUpdated ScoreUpdatedEvent;
    private void OnEnable()
    {
        _score = 0;
    }
}
