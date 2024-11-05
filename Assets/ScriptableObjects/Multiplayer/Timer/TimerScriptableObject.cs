//*************************************************************************************************************
/*  Timer scriptable object
 *  A scriptable object used to store how long a specific timer should last
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "TimerScriptableObject", menuName = "Scriptable Objects/Multiplayer/Timer")]
public class TimerScriptableObject : ScriptableObject
{
    public int startingTime;
}
