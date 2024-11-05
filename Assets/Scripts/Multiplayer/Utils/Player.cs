//*************************************************************************************************************
/*  Player
 *  This is a subclass of TeamMember. It:
 *      Is in charge of spawning cars
 *      Handles dying and respawning
 *      Manages respawn timer client side
 *      Enabling client only components of cars
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : TeamMember
{
    private NetworkVariable<int> _respawnTimerNetworkVariable = new NetworkVariable<int>();
    private NetworkVariable<FixedString32Bytes> _chosenCarString = new NetworkVariable<FixedString32Bytes>("",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [SerializeField] private PlayerIDHandlerScriptableObject playerIDHandlerScriptableObject;
    [SerializeField] private ConnectedBooleanScriptableObject connectedBooleanScriptableObject;
    [SerializeField] private SelectedCarScriptableObject selectedCarScriptableObject;
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    [SerializeField] private PlayersReadyScriptableObject playersReadyScriptableObject;
    [SerializeField] private CarSelectDataScriptableObject[] carSelectDataScriptableObjects;
    [SerializeField] private PlayerHasControlBoolScriptableObject playerHasControlBoolScriptableObject;
    [SerializeField] private RespawnTimerScriptableObject respawnTimerScriptableObject;
    
    private bool _hasSceneLoadedEventAlreadyHappened;

    private GameObject _spectatorCamera;
    private GameObject _carGameObject;
    private GameMode _gameMode;

    private void Awake()
    {
        type = ETeamMember.Player;
    }

    private void Start()
    {
        numberOfConnectionsScriptableObject.AddConnection();
        // DontDestroyOnLoad(this);
        if (IsServer)
        {
            _GetId();
            _SetParentOfThisObject();
        }
        if (IsOwner)
        {
            connectedBooleanScriptableObject.Connected = true;
            
            selectedCarScriptableObject.CarSelectedEvent += _SetCar;
            
            _SetCar();
            _SetName("ThisClientsPlayer");
        }
    }
    
    private void OnEnable()
    {
        NetworkManager.SceneManager.OnSceneEvent += _OnSceneEvent;
        playerHasControlBoolScriptableObject.ControlRemovedEvent += _SetPlayerInputComponentInactive;
        playerHasControlBoolScriptableObject.ControlReturnedEvent += _SetPlayerInputComponentActive;
        
        if (IsOwner)
        {
            selectedCarScriptableObject.CarSelectedEvent += _SetCar;
        }
    }
    
    private void OnDisable()
    {
        NetworkManager.SceneManager.OnSceneEvent -= _OnSceneEvent;
        playerHasControlBoolScriptableObject.ControlRemovedEvent -= _SetPlayerInputComponentInactive;
        playerHasControlBoolScriptableObject.ControlReturnedEvent -= _SetPlayerInputComponentActive;

        if (IsOwner)
        {
            selectedCarScriptableObject.CarSelectedEvent -= _SetCar;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _respawnTimerNetworkVariable.OnValueChanged += _OnNetworkTimerChanged;
    }

    public override void OnNetworkDespawn()
    {
        _respawnTimerNetworkVariable.OnValueChanged -= _OnNetworkTimerChanged;
        base.OnNetworkDespawn();
    }

    // Called from NetworkManager.SceneManager.OnSceneEvent event
    // It is called regardless of what type of scene event has happened, so we manually have to check which it is
    private void _OnSceneEvent(SceneEvent sceneEvent)
    {
        // If event is not LoadEventCompleted event 
        // LoadEventCompleted means that all clients have finished loading their scene
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
        
        // Makes sure this only happens once on the host, otherwise it would happen once when the server gets this event
        // and once when the client gets it
        if (_hasSceneLoadedEventAlreadyHappened) return;
        _hasSceneLoadedEventAlreadyHappened = true;
            
        // Get spawnpoint if not already exists
        _spectatorCamera = GameObject.FindWithTag("SpectatorCamera");

        // Assign player
        _gameMode = HuntedManager.Instance ? HuntedManager.Instance : KingOfTheHillManager.Instance;
        _gameMode.AssignPlayer(this);
        
        if (IsServer) _SpawnCarAndSetParentsServerSide();
    }
    
    private void _SpawnCarAndSetParentsServerSide()
    {
        // Remember that this is run per player object
        _SpawnCar();
        _SetParentOfCarToThis();
        _CarSpawnedClientRpc();
    }
    
    [ClientRpc]
    private void _CarSpawnedClientRpc()
    {
        // Remember that this is run per player object
        _SetCarIfNotExists();
        _carGameObject.GetComponentInChildren<HealthManager>().DeathEvent += _Die;

        if (!IsOwner) return;
        
        _EnableClientOnlyComponents();
        playerHasControlBoolScriptableObject.ReturnControl();
        _spectatorCamera.SetActive(false);
        _IncreaseReadyPlayersServerRpc();
    }
    
    [ServerRpc]
    private void _IncreaseReadyPlayersServerRpc()
    {
        playersReadyScriptableObject.PlayersReady++;
    }
    
    // Called from event in healthmanager (subscribed to in spawncar)
    private void _Die()
    {
        _DieClientRpc();

        if (!IsServer) return;
        _gameMode.KillPlayer(this);
    }
    
    [ClientRpc]
    private void _DieClientRpc()
    {
        if (IsOwner || IsServer) StartCoroutine(Coroutine());
        
        IEnumerator Coroutine()
        {
            if (IsOwner) playerHasControlBoolScriptableObject.RemoveControl();
            _carGameObject.GetComponentInChildren<HealthManager>().DeathEvent -= _Die;
            yield return new WaitForSeconds(2);
            if (IsOwner) _EnableSpectatorCamera();
            if (IsServer) _StartRespawnTimer();
            yield return new WaitForSeconds(2);
            if (IsServer) Destroy(_carGameObject);
        }
    }
    
    private void _StartRespawnTimer()
    {
        _respawnTimerNetworkVariable.Value = respawnTimerScriptableObject.startingTime;
        _EnableTimerClientRpc();
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            while (_respawnTimerNetworkVariable.Value >= 0)
            {
                yield return new WaitForSeconds(1);
                _respawnTimerNetworkVariable.Value--;
            }
            _SpawnCarAndSetParentsServerSide();
        }
    }

    [ClientRpc]
    private void _EnableTimerClientRpc()
    {
        if (IsOwner) respawnTimerScriptableObject.StartTimer();
    }

    private void _OnNetworkTimerChanged(int oldValue, int newValue)
    {
        if (IsOwner) respawnTimerScriptableObject.CurrentTime = newValue;
    }
    
    // Called from DieCoroutine
    private void _EnableSpectatorCamera()
    {
        // _carGameObject.transform.Find("DebugScreen").gameObject.SetActive(false);
        _carGameObject.transform.Find("CarBody/CAMERAS").gameObject.SetActive(false);
        _carGameObject.transform.Find("CarBody/HUD").gameObject.SetActive(false);
        _spectatorCamera.SetActive(true);
        Debug.Log($"spectatorcamera active: {_spectatorCamera.activeSelf}");
    }

    // Called from event
    private void _SetPlayerInputComponentInactive()
    {
        GetComponentInChildren<PlayerInput>().enabled = false;
    }
    
    // Called from event
    private void _SetPlayerInputComponentActive()
    {
        GetComponentInChildren<PlayerInput>().enabled = true;
    }
    
    private void _SetCar()
    {
        _chosenCarString.Value = selectedCarScriptableObject.selectedCarString;
    }
    
    private void _GetId()
    {
        id.Value = playerIDHandlerScriptableObject.nextPlayerId;
        playerIDHandlerScriptableObject.nextPlayerId++;
    }
    
    private void _SetName(string newName)
    {
        gameObject.name = newName;
    }
    // This is needed as it is not guaranteed for parent setting to be successful
    // Sometimes it will fail because the object doesn't exist yet on the other device
    // Async stuff pepeW
    private void _SetParentOfObjectToOtherObject(NetworkObject newChild, NetworkObject newParent)
    {
        bool parentSetSuccessfully = false;
        while (!parentSetSuccessfully)
        {
            parentSetSuccessfully = newChild.TrySetParent(newParent);
        }
    }
    
    // Puts this object into the Players gameobject 
    private void _SetParentOfThisObject()
    {
        NetworkObject thisObjectsNetworkObject = GetComponent<NetworkObject>();
        NetworkObject playerHolderNetworkObject =
            GameObject.FindWithTag("PlayerHolder").GetComponent<NetworkObject>();
        _SetParentOfObjectToOtherObject(thisObjectsNetworkObject, playerHolderNetworkObject);
    }
    
    // Puts cars object as a child of this object
    private void _SetParentOfCarToThis()
    {
        NetworkObject thisObjectsNetworkObject = GetComponent<NetworkObject>();
        NetworkObject carNetworkObject = _carGameObject.GetComponent<NetworkObject>();
        _SetParentOfObjectToOtherObject(carNetworkObject, thisObjectsNetworkObject);
    }
    
    // Spawns car at same location as _spawnPointsGameObject
    private void _SpawnCar()
    {
        // Whether the player is in King of the Hill or Hunted
        bool inKOTH = GameMode.Instance;

        Transform spawnpoint = inKOTH? _gameMode.GetSpawnKOTH(teamId.Value): _gameMode.GetRandomSpawnPoint();

        // Get car prefab from _chosenCarString.Value and spawn it
        foreach (CarSelectDataScriptableObject carSelectDataScriptableObject in carSelectDataScriptableObjects)
        {
            if (_chosenCarString.Value == new FixedString32Bytes(carSelectDataScriptableObject.carName))
            {
                GameObject carPrefab = carSelectDataScriptableObject.carPrefab;
                print($"car prefab name: {carPrefab.name}");
                _carGameObject = Instantiate(carPrefab, spawnpoint.position, new());
                _carGameObject.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
                return;
            }
        }
        throw new Exception($"Car prefab for car with name: '{selectedCarScriptableObject.selectedCarString}' not found");
    }

    private void _SetCarIfNotExists()
    {
        // Returns if car is not null
        if (_carGameObject) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            // If it is the player
            if (transform.GetChild(i).CompareTag("Player"))
            {
                _carGameObject = transform.GetChild(i).gameObject;
            }
        }
        if (!_carGameObject) throw new Exception("Clients car not found");
    }
    
    // Enables client specific things like player input component, cameras and debug screen. 
    private void _EnableClientOnlyComponents()
    {
        GameObjectsToEnable gameObjectsToEnable = _carGameObject.GetComponent<GameObjectsToEnable>();
        foreach (GameObject gameObjectIterable in gameObjectsToEnable.gameObjectsList)
        {
            if (gameObjectIterable) gameObjectIterable.SetActive(true);
        }
        foreach (Behaviour behaviour in gameObjectsToEnable.behavioursList) behaviour.enabled = true;
    }
}