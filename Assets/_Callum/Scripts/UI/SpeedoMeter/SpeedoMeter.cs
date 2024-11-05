using UnityEngine;
using TMPro;
//c
// Summary 
// Attached To GameObjects - [ SpeedoMeter ] 
// Purpose -                 [ Makes a Funcioning Speed Meter ] 
// Functions -               [ 1. Increases the Angle of the Pointer Depending on the Speed of the Player ]
//                           [ 2. Display the Speed as a Number to Visulise the Speed Better ]            
// Dependencies -            [ None ]
// Notes - 
public class SpeedoMeter : MonoBehaviour
{
    // Public Fields
    public float minSpeedPointerAngle;
    public float maxSpeedPointerAngle;
    public float maxSpeed;
    
    public TextMeshProUGUI    speedText;
    public RectTransform      pointer;
    public MovementController movementContoller;
    private void Update()
    {
        // Gets the current speed, rounds it and displays it as a string
        float speed    =  movementContoller.GetSpeed();
        int roundSpeed =  Mathf.RoundToInt(speed);
        speedText.text =  roundSpeed.ToString();
        
        // Cacluates the angle based on the current speed adjusting the pointers rotation
        float angle = Mathf.Lerp(minSpeedPointerAngle, maxSpeedPointerAngle, speed / maxSpeed);
        pointer.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
}

    
