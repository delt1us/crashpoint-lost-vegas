//*************************************************************************************************************
/*  Car select spawned
 *  Somewhat similar to the old file that was made by Callum but heavily modified by me
 *  This is completely new though
 *  Spawns the UI cars for the car select menu and handles switching between them
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

struct carStruct
{
    public GameObject car;
    public CarSelectDataScriptableObject scriptableObject;

    public carStruct(GameObject newCar, CarSelectDataScriptableObject newScriptableObject)
    {
        car = newCar;
        scriptableObject = newScriptableObject;
    }
}

public class CarSelectSpawner : MonoBehaviour
{
    [SerializeField] private CarSelectDataScriptableObject[] carSelectDataScriptableObjects;
    [SerializeField] private Transform carSpawnLocation;
    [SerializeField] private SelectedCarScriptableObject selectedCarScriptableObject;
    
    private GameObject _currentCar;
    private Transform _currentCarRotationTransform;
    private List<carStruct> _cars;
    private int _currentCarIndex = -1;

    private Label _vehicleTypeLabel;
    private Label _driverNameLabel;
    private Label _vehicleBestLabel;
    private Label _abilitiesLabel;
    
    private ProgressBar _speedProgressBar;
    private ProgressBar _handlingProgressBar;
    private ProgressBar _healthProgressBar;

    // Start is called before the first frame update
    void Start()
    {
        _cars = new List<carStruct>();
        for (int i = 0; i < carSelectDataScriptableObjects.Length; i++)
        {
            GameObject carPrefab = carSelectDataScriptableObjects[i].uiCarPrefab;
            GameObject car = Instantiate(carPrefab, carSpawnLocation.position, carSpawnLocation.rotation, transform);
            carStruct thisCarStruct = new carStruct(car, carSelectDataScriptableObjects[i]);
            _cars.Add(thisCarStruct);
            car.SetActive(false);
        }
        _SetCar(0);
    }

    private void FixedUpdate()
    {
        _currentCarRotationTransform.Rotate(0f, 0.5f, 0f);
    }

    public void SetUiElements(VisualElement root)
    {
        _vehicleTypeLabel = root.Q<Label>("vehicle-type-label");
        _driverNameLabel = root.Q<Label>("driver-name-label");
        _vehicleBestLabel = root.Q<Label>("vehicle-best-label");
        _abilitiesLabel = root.Q<Label>("abilities-label");
        
        _speedProgressBar = root.Q<ProgressBar>("speed-progress-bar");
        _handlingProgressBar = root.Q<ProgressBar>("handling-progress-bar");
        _healthProgressBar = root.Q<ProgressBar>("health-progress-bar");
        
        _UpdateUiElementsForCar();
    }
    
    public void _CycleCarBack()
    {
        int index = _currentCarIndex - 1 >= 0 ? _currentCarIndex - 1 : _currentCarIndex;
        _SetCar(index);
    }

    public void _CycleCarForward()
    {
        int index = _currentCarIndex + 1 < _cars.Count ? _currentCarIndex + 1 : _currentCarIndex;
        _SetCar(index);
    }
    
    // Sets new car active and copies previous car rotation onto it
    private void _SetCar(int newIndex)
    {
        if (newIndex == _currentCarIndex) return;
        
        Quaternion previousRotation = _currentCar ? _currentCar.transform.rotation : quaternion.identity;
        if (_currentCar) {_currentCar.SetActive(false);}
        _currentCarIndex = newIndex;
        _currentCar = _cars[_currentCarIndex].car;
        selectedCarScriptableObject.SetCar(_cars[_currentCarIndex].scriptableObject.carName);
        _currentCarRotationTransform = _currentCar.transform.GetChild(0);
        _currentCar.SetActive(true);
        _currentCar.transform.rotation = previousRotation;
        _UpdateUiElementsForCar();
    }

    private void _UpdateUiElementsForCar()
    {
        // Means UI elements not loaded yet
        if (_vehicleTypeLabel == null) return;
        
        CarSelectDataScriptableObject currentCarData = _cars[_currentCarIndex].scriptableObject;
        
        _vehicleTypeLabel.text = currentCarData.type;
        _driverNameLabel.text = currentCarData.driverName;
        _vehicleBestLabel.text = currentCarData.vehicleBest;
        _abilitiesLabel.text = currentCarData.abilities;
        
        _speedProgressBar.value = currentCarData.speed;
        _handlingProgressBar.value = currentCarData.handling;
        _healthProgressBar.value = currentCarData.health;
    }
}
