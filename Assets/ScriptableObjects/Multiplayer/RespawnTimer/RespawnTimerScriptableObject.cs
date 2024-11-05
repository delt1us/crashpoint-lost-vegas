//*************************************************************************************************************
/*  Respawn timer scriptable object
 *  A scriptable object used to show how many seconds are left until respawn (client side)
 *  Changed in the Player script
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "RespawnTimerScriptableObject", menuName = "Scriptable Objects/Multiplayer/Respawn timer")]
public class RespawnTimerScriptableObject : ScriptableObject
{
    public int startingTime;
    private int _currentTime;

    public int CurrentTime
    {
        get { return _currentTime; }
        set
        {
            _currentTime = value;
            TimerChangedEvent?.Invoke();
            if (_currentTime == -1) TimerFinishedEvent?.Invoke();
        }
    }
    
    public delegate void TimerStarted();
    public event TimerStarted TimerStartedEvent;
    
    public delegate void TimerChanged();
    public event TimerChanged TimerChangedEvent;

    public delegate void TimerFinished();
    public event TimerFinished TimerFinishedEvent;

    public void StartTimer()
    {
        TimerStartedEvent?.Invoke();
    }
}
