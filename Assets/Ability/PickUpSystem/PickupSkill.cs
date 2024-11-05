using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSkill : MonoBehaviour
{
  [SerializeField] private GameObject Car;
  private PickupHandle Handle;
  private PlayerHealth playerHealth;


  Rigidbody rb;



  public float DelayBeforePickup = 1; //cooldown time

  public int HeldPickup;

  public bool CanPickup;
  private bool UsePickup;

  public Pickup PuUse;
  private int RemainingPickupUses; //limitation for the time pick up can be use



  private void Start()
  {
    Handle = GameObject.FindGameObjectWithTag("GameController").GetComponent<PickupHandle>();



    playerHealth = GetComponent<PlayerHealth>();

    

    ResetPickup();
  }

  private void Update()
  {
    UsePickup = Input.GetKeyDown(KeyCode.M);
    if (UsePickup && HeldPickup != -1)
    {
      ActivatePickup();
    }
  }

  public void ResetPickup()
  {
    PuUse = null;
    HeldPickup = -1;
    CanPickup = true;
  }

  public void StartPickup()
  {
    StartCoroutine(PickAction());
  }

  public IEnumerator PickAction()
  {
    if (HeldPickup == -1 && CanPickup)
    {
      CanPickup = false;

      yield return new WaitForSeconds(DelayBeforePickup);

      int PickupRand = Random.Range(0, Handle.AllPickups.Length);

      PuUse = Handle.AllPickups[PickupRand];

      HeldPickup = PickupRand;
      RemainingPickupUses = PuUse.Uses;
    }
  }

  public void ActivatePickup()
  {
    RemainingPickupUses -= 1;

    if (PuUse.Boost.Length > 0)
    {
      foreach (PickupBoostFunction PuBoost in PuUse.Boost)
      {
        
        rb = Car.GetComponent<Rigidbody>();
        rb.AddForce(Car.transform.forward * PuBoost.BoostAmt ,ForceMode.Acceleration);
      }
    }

    if (PuUse.Heal.Length > 0)
    {
      foreach (PickupHealFunction PuHeal in PuUse.Heal)
      {
        playerHealth.TakeHeal(PuHeal.healAmt);
      }
    }
    if (RemainingPickupUses <= 0)
    {
      ResetPickup();
    }
  }
}


