using System.Collections;
using UnityEngine;



public class EnemyHealth : MonoBehaviour, IDamageable 

{
    [SerializeField] private int health;
    Rigidbody rb;

  private ProjectileStats stats;

  public int Health { get => health; set => health = value; }

  [SerializeField] private bool _IsBurning;
    [SerializeField] private bool _IsForzen;
    public bool IsBurning { get => _IsBurning; set => _IsBurning = value; }
    public bool IsForzen { get => IsForzen; set => IsForzen = value; }
    private Coroutine BurnCoroutine;
    private Coroutine ForzenCoroutine;


  public void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 EnemyVelocity = rb.velocity;

    }

  
  public void TakeDamage(int Damage)
    {
    

    Health -= Damage;

    if (Health <= 0)
        {
            Health = 0;

            StopBurning();
            StopForzen();
            Destroy(gameObject);
        }
    }
        

  public void StartBurning(int DamagePerSecond)
    {
        IsBurning = true;
        if (BurnCoroutine != null)
        {
            StopCoroutine(BurnCoroutine);
    }

    BurnCoroutine = StartCoroutine(Burn(DamagePerSecond));
    }


  public void StartForzen(int DamagePerSecond)
    {
        IsForzen = true;
        rb.velocity = new Vector3(DamagePerSecond, 0, 0);
        if (ForzenCoroutine != null)
        {
            StopCoroutine(ForzenCoroutine);

    }

    ForzenCoroutine = StartCoroutine(Froze(DamagePerSecond));
    }

  private IEnumerator Burn(int DamagePerSecond)
    {
        float minTimeToDamage = 1f / DamagePerSecond;
        WaitForSeconds wait = new WaitForSeconds(minTimeToDamage);
        int damagePerTick = (int)(Mathf.Floor(minTimeToDamage));

    TakeDamage(damagePerTick);
        while (IsBurning)
        {
            yield return wait;
            TakeDamage(damagePerTick);
        }
    }

  private IEnumerator Froze(int DamagePerSecond)
    {
        float minTimeToDamage = 1f / DamagePerSecond;
        WaitForSeconds wait = new WaitForSeconds(minTimeToDamage);
        int damagePerTick = (int)(Mathf.Floor(minTimeToDamage));

    TakeDamage(damagePerTick);
        while (IsForzen)
        {
            yield return wait;
            TakeDamage(damagePerTick);
        }
    }


  public void StopBurning()
    {
        IsBurning = false;
        if (BurnCoroutine != null)
        {
            StopCoroutine(BurnCoroutine);
        }
    }

  public void StopForzen()
    {
        IsForzen = false;
        if (ForzenCoroutine != null)
        {
            StopCoroutine(ForzenCoroutine);
        }
    }
}
