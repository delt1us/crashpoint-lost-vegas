using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ CarSpawner ] 
// Purpose -                 [ Spawns the Veichle Into the Scene ] 
// Functions -               [ 1. Gets the car That was Selected From PlayerPrefs and Instantiates it Into the Scene ]
// Dependencies -            [ None ]
// Notes -  Need to Give the car a Specific Spawn Point in the Scene [ Done ]
public class CarSpawner : MonoBehaviour
{
    // Public Fields
    public int index;
    public GameObject[] cars;
    public Transform spawnLocation;

    public void Start()
    { 
        index = PlayerPrefs.GetInt("carIndex");
        //GameObject car = Instantiate(cars[index], spawnLocation.position, spawnLocation.rotation);
    }
}

