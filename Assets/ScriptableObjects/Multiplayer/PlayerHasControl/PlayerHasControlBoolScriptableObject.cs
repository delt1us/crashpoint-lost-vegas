//*************************************************************************************************************
/*  Player has control boolean scriptable object
 *  A scriptable object to store whether or not the player has control of their car (client side)
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHasControlBoolScriptableObject", menuName = "Scriptable Objects/Multiplayer/Player has control boolean")]
public class PlayerHasControlBoolScriptableObject : ScriptableObject
{
    public bool hasControl { get; private set; }

    public delegate void ControlRemoved();
    public event ControlRemoved ControlRemovedEvent;

    public delegate void ControlReturned();
    public event ControlReturned ControlReturnedEvent;
    private void OnEnable()
    {
        hasControl = true;
    }

    public void RemoveControl()
    {
        if (!hasControl) return;
        hasControl = false;
        ControlRemovedEvent?.Invoke();
    }

    public void ReturnControl()
    {
        if (hasControl) return;
        hasControl = true;
        ControlReturnedEvent?.Invoke();
    }
}
