//*************************************************************************************************************
/*  Timer per gamemode cycle scriptable object
 *  A scriptable object used to store how long each game mode cycle should take.
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "TimePerGamemodeCycleScriptableObject", menuName = "Scriptable Objects/Multiplayer/Time per game mode cycle")]
public class TimePerGamemodeCycleScriptableObject : ScriptableObject
{
    public int secondsPerCycle;
}
