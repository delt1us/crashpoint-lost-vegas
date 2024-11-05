//*************************************************************************************************************
/*  Number of connections scriptable object
 *  A scriptable object used to show how many connections there are, used in the loading screen
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "NumberOfConnectionsScriptableObject", menuName = "Scriptable Objects/Multiplayer/Number Of Connections")]
public class NumberOfConnectionsScriptableObject : ScriptableObject
{
    public delegate void NewConnection();

    public event NewConnection NewPlayerConnectedEvent;

    public int Connections { get; private set; }

    private void OnEnable()
    {
        ResetValues();
    }

    public void AddConnection()
    {
        Connections++;
        NewPlayerConnectedEvent?.Invoke();
    }

    // Called when loading screen starts
    public void ResetValues()
    {
        Debug.Log("reset values");
        Connections = 0;
    }
}

