using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ PointToFlagImage ] 
// Purpose -                 [ Points to The Teams Flag Location ] 
// Functions -               [ 1. Points to the Flag Location ]                         
// Dependencies -            [ None ]
// Notes - Their is a issue where the pointer is not always accurate to the flags location
public class Pointer : MonoBehaviour
{
    // Public Fields
    public Transform target;
    public Transform car;

    // Private Fields
    private RectTransform rectTransform;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    
    void Update()
    {
        // Finds the Position of the target and Rotates the pointer based on it
        if (car == null || target == null)
            return;

        Vector3 toTarget = target.position - car.position;
        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
