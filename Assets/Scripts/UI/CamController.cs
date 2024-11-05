//*************************************************************************************************************
/*  Cam controller
 *  Would have named this Camera Controller, but that exists already.
 *  A script to handle the camera moving around, sets the destinations for the camera and also loads/unloads
 *      certain UI documents
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform carSelectCamera;
    
    [SerializeField] private float secondsBetweenMenus;
    [SerializeField] private float secondsBetweenPoints;

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject carSelectUI;
    [SerializeField] private GameObject carSelectSpawner;
    [SerializeField] private GameObject optionsUI;
    
    private CameraPan _cameraPan;
    private int _currentDestinationIndex = 1;
        
    private void Start()
    {
        _cameraPan = GetComponent<CameraPan>();
        SetCameraDestinationMainMenu();
    }
    
    private void _SwapTargets()
    {
        _cameraPan.SetDestination(points[_currentDestinationIndex].position, 
            points[_currentDestinationIndex].rotation, secondsBetweenPoints);
        _currentDestinationIndex = _currentDestinationIndex == 1 ? 0 : 1;
    }
    
    public void SetCameraDestinationMainMenu()
    {
        _cameraPan.SetDestination(points[_currentDestinationIndex].position,
            points[_currentDestinationIndex].rotation, 1f);
        _currentDestinationIndex = _currentDestinationIndex == 1 ? 0 : 1;
        _cameraPan.OnTargetReached += _EnableMainMenu;
        _cameraPan.OnTargetReached += _SwapTargets;
    }

    public void SetCameraDestinationCarSelect()
    {
        _cameraPan.OnTargetReached -= _SwapTargets;
        _cameraPan.OnTargetReached += _SetCarSelectActive;
        _cameraPan.SetDestination(carSelectCamera.position, carSelectCamera.rotation, secondsBetweenMenus);
        mainMenuUI.SetActive(false);
    }

    public void SetCameraDestinationOptionsMenu()
    {
        _cameraPan.OnTargetReached -= _SwapTargets;
        _cameraPan.OnTargetReached += _SetOptionsMenuActive;
        _cameraPan.SetDestination(optionsUI.transform.position, optionsUI.transform.rotation, secondsBetweenMenus);
        mainMenuUI.SetActive(false);
    }

    
    private void _SetCarSelectActive()
    {
        _cameraPan.OnTargetReached -= _SetCarSelectActive;
        carSelectSpawner.SetActive(true);
        if(carSelectUI) carSelectUI.SetActive(true);
    }

    private void _SetOptionsMenuActive()
    {
        _cameraPan.OnTargetReached -= _SetOptionsMenuActive;
        optionsUI.SetActive(true);
    }
    
    private void _EnableMainMenu()
    {
        mainMenuUI.SetActive(true);
        _cameraPan.OnTargetReached -= _EnableMainMenu;
    }

    public void DisableCarSelectScreen()
    {
        carSelectUI.SetActive(false);
    }
}
