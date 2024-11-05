using UnityEngine;

// This is a scriptable object to keep track of if the game has started or not
// It is mainly used in the server side to close the lobby so that more people cannot join in the case of a force start
[CreateAssetMenu(fileName = "GameStartedBooleanScriptableObject", menuName = "Scriptable Objects/Multiplayer/Game Started Boolean")]
public class GameStartedBooleanScriptableObject : ScriptableObject
{
    public delegate void GameStarted();
    public event GameStarted GameStartedEvent;

    private bool _started;
    public bool Started
    {
        get { return _started; }
        set
        {
            _started = value;
            GameStartedEvent?.Invoke();            
        }
    }
    
    private void OnEnable()
    {
        _started = false;
    }
}
