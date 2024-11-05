//*************************************************************************************************************
/*  Waiting Screen
 *  This is actually an old file that was used by a previous UI element. It is no longer used
 *  It used to enable certain ui elements depending on whether the current client was a host or not
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - unknown date - Made temporary waiting screen
 *      Armin - unknown date - No longer used
 */
//*************************************************************************************************************

using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingScreen : NetworkBehaviour
{
    [SerializeField] private GameObject hostOnlyGameObject;
    [SerializeField] private GameObject clientOnlyGameObject;
    [SerializeField] private GameObject UIHolderGameObject;
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    [SerializeField] private GameStartedBooleanScriptableObject gameStartedBooleanScriptableObject;

    private void OnEnable()
    {
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent += CheckIfLobbyFull;
    }

    private void OnDisable()
    {
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent -= CheckIfLobbyFull;
    }

    // Called from previous UI 
    public void SetChildrenActive()
    {
        UIHolderGameObject.SetActive(true);
        if (IsServer)
        {
            hostOnlyGameObject.SetActive(true);
            clientOnlyGameObject.SetActive(false);
        }
            
        else
        {
            hostOnlyGameObject.SetActive(false);
            clientOnlyGameObject.SetActive(true);
        }
    }
    
    // Called from NewPlayerConnected event 
    public void CheckIfLobbyFull()
    {
        if (numberOfConnectionsScriptableObject.Connections == 4) StartGame();
    }

    // Called from CheckIfLobbyFull and from pressing force start button on host
    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("Hunted", LoadSceneMode.Single);
        gameStartedBooleanScriptableObject.Started = true;
    }
}
