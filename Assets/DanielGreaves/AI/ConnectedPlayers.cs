/**************************************************************************************************************
* AI_Nav_Movement_1
* 
* Created by Daniel Greaves 2023
* 
* Change Log:
* 
* Daniel - Must attach this to other player or ai so that the AI can give chace to this object. Will need to add tag (Target) in editor
* 
***************************************************************************************************************/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
  public class ConnectedPlayers: PlayersObject
  {
     [SerializeField] protected float Radius = 20.0f;



        List<ConnectedPlayers> connections;

        public void Start()
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Target");

            connections = new List<ConnectedPlayers>();


            for (int i = 0; i < Players.Length; i++)
            {

                if (Players[i].TryGetComponent<ConnectedPlayers>(out var NextPlayer))
                {
                    if (Vector3.Distance(this.transform.position, NextPlayer.transform.position) <= Radius && NextPlayer != this)
                    {
                        connections.Add(NextPlayer);
                    }
                }
            }

           // for (int i = 0; i < Players.Length; i++)
           // {

           //     ConnectedPlayers NextPlayer = Players[i].GetComponent<ConnectedPlayers>();


           //     if (NextPlayer == null)
            //    {
             //       if (Vector3.Distance(this.transform.position, NextPlayer.transform.position) <= Radius && NextPlayer != this)
              //      {
             //           connections.Add(NextPlayer);
             //       }

           //     }


            //}
        }
        
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
        public ConnectedPlayers NextWaypoint(ConnectedPlayers PrevoiusPlayers)
        {
            if(connections.Count == 0)
            {
                Debug.LogError(" No players to find :(");
                return null;

            }
            else if(connections.Count == 1 && connections.Contains(PrevoiusPlayers))
            {
                return PrevoiusPlayers;
            }
            else
            {
                ConnectedPlayers NextPlayer;
                int NextIndex = 0;

                do
                {
                    NextIndex = UnityEngine.Random.Range(0, connections.Count);
                   
                    NextPlayer = connections[NextIndex]; 
                }while(NextPlayer == PrevoiusPlayers); 

                return NextPlayer;
            }
        }
  }
}


