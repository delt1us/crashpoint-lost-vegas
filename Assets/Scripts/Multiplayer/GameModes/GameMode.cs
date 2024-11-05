//*************************************************************************************************************
/* GameMode
 * This is the super class for both game modes. It does:
 *      Getting random spawnpoints
 *      Spawning AI
 *      Adding score for players dying
 *      Updating team score for the UI
 *      Ending the game
 *      Assigning player data such as Id, TeamId
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 07/08/23 - Massive refactor of game modes
 *      Armin - 08/08/23 - Fixed score bars not updating
 *      Armin - 08/08/23 - Fixed respawn timer
 *      Armin - 08/08/23 - Fixed capture point sync issue
 *      Armin - 10/08/23 - Added support for AI spawning
 *      Armin - 11/08/23 - Added AI spawning
 *      Armin - 11/08/23 - Fixed AI targeting
 *      Armin - 13/08/23 - Removed friendly fire
 *      Armin - 13/08/23 - Added range check for spawnpoints
 *      Dean -  15/08/23 - Players are spawned with the team at the 2 closest spawn locations from the capture point.
 */
//*************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMode : NetworkBehaviour
{
    // Singleton
    public static GameMode Instance = null;

    // Public
    public TeamMember thisClientsPlayer;
    public NetworkVariable<float> teamOneScore = new NetworkVariable<float>(0);
    public NetworkVariable<float> teamTwoScore = new NetworkVariable<float>(0);
    
    public CountdownTimer StartTimer;

    // Serialize fields
    [SerializeField] protected TMP_Text debugInfoText;
    [SerializeField] protected PlayersReadyScriptableObject playersReadyScriptableObject;
    [SerializeField] protected ScoreValuesScriptableObject scoreValuesScriptableObject;
    [SerializeField] protected TeamScoreScriptableObject teamOneScoreScriptableObject;
    [SerializeField] protected TeamScoreScriptableObject teamTwoScoreScriptableObject;
    [SerializeField] protected PlayerHasControlBoolScriptableObject playerHasControlBoolScriptableObject;
    [SerializeField] protected WinningTeamScriptableObject winningTeamScriptableObject;
    [SerializeField] protected TimePerGamemodeCycleScriptableObject timePerGamemodeCycleScriptableObject;
    [SerializeField] private AIArrayScriptableObject aiArrayScriptableObject;
    [SerializeField] private MaxPlayersScriptableObject maxPlayersScriptableObject;
    [SerializeField] protected CountdownTimer GameTimer;
    
    [SerializeField] private GameObject[] objectsToDisable;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private CameraPan endGameCamera;
    [SerializeField] private Transform endGameCameraLocation;
    
    public List<TeamMember> Players { get; private set; }
    
    // Protected
    protected List<TeamMember> NetworkAis;
    protected List<TeamMember> TeamMembers;

    protected GameObject SpawnPoints;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Players = new List<TeamMember>();
        NetworkAis = new List<TeamMember>();
        TeamMembers = new List<TeamMember>();
        SpawnPoints = GameObject.FindWithTag("Spawnpoint");
        
        Destroy(GameObject.Find("MusicPlayer"));
    }

    protected virtual void OnEnable()
    {
        playersReadyScriptableObject.AllPlayersReadyEvent += StartTimer.StartTimer;
        // playersReadyScriptableObject.AllPlayersReadyEvent += _SpawnAI;
        StartTimer.TimerStartedEvent += playerHasControlBoolScriptableObject.RemoveControl;
        StartTimer.TimerFinishedEvent += _SetupTeamScoreUpdating;
        StartTimer.TimerFinishedEvent += playerHasControlBoolScriptableObject.ReturnControl;
        StartTimer.TimerFinishedEvent += GameTimer.StartTimer;
        GameTimer.TimerFinishedEvent += _CheckIfGameOver;
    }
    
    protected virtual void OnDisable()
    {
        playersReadyScriptableObject.AllPlayersReadyEvent -= StartTimer.StartTimer;
        // playersReadyScriptableObject.AllPlayersReadyEvent -= _SpawnAI;
        StartTimer.TimerStartedEvent -= playerHasControlBoolScriptableObject.RemoveControl;
        StartTimer.TimerFinishedEvent -= _SetupTeamScoreUpdating;
        StartTimer.TimerFinishedEvent -= playerHasControlBoolScriptableObject.ReturnControl;
        StartTimer.TimerFinishedEvent -= GameTimer.StartTimer;
        GameTimer.TimerFinishedEvent -= _CheckIfGameOver;
    }

    protected virtual void Update()
    {
        if (IsClient && (playersReadyScriptableObject.AllPlayersReadyBool || !IsServer)) _UpdateDebugInfo();
    }
    
    private void _SpawnAI()
    {
        Transform[] playerTransformArray = new Transform[Players.Count];
        for (int i = 0; i < playerTransformArray.Length; i++)
            playerTransformArray[i] = Players[i].GetComponentInChildren<HealthManager>().transform;
        int aiToSpawn = maxPlayersScriptableObject.maxAi + maxPlayersScriptableObject.maxPlayers - Players.Count;
        
        GameObject aiHolder = GameObject.FindWithTag("AIHolder");
        NetworkObject aiHolderNetworkObject = aiHolder.GetComponent<NetworkObject>();
        
        // Using j just to differentiate from above for loop
        for (int j = 0; j < aiToSpawn; j++)
        {
            GameObject thisNetworkAi = Instantiate(aiArrayScriptableObject.networkAiPrefab);
            NetworkObject thisAisNetworkObject = thisNetworkAi.GetComponent<NetworkObject>();
            thisAisNetworkObject.Spawn();
            
            bool setParent = false;
            while (!setParent)     
            {
                setParent = thisAisNetworkObject.TrySetParent(aiHolderNetworkObject);
            }
            
            NetworkAI networkAI = thisNetworkAi.GetComponent<NetworkAI>();
            AssignPlayer(networkAI);
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        bool suitableSpawnpointFound = false;
        Transform spawnpoint = null;
        while (!suitableSpawnpointFound)
        {
            spawnpoint =
                SpawnPoints.transform.GetChild(Random.Range(0, SpawnPoints.transform.childCount));
            
            suitableSpawnpointFound = true;
            for (int i = 0; i < TeamMembers.Count; i++)
            {
                HealthManager carHealthManager = TeamMembers[i].gameObject.GetComponentInChildren<HealthManager>();
                if (!carHealthManager) continue;

                Vector3 thisCarsPosition = carHealthManager.transform.position;
                Vector3 thisSpawnpointsPosition = spawnpoint.position;
                if (0.5 >= Vector3.Distance(thisCarsPosition, thisSpawnpointsPosition)) suitableSpawnpointFound = false;
            }
        }
        
        spawnpoint.transform.rotation = quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        return spawnpoint;
    }


    // The player spawns with their teammates and always spawn at the 2 closest spawnpoints from the active capture point
    public Transform GetSpawnKOTH(int teamId)
    {
        // Get the game object that contains all the possible spawns
        GameObject[] spawnGroups = GameObject.FindGameObjectsWithTag("SpawnGroup");

        // If it didn't get it, ignore the rest of the function.
        if (spawnGroups.Length < 1) return null; 

        // Get all of the capture points in the level
        GameObject[] capturePoints = GameObject.FindGameObjectsWithTag("CapturePoint");
        GameObject activePoint = null;

        // Get the active point (all the other points would be inactive)....
        foreach (GameObject point in capturePoints) if (point.activeSelf) activePoint = point; 

        float shortestDistance = 9999999;
        float secondShortest = 9999998;
        
        // Go through all the spawn groups
        foreach (GameObject spawnGroup in spawnGroups)
        {
            // Get the shortest distance
            float distanceFromPoint = Vector3.Distance(spawnGroup.transform.position, activePoint.transform.position);
            shortestDistance = distanceFromPoint < shortestDistance? distanceFromPoint: shortestDistance;

            // Get the second shortest distance ... since there's 2 teams
            // (If the distance is further than the shortest distance but closer than than the previous second shortest distance)
            secondShortest = distanceFromPoint > shortestDistance && distanceFromPoint < secondShortest? distanceFromPoint: secondShortest;
        }

        GameObject spawnA = null;
        GameObject spawnB = null;

        GameObject selectedSpawn;

        foreach (GameObject spawnGroup in spawnGroups)
        {
            // If its the closest spawn group
            if (Mathf.Approximately(shortestDistance, Vector3.Distance(spawnGroup.transform.position, activePoint.transform.position)))
            {
                spawnA = spawnGroup;
            }

            else if (Mathf.Approximately(secondShortest, Vector3.Distance(spawnGroup.transform.position, activePoint.transform.position)))
            {
                spawnB = spawnGroup;
            }
        }

        selectedSpawn = teamId == 1? spawnA : spawnB;

        Transform spawnpoint = selectedSpawn.transform.GetChild(Random.Range(0, selectedSpawn.transform.childCount));

        return spawnpoint;
    }

    public virtual void KillPlayer(TeamMember teamMember)
    {
        throw new NotImplementedException();
    }
    
    // Called when all players ready from event
    protected void _SetupTeamScoreUpdating()
    {
        teamOneScore.OnValueChanged += _UpdateTeamOneScore;
        teamTwoScore.OnValueChanged += _UpdateTeamTwoScore;
    }
    
    protected void _UpdateTeamOneScore(float old, float current)
    {
        teamOneScoreScriptableObject.Score = (int)current;
    }
    
    protected void _UpdateTeamTwoScore(float old, float current)
    {
        teamTwoScoreScriptableObject.Score = (int)current;
    }
    
    protected virtual void _CheckIfGameOver()
    {
        if (GameTimer.finished) _TeamWins(_GetTeamWithMorePoints());
    }
    
    protected int _GetTeamWithMorePoints()
    {
        if ((int)teamOneScore.Value == (int)teamTwoScore.Value) return 0;
        if (teamOneScore.Value > teamTwoScore.Value) return 1;
        return 2;
    }
    
    protected void _TeamWins(int winningTeam)
    {
        StartCoroutine(HalfSecondDelay());
        IEnumerator HalfSecondDelay()
        {
            yield return new WaitForSeconds(1f);
            _EndGameClientRpc(winningTeam);
        }
    }
    
    [ClientRpc]
    protected void _EndGameClientRpc(int winningTeam)
    {
        winningTeamScriptableObject.winningTeam = winningTeam;
        winningTeamScriptableObject.thisPlayersTeam = thisClientsPlayer.teamId.Value;
        
        endGameCamera.gameObject.SetActive(true);
        endGameCamera.transform.position = thisClientsPlayer.transform.position;
        endGameCamera.transform.rotation = thisClientsPlayer.transform.rotation;
        
        if (IsServer) Destroy(GameObject.Find("Player Holder"));
        
        Destroy(NetworkManager.Singleton);
        Destroy(NetworkManager.Singleton.gameObject);

        Cursor.lockState = CursorLockMode.None;
        
        // Enable post game stuff
        foreach (var obj in objectsToDisable) obj.SetActive(false);
        endGameCamera.SetDestination(endGameCameraLocation.position, endGameCameraLocation.rotation, 0.5f);
        endGameCamera.OnTargetReached += _EnableUI;
    }

    private void _EnableUI()
    {
        endGameUI.SetActive(true);
    }
    
    protected virtual void _UpdateDebugInfo()
    {
        return;
        if (!thisClientsPlayer) return;
        debugInfoText.text = $"ID: {thisClientsPlayer.id.Value}\n" +
                             $"Team: {thisClientsPlayer.teamId.Value}\n" +
                             $"Team 1 Score: {(int)teamOneScore.Value}\n" +
                             $"Team 2 Score: {(int)teamTwoScore.Value}";
    }
    
    // Called from Player script
    public void AssignPlayer(TeamMember teamMember)
    {
        switch (teamMember.type)
        {
            // Get added to players list on all clients
            case ETeamMember.Player:
            {
                Players.Add(teamMember);
                if (teamMember.IsOwner) thisClientsPlayer = teamMember;
                break;
            }
            case ETeamMember.NetworkAI:
                NetworkAis.Add(teamMember);
                break;
        }
        TeamMembers.Add(teamMember);
        
        // Only happens on server
        if (!IsServer) return;
        
        // Find smallest team
        int teamOneCount = 0;
        int teamTwoCount = 0;
        var teamMembers = Players.Concat(NetworkAis);
        foreach (var playerIterable in teamMembers)
        {
            if (playerIterable.teamId.Value == 1) teamOneCount++;
            else if (playerIterable.teamId.Value == 2) teamTwoCount++;
        }

        if (teamOneCount < teamTwoCount) teamMember.teamId.Value = 1;
        else teamMember.teamId.Value = 2;
    }
}