//*************************************************************************************************************
/*  Matchmaking
 *  I hate this because of an occasional issue that occurs where the player joins an empty lobby as the client
 *      This is a hardlock and the only way to reset it is to restart the game
 *      I couldn't find a way around this whatsoever, mainly because I can't reproduce this issue reliably
 *          Cause is unknown.
 *  This file is effectively an interface between this game and UnityServices. It lets us get around the need for
 *      port forwarding and firewall by connecting to the unity Relay.
 *  This file is in charge of joining a lobby or creating one if none are available
 *  A lot of this was taken from a video linked below, though it is basically the same as the things in the documentation
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 11/07/23 - Created file
 *      Armin - 13/07/23 - Matchmaking working
 *      Armin - 29/07/23 - Lobby now gets deleted when the game is started
 *      Armin - 05/08/23 - Support for joining a game after having left a game
 *      Armin - 10/08/23 - Added handling for edge case for not finding a lobby
 *      Armin - 10/08/23 - Improved matchmaking
 *      Armin - 12/08/23 - Another fix
 *      Armin - 12/08/23 - Another fix again
 *      Armin - 12/08/23 - Fixed
 *
 *  Bug status 13/08/23: not fixed 
 */
//*************************************************************************************************************


// A lot of this is from a youtube video
// https://www.youtube.com/watch?v=fdkvm21Y0xE&list=WL&index=1

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

#if  UNITY_EDITOR
using ParrelSync;
#endif

public class Matchmaking : NetworkBehaviour
{
    private const string _JOIN_CODE_KEY = "j";
    public static Matchmaking Singleton;
    private Lobby _connectedLobby;
    private UnityTransport _transport;
    private string _playerId;

    [SerializeField] private GameStartedBooleanScriptableObject gameStartedBooleanScriptableObject;
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    [SerializeField] private PlayerAuthenticatedScriptableObject playerAuthenticatedScriptableObject;
    [SerializeField] private MaxPlayersScriptableObject maxPlayersScriptableObject;
    
    private bool _connectedBool;
    
    public delegate void LobbyJoined();
    public event LobbyJoined OnLobbyJoined;
    
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else if (Singleton != this)
        {
            Destroy(gameObject);
        }
        
        _transport = FindObjectOfType<UnityTransport>();
    }

    private void Start()
    {
        // print($"authenticated?: {playerAuthenticatedScriptableObject.authenticated}");
        // if (!playerAuthenticatedScriptableObject.authenticated) await _Authenticate();
        // _connectedLobby = await _QuickJoinLobby() ?? await _CreateLobby();
        _JoinOrMakeLobby();
    }
    
    private async void _JoinOrMakeLobby()
    {
        if (!playerAuthenticatedScriptableObject.authenticated) await _Authenticate();
        _connectedLobby = await _QuickJoinLobby() ?? await _CreateLobby();
        OnLobbyJoined?.Invoke();
        //
        // // Sometimes player joins an empty lobby and is the client
        // print($"playercount: {_connectedLobby.Players.Count}\nhost id: {_connectedLobby.HostId} player id: {_playerId}");
        // if (_connectedLobby.Players.Count == 1 && _connectedLobby.HostId != _playerId)
        // {
        //     print("joined empty lobby, retrying");
        //     _StopLobby();
        //     NetworkManager.Singleton.Shutdown();
        //     _JoinOrMakeLobby();
        // }
        // else
        // {
        //     print("invoking event");
        // }
    }

    private void OnEnable()
    {
        gameStartedBooleanScriptableObject.GameStartedEvent += _StopLobby;
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent += CheckIfLobbyFull;
    }

    private void OnDisable()
    {
        gameStartedBooleanScriptableObject.GameStartedEvent -= _StopLobby;
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent -= CheckIfLobbyFull;
    }

    public bool _IsHost()
    {
        return _connectedLobby.HostId == _playerId;
    }
    
    // Called from force start button and CheckIfLobbyFull
    public void StartGame()
    {
        Debug.Log("starting game");
        Matchmaking.Singleton = null;
        // var gameMode = Random.Range(0, 101) >= 50 ? "Hunted" : "KingOfTheHill";
        NetworkManager.Singleton.SceneManager.LoadScene("KingOfTheHill", LoadSceneMode.Single);
        gameStartedBooleanScriptableObject.Started = true;
    }
    
    // Called from NewPlayerConnected event 
    public void CheckIfLobbyFull()
    {
        if (numberOfConnectionsScriptableObject.Connections >= maxPlayersScriptableObject.maxPlayers) StartGame();
    }
    
    private async Task _Authenticate()
    {
        try
        {
            var options = new InitializationOptions();
            
#if UNITY_EDITOR
            /* This is needed for parallel sync
             * UnityServices Lobby requires you to be on a different computer, this is a workaround for parallel sync
             */
            options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerId = AuthenticationService.Instance.PlayerId;
            playerAuthenticatedScriptableObject.authenticated = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Lobby> _QuickJoinLobby()
    {
        try
        {
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions());
            var allocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[_JOIN_CODE_KEY].Value);

            _transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            _playerId = AuthenticationService.Instance.PlayerId;
            
            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private async Task<Lobby> _CreateLobby()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayersScriptableObject.maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                    { { _JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync("Random Lobby Name", maxPlayersScriptableObject.maxPlayers, options);

            // Lobby times out after 30 seconds of inactivity. This prevents that
            StartCoroutine(KeepLobbyActive(lobby.Id, 15));

            _transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private static IEnumerator KeepLobbyActive(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void _StopLobby()
    {
        try
        {
            // Stops heartbeat
            StopAllCoroutines();
            if (_connectedLobby != null)
            {
                // If player is host
                if (_connectedLobby.HostId == _playerId) Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                // If player is client
                else Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    // Prevents some shenanigans like ghost players that have left the game already
    private void OnDestroy()
    {
        _StopLobby();
    }
}