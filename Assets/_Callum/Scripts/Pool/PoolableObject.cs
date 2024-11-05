using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ DamageText ] 
// Purpose -                 [ Returns GameObjects Back to the Pool ] 
// Functions -               [ 1. Checks if the GameObject is Disabled Then Returns it to the Pool ]
// Dependencies -            [ None ]
// Notes -  Currently Does not Return Gameobjects Back to the Pool After They Have Been Disabled
public class PoolableObject : MonoBehaviour
{
    // Public Fields
    public ObjectPool parent;

    public virtual void OnDisable()
    {
        Debug.Log("OnDisabled Called");

            parent.ReturnObjectsToPool(this);
    }
}
