//*************************************************************************************************************
/*  Network AI
 *  A script used as a representation of an AI player
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkAI : TeamMember
{
    [SerializeField] private AIArrayScriptableObject aiArrayScriptableObject;
    [SerializeField] private HealthScriptableObject healthScriptableObject;
    
    private HealthManager _healthManager;
    private GameObject _carGameObject;
    
    private AI_Weapon_Controller _aiWeaponController;
    private AI_Nav_Movement_1 _aiNavMovement1;

    private void Awake()
    {
        type = ETeamMember.NetworkAI;
    }

    private void Start()
    {
        if (!IsServer) return;
        _SpawnRandomCar();       
    }

    private void OnEnable()
    {   
        GameMode.Instance.StartTimer.TimerFinishedEvent += _EnableAiControl;
    }

    private void OnDisable()
    {
        GameMode.Instance.StartTimer.TimerFinishedEvent -= _EnableAiControl;
    }

    private void _Die()
    {
        if (!IsServer) return;
        GameMode.Instance.KillPlayer(this);
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            _healthManager.DeathEvent -= _Die;
            _DisableAiControl();
            _StartRespawnTimer();    
            yield return new WaitForSeconds(2);
            _carGameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void _EnableAiControl()
    {
        _aiWeaponController.aiHasControl.Value = true;
        _aiNavMovement1.aiHasControl.Value = true;
    }

    private void _DisableAiControl()
    {
        _aiWeaponController.aiHasControl.Value = false;
        _aiNavMovement1.aiHasControl.Value = false;
    }
    
    private void _StartRespawnTimer()
    {
        StartCoroutine(Coroutine());
        
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(healthScriptableObject.RespawnTimeSeconds);
            _SpawnRandomCar();
            _EnableAiControl();
            print("Respawned car");
        }
    }
    
    private void _SpawnRandomCar()
    {
        GameObject chosenPrefab = aiArrayScriptableObject.aiPrefabsArray[
            Random.Range(0, aiArrayScriptableObject.aiPrefabsArray.Length)];
        
        _carGameObject = Instantiate(chosenPrefab, new Vector3(), new Quaternion());
        _carGameObject.GetComponent<NetworkObject>().Spawn();
        
        Transform randomSpawnPoint = GameMode.Instance.GetRandomSpawnPoint();
        _carGameObject.transform.position = randomSpawnPoint.position;
        
        NetworkObject thisNetworkObject = GetComponent<NetworkObject>();
        NetworkObject carNetworkObject = _carGameObject.GetComponent<NetworkObject>();
        bool setParent = false;
        while (!setParent)
        {
            setParent = carNetworkObject.TrySetParent(thisNetworkObject);
        }
        _healthManager = GetComponentInChildren<HealthManager>();
        _healthManager.DeathEvent += _Die;
        _aiWeaponController = GetComponentInChildren<AI_Weapon_Controller>();
        _aiNavMovement1 = GetComponentInChildren<AI_Nav_Movement_1>();
    }
}