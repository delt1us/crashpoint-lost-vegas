//*************************************************************************************************************
/*  Car select menu controller
 *  A controller for the car select menu:
 *      Handles all the UI elements
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *
 */
//*************************************************************************************************************

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CarSelectMenuController : MonoBehaviour
{
    [SerializeField] private CamController camController;
    [SerializeField] private CarSelectSpawner carSelectSpawner;
    
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    [SerializeField] private PlayersReadyScriptableObject playersReadyScriptableObject;
    
    private Button _confirmButton;
    private Button _previousButton;
    private Button _nextButton;
    private Button _backButton;
    
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
     
        _confirmButton = root.Q<Button>("confirm-button");
        _confirmButton.clicked += _LoadLoadingScreen;
        
        _previousButton = root.Q<Button>("cycle-back");
        _previousButton.clicked += carSelectSpawner._CycleCarBack;
        
        _nextButton = root.Q<Button>("cycle-forward");
        _nextButton.clicked += carSelectSpawner._CycleCarForward;
        
        _backButton = root.Q<Button>("back-button");
        _backButton.clicked += camController.SetCameraDestinationMainMenu;
        _backButton.clicked += camController.DisableCarSelectScreen;
        
        carSelectSpawner.SetUiElements(root);
    }

    private void OnDisable()
    {
        _previousButton.clicked -= carSelectSpawner._CycleCarBack;
        _nextButton.clicked -= carSelectSpawner._CycleCarForward;
        
        _backButton.clicked -= camController.SetCameraDestinationMainMenu;
        _backButton.clicked -= camController.DisableCarSelectScreen;
    }

    private void _LoadLoadingScreen()
    {
        numberOfConnectionsScriptableObject.ResetValues();
        playersReadyScriptableObject.ResetValues();
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("NewMainMenu");
    }
}