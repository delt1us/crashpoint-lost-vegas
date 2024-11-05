//*************************************************************************************************************
/*  Loading screen controller
 *  A script to handle the UI in the loading screen
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreenController : NetworkBehaviour
{
    private Button _forceStartButton;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        
        _forceStartButton = root.Q<Button>("force-start-button");
        _DisableStartButton();
    }

    private void Start()
    {
        Matchmaking.Singleton.OnLobbyJoined += _EnableStartButton;
    }

    private void _EnableStartButton()
    {
        _SetStartButton(true);
        _forceStartButton.clicked += Matchmaking.Singleton.StartGame;
    }

    private void _DisableStartButton()
    {
        _SetStartButton(false);
    }

    private void _SetStartButton(bool active)
    {
        if (active && !Matchmaking.Singleton._IsHost()) return;
        _forceStartButton.SetEnabled(active);
        _forceStartButton.visible = active;
    }
}
