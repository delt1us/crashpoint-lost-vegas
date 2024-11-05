/**************************************************************************************************************
* Player Controller
* Mainly responsible for debuggin 
* 
*           (Used.. but UNSUSED)
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private InputManager inputManager;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject debugMenu;
    private bool paused;
    
    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponentInChildren<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    //////////////////// //////// DEBUGGING ////////////////////////////  

    public void OnPause()
    {
        paused = !paused;

        pauseMenu.SetActive(paused);
        debugMenu.SetActive(!paused);
        if(GetComponentInChildren<ActionController>()) GetComponentInChildren<ActionController>().enabled = !paused;
        if (GetComponentInChildren<MountedGun>()) GetComponentInChildren<MountedGun>().enabled = !paused;

       
        if(paused) Cursor.lockState = CursorLockMode.None;
        else if(Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.GetComponentInChildren<Toggle>().isOn = inputManager.AimToggle;
    }
}
