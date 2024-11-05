//*************************************************************************************************************
/*  Selected car scriptable object
 *  Used to store what car the player chose in the main menu
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "SelectedCarScriptableObject", menuName = "Scriptable Objects/Multiplayer/Selected Car Event")]
public class SelectedCarScriptableObject : ScriptableObject
{
    public string selectedCarString;

    public delegate void CarSelected();
    public event CarSelected CarSelectedEvent;

    private void OnEnable()
    {
        selectedCarString = null;
    }

    public void SetCar(string carString)
    {
        selectedCarString = carString;
        CarSelectedEvent?.Invoke();
    }
}
