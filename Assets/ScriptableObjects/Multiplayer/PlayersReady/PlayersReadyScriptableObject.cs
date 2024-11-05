//*************************************************************************************************************
/*  Players ready scriptable object
 *  Used to store the number of players who are ready (loaded in and waiting for the countdown to finish)
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "PlayersReadyScriptableObject", menuName = "Scriptable Objects/Multiplayer/Players Ready")]
public class PlayersReadyScriptableObject : ScriptableObject
{
    // This is a scriptable object to keep track of how many players are ready to play
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;

    public delegate void AllPlayersReady();
    public event AllPlayersReady AllPlayersReadyEvent;
    
    private int _playersReady;
    public int PlayersReady
    {
        get { return _playersReady; }
        set
        {
            _playersReady = value;
            if (_playersReady == numberOfConnectionsScriptableObject.Connections)
            {
                Debug.Log("all players ready");
                AllPlayersReadyBool = true;
                AllPlayersReadyEvent?.Invoke();
            }
        }
    }

    public bool AllPlayersReadyBool { get; private set; }
    
    private void OnEnable()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        _playersReady = 0;
        AllPlayersReadyBool = false;
    }
}
