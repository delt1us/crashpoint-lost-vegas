//*************************************************************************************************************
/*  Connected boolean scriptable object
 *  A scriptable object used to show the player is connected
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

// Decouples code
[CreateAssetMenu(fileName = "ConnectedBooleanScriptableObject", menuName = "Scriptable Objects/Multiplayer/Connected Boolean")]
public class ConnectedBooleanScriptableObject : ScriptableObject
{
    private bool _connected;
    public bool Connected
    {
        get { return _connected; }
        set
        {
            _connected = value;
            ConnectionEstablishedEvent?.Invoke();
        }
    }

    public delegate void ConnectionEstablished();
    public event ConnectionEstablished ConnectionEstablishedEvent;
    
    
    private void OnEnable()
    {
        _connected = false;
    }
}