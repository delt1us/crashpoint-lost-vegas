//*************************************************************************************************************
/*  Car selector
 *  A script for the car selector in the main menu
 *  It handlers cycling through the car options
 *  
 *  Created by Callum (insert surname) 2023
 *  Change log:
 *      Armin - 01/08/23 - Revamped car selector to allow easier addition of new cars
 */
//*************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
//c
// Summary 
// Attached To GameObjects - [ CarSelector ] 
// Purpose -                 [ Allows the Player to Choose a Car ] 
// Functions -               [ 1. Checks if the Player can Interact With the Buttons ]
//                           [ 2. Increments and Decrements the Array to Move to the Next/Previous Vehicle by Activating/Deactivating the Gameobjects and Then Saving the Selected Vehicle in PlayerPrefs ]
// Dependencies -            [ None ]
// Notes -  
public class CarSelector : MonoBehaviour
{
    // Public fields
    public GameObject activeCar;
    public Button forward;
    public Button previous;

    // Private Fields 
    private int index;
    private List<GameObject> _carsList;

    // Serialized Fields
    [SerializeField] private AudioSource ButtonSFX;
    [SerializeField] private CarSelectDataScriptableObject[] carsScriptableObjects;
    [SerializeField] private GameObject carSpawnPoint;
    [SerializeField] private GameObject carHolderGameObject;
    [SerializeField] private CarInfoUIButtonsScriptableObject carInfoUIButtonsScriptableObject;
    [SerializeField] private SelectedCarScriptableObject selectedCarScriptableObject;

    private void Start()
    {
        PlayerPrefs.SetInt("carIndex", 0);

        _carsList = new List<GameObject>();
        
        // Spawn every car
        foreach (var carSelectDataScriptableObject in carsScriptableObjects)
        {
            var car = Instantiate(carSelectDataScriptableObject.carPrefab, carSpawnPoint.transform.position,
                carSpawnPoint.transform.rotation, carHolderGameObject.transform);
            // Disable weapons
            foreach (var weaponController in car.GetComponentsInChildren<WeaponController>())
            {
                weaponController.gameObject.SetActive(false);
            }
            // Disable movement
            car.GetComponentInChildren<MovementController>().enabled = false;
            car.GetComponent<PlayerController>().enabled = false;
            car.GetComponentInChildren<ActionController>().enabled = false;
            car.GetComponentInChildren<InputManager>().enabled = false;
            
            car.SetActive(false);
            _carsList.Add(car);
        }
        
        // Enable the one previously enabled
        index = PlayerPrefs.GetInt("carIndex");

        _carsList[index].SetActive(true);
        activeCar = _carsList[index];
    }

    private void OnEnable()
    {
        carInfoUIButtonsScriptableObject.NextCarButtonPressedEvent += Forward;
        carInfoUIButtonsScriptableObject.PreviousCarButtonPressedEvent += Previous;
    }

    private void OnDisable()
    {
        carInfoUIButtonsScriptableObject.NextCarButtonPressedEvent -= Forward;
        carInfoUIButtonsScriptableObject.PreviousCarButtonPressedEvent -= Previous;
    }

    private void Update()
    {
        CanPress();
    }
    
   // Checks if the player can interact with the butons depedning on the current car index 
    private void CanPress()
    {
        if (index >= _carsList.Count - 1)
        {
            // TODO fix these
            forward.interactable  = false;
        }
        else
        {
            forward.interactable  = true;
        }
        if (index <= 0)
        {
            previous.interactable = false;
        }
        else
        {
            previous.interactable = true;
        }
    }

    // Called when confirm button pressed from button unity event
    public void SetChosenCar()
    {
        int i = PlayerPrefs.GetInt("carIndex");
        selectedCarScriptableObject.SetCar(carsScriptableObjects[i].carName);
    }
    
    // Adds the car index to move to the next car then saves it in PlayerPrefs
    public void Forward()
    {
        Debug.Log("forward");
        
        index++;
        
        // Deactivates all the cars expect for the newly selected one
        for (int i = 0; i < _carsList.Count; i++)
        {
            _carsList[i].SetActive(false);
        }
        _carsList[index].SetActive(true);
        activeCar = _carsList[index];
        
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
        ButtonSFX.Play();
    }

    // Decreases the car index to move to the next car then saves it in PlayerPrefs
    public void Previous()
    {
        Debug.Log("previous");
        
        index--;
        
        // Deactivates all the cars expect for the newly selected one
        for (int i = 0; i < _carsList.Count; i++)
        {
            _carsList[i].SetActive(false);
        }
        _carsList[index].SetActive(true);
        activeCar = _carsList[index];

        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
        ButtonSFX.Play();
    }
}
