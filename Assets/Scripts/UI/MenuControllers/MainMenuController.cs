//*************************************************************************************************************
/*  Main menu controller
 *  A controller to handle all the UI for the main menu
 *  Needed because of UI Toolkit
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 *      Dean - added menu music
 *
 */
//*************************************************************************************************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CamController camController;
    
    private Button _playButton;
    private Button _optionsButton;
    private Button _quitButton;

    private void OnEnable()
    { 
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _playButton = root.Q<Button>("play-button");
        _playButton.clicked += _GoToCarSelect;
        
        _optionsButton = root.Q<Button>("options-button");
        _optionsButton.clicked += _GoToOptions;
        
        _quitButton = root.Q<Button>("quit-button");
        _quitButton.clicked += _QuitGame;
    }

    private void OnDisable()
    {
        _playButton.clicked -= _GoToCarSelect;
        _optionsButton.clicked -= _GoToOptions;
        _quitButton.clicked -= _QuitGame;
    }

    private void Start()
    {
        _SetCapturePointsActive(false);
    }

    private void OnDestroy()
    {
        _SetCapturePointsActive(true);
    }

    private void _SetCapturePointsActive(bool active)
    {
        GameObject capturePoints = GameObject.FindWithTag("CapturePoint");
        for (int i = 0; i < capturePoints.transform.childCount; i++)
        {
            capturePoints.transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    
    private void _GoToCarSelect()
    {
        camController.SetCameraDestinationCarSelect();
        gameObject.SetActive(false);
    }

    private void _GoToOptions()
    {
        camController.SetCameraDestinationOptionsMenu();
        gameObject.SetActive(false);
    }

    private void _QuitGame()
    {
        print("quitting game");
        Application.Quit();
    }
}
