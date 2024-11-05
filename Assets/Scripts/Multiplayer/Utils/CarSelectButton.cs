//*************************************************************************************************************
/*  Car select button
 *  A script to pick a car to load into the game with
 *  No longer used
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

public class CarSelectButton : MonoBehaviour
{
    [SerializeField] private CarSelectDataScriptableObject carSelectDataScriptableObject;
    [SerializeField] private SelectedCarScriptableObject selectedCarScriptableObject;
    // Ran when button clicked
    public void Clicked()
    {
        selectedCarScriptableObject.SetCar(carSelectDataScriptableObject.carName);        
    }
}
