//*************************************************************************************************************
/*  Car select data scriptable object
 *  A scriptable object used to represent the players car selection
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - unknown date - created file
 *      Dean - unknown date - name is automatically assigned
 */
//*************************************************************************************************************


using UnityEngine;

[CreateAssetMenu(fileName = "CarSelectDataScriptableObject", menuName = "Scriptable Objects/Multiplayer/Car Select Data")]
public class CarSelectDataScriptableObject : ScriptableObject
{
    public string carName;
    public GameObject carPrefab;
    public GameObject uiCarPrefab;

    public string type;
    public string driverName;
    public string vehicleBest;
    public string abilities;
    
    [Space]
    
    [Tooltip("0-3")]
    public int speed;
    [Tooltip("0-3")]
    public int handling;
    [Tooltip("0-3")]
    public int health;
    
    private void OnEnable()
    {
        carName = name;
    }
}
