/**************************************************************************************************************
* StickyBomb
* 
* Modified grenade that can stick into vehicles
*
* Created by Envy Cham 2023
* 
* Change Log:
*
*
*            
***************************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StickyBomb : MonoBehaviour
{

  [SerializeField] private ProjectileStats projStats;
  [SerializeField] private GameObject explosion;

  /*public Transform player;
  public LayerMask whatIsPlayer, whatIsGround;
  public NavMeshAgent agent;

  public Vector3 walkPoint;
  bool walkPointSet;
  public float walkPointRange;

  public float sightRange;
    public bool playerInSightRange;
  */

  //private bool blown;

  [SerializeField] private Rigidbody nadeBody;

 /* private void Awake()
  {
    GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
    Transform[] playerTransform = new Transform[targets.Length];
    for(int i= 0; i < targets.Length; i++)
    {
      playerTransform[i] = targets[i].transform;
      player = playerTransform[i];

    }


    agent = GetComponent<NavMeshAgent>();
  }*/
  private void Start()
  {
    //impact = projStats.FuseTime <= 0;
    //timerOn = !impact;

    nadeBody.AddForce(transform.forward * projStats.LaunchForce, ForceMode.Impulse);

    Destroy(gameObject, 30);
  }

 /* private void Update()
  {
    playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
    

    if (!playerInSightRange) Patroling();
    if(playerInSightRange) ChasePlayer();
  }

  private void ChasePlayer()
  {
    agent.SetDestination(player.position);
   // nadeBody.AddForce(transform.forward * Time.deltaTime, ForceMode.Acceleration);
  }

  private void Patroling()
  {
    if(!walkPointSet) SearchWalkPoint();

    if(walkPointSet)
      agent.SetDestination(walkPoint);

    Vector3 distanceToWalkPoint = transform.position - walkPoint;

    if (distanceToWalkPoint.magnitude < 1)
      walkPointSet = false;
  }

  private void SearchWalkPoint()
  {
    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    float randomX = Random.Range(-walkPointRange, walkPointRange);

    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    if(Physics.Raycast(walkPoint, -transform.up, 2, whatIsGround))
      walkPointSet = true;
  }
  */


  public void Throw(Vector3 velocity)
  {
    nadeBody.velocity += velocity;
  }




  private void FixedUpdate()
  {
   
    nadeBody.AddForce(Physics.gravity * projStats.GravityMultiplier);
  }

  //private void OnTriggerEnter(Collider other)
  //{
    //if(other.gameObject.tag == "Plyaer" || other.gameObject.tag == "AIPlayer")
    //{
     
      //  Explode();
    //}
  //}

  private void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "AIPlayer")
    {
      
      Explode();
    }
  }

  private void Explode()
  {
    //if (blown) return;

    GameObject newExplo = Instantiate(explosion, transform);
    //newExplo.GetComponentInChildren<BlastRadius>().SetExplosion(projStats.Damage, projStats.BlastRadius);
    Destroy(gameObject);

    //blown = true;
  }
}


