/**************************************************************************************************************
* FlameThrower
* For flame thrower damage and effect
*
* Created by Envy Cham 2023
* 
* Change Log:
*   Envy - 
*   Dean - Made it work with multiplayer... Had to make the weapon controller do everything 
*            
***************************************************************************************************************/



using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

public class FlameThrower : NetworkBehaviour
{
    [SerializeField] private VisualEffect flameVFX;

    private FlameThrowerAttackRadius attackRadius;

    //private ObjectPool<ParticleSystem> OnFirePool;

    //private Dictionary<HealthManager, ParticleSystem> EnemyParticleSystems = new();

    private void Start()
    {
        attackRadius = GetComponentInChildren<FlameThrowerAttackRadius>();

        flameVFX.Stop();

        //OnFirePool = new ObjectPool<ParticleSystem>(CreateOnFireSystem);
        //AttackRadius.OnEnemyEnter += StartDamagingEnemy;
        //AttackRadius.OnEnemyExit += StopDamagingEnemy;

        attackRadius.gameObject.SetActive(false);
    }


    [ClientRpc]
    public void ActivateFlameClientRpc()
    {
        if (!IsOwner) return; 
    }


    [ClientRpc]
    public void DeactivateFlameClientRpc()
    {
        if (!IsOwner) return;

        attackRadius.RemoveFlame();
    }

    public void RemoveFlame()
    {
        attackRadius.RemoveFlame();
    }

    public void ConfigureFlame(int dps, float duration, float tickRate, GameObject owner)
    {
        attackRadius.ConfigureFlame(dps, duration, tickRate, owner);
    }


    public GameObject GetFlameCollider()
    {
        if(!attackRadius) return null;

        return attackRadius.gameObject;
    }

    public VisualEffect GetFlameVFX()
    {
        if (!flameVFX) return null;

        return flameVFX;
    }

    //private void StartDamagingEnemy(HealthManager Players)
    //{
    //  if (Players.TryGetComponent<HealthManager>(out HealthManager Player))
    //  {
    //          if(IsOwner) Player.StartBurningServerRpc(controller.SpecialDPS);


    //    ParticleSystem onFireSystem = OnFirePool.Get();
    //    onFireSystem.transform.SetParent(Players.transform, false);
    //    onFireSystem.transform.localPosition = Vector3.zero;
    //    ParticleSystem.MainModule main = onFireSystem.main;
    //    main.loop = true;
    //    EnemyParticleSystems.Add(Players, onFireSystem);
    //  }
    //}


    //private IEnumerator DelayedDisableBurn(HealthManager Players, ParticleSystem Instance, float Duration)
    //{
    //  ParticleSystem.MainModule main = Instance.main;
    //  main.loop = false;
    //  yield return new WaitForSeconds(Duration);
    //  Instance.gameObject.SetActive(false);
    //  if (Players.TryGetComponent<HealthManager>(out HealthManager player))
    //  {
    //    player.StopBurningServerRpc();
    //  }
    //}

    //private void StopDamagingEnemy(HealthManager Players)
    //{
    //  if (EnemyParticleSystems.ContainsKey(Players))
    //  {
    //    StartCoroutine(DelayedDisableBurn(Players, EnemyParticleSystems[Players], controller.SpecialDuration));
    //    EnemyParticleSystems.Remove(Players);
    //  }
    //}

}


