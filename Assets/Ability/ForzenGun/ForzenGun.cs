using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ForzenGun : MonoBehaviour
{
  [SerializeField] private GameObject ForzenVFX;
  [SerializeField] private ParticleSystem ForzenEffect;
  [SerializeField] private ParticleSystem forzenGun;
  [SerializeField] private FlameThrowerAttackRadius AttackRadius;




  [SerializeField] private int ForzenDPS;
  [SerializeField] private float ForzenDuration;
  [SerializeField] private float ForzenAmmo;


  private ObjectPool<ParticleSystem> OnForzenPool;

  //private Dictionary<Enemy, ParticleSystem> EnemyParticleSystems = new();

  private void Awake()
  {
    OnForzenPool = new ObjectPool<ParticleSystem>(CreateOnFireSystem);
    //AttackRadius.OnEnemyEnter += StartDamagingEnemy;
    //AttackRadius.OnEnemyExit += StopDamagingEnemy;

    forzenGun.Stop();
    AttackRadius.gameObject.SetActive(false);
  }



  private ParticleSystem CreateOnFireSystem()
  {
    return Instantiate(ForzenEffect);
  }




  //private void StartDamagingEnemy(Enemy Enemy)
  //{
  //  if (Enemy.TryGetComponent<IForzen>(out IForzen Forzen))
  //  {
  //    Forzen.StartForzen(ForzenDPS);

  //    ParticleSystem onForzenSystem = OnForzenPool.Get();
  //    onForzenSystem.transform.SetParent(Enemy.transform, false);
  //    onForzenSystem.transform.localPosition = Vector3.zero;
  //    ParticleSystem.MainModule main = onForzenSystem.main;
  //    main.loop = true;
  //    EnemyParticleSystems.Add(Enemy, onForzenSystem);
  //  }
  //}


  //private IEnumerator DelayedDisableForzen(Enemy Enemy, ParticleSystem Instance, float Duration)
  //{
  //  ParticleSystem.MainModule main = Instance.main;
  //  main.loop = false;
  //  yield return new WaitForSeconds(Duration);
  //  Instance.gameObject.SetActive(false);
  //  if (Enemy.TryGetComponent<IForzen>(out IForzen Forzen))
  //  {
  //    Forzen.StopForzen();
  //  }
  //}

  //private void StopDamagingEnemy(Enemy Enemy)
  //{

  //  if (EnemyParticleSystems.ContainsKey(Enemy))
  //  {
  //    StartCoroutine(DelayedDisableForzen(Enemy, EnemyParticleSystems[Enemy], ForzenDuration));
  //    EnemyParticleSystems.Remove(Enemy);
  //  }
  //}

  public void Activate()
  {
    forzenGun.Play();
    AttackRadius.gameObject.SetActive(true);
    ForzenAmmo--;
  }

  public void Deactivate()
  {
    forzenGun.Stop();
    AttackRadius.gameObject.SetActive(false);
  }

}
