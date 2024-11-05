using System.Collections.Generic;
using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ None ] 
// Purpose -                 [ Pools GameObjects to be Used ] 
// Functions -               [ 1. Creates a Instance of the Object Pool ]
//                           [ 2. Initilses the Objects in Preperation to be Used ]
//                           [ 3. Retruns Used GameObjects Back Into the Pool ]
// Dependencies -            [ None ]
// Notes - 
public class ObjectPool 
{
    // Private Fields
    private PoolableObject prefab;
    private List<PoolableObject> avalableObjectsPool;
    private int size;
    public Transform damageTextContainer;
   
    private ObjectPool(PoolableObject prefab, int size)
    {
        this.prefab = prefab;
        this.size   = size;
       

        // Initilizes a list with a specific capacity
        avalableObjectsPool = new List<PoolableObject>(size);
    }

    // Creates a instance of the object pool
    public static ObjectPool CreateInstance(PoolableObject prefab, int size)
    {
        ObjectPool pool = new ObjectPool(prefab, size);
        GameObject poolGameObject = new GameObject(prefab.name + "pool");
        pool.CreateObjects(poolGameObject);
        

        return pool;
    }

    // Creates the objects and initilizes them for the pool 
    private void CreateObjects(GameObject parent)
    {
        for (int i = 0; i < size; i++)
        {
            PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.parent = this;
            poolableObject.gameObject.SetActive(false);
        }
    }

    // Returns objects back to the pool
    public void ReturnObjectsToPool(PoolableObject poolableObject)
    {
        avalableObjectsPool.Add(poolableObject);
    }

    // Retrives a avaliable object from the pool
    public PoolableObject GetObject()
    {
        
            PoolableObject instance = avalableObjectsPool[0];
            avalableObjectsPool.RemoveAt(0);
            instance.gameObject.SetActive(true);
            return instance;
    }
}
