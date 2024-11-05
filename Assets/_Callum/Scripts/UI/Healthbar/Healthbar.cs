using UnityEngine;
using UnityEngine.UI;
//c
// Summary 
// Attached To GameObjects - [ HealthBarSlider ] [ EnemyHealthBarSlider ]
// Purpose -                 [ Updates the Healthbars ] 
// Functions -               [ 1. Updates the Slider Based On The GameObjects Health Vaule ]
//                           [ 2. If its Not a Player, The HealthBar is to Look at the Player ] 
//                           [ 3. Updates the Fill Colour Depending on Health Vaule ]
// Dependencies -            [ HealthManager ]
// Notes - Fixed issue where only some of the target cars will be assined the players position
public class Healthbar : MonoBehaviour
{
    // Public fields 
    public Slider    slider;    
    public Image     fill;
    private Transform carPosition;

    // Private fields 
    private bool isEnemy;

    // Serialized fields
    [SerializeField] private Gradient healthBarGradient;

    private GameObject player;

    private void Start()
    {
        // Find player car and set carPosition 
        player = GetComponentInParent<MovementController>().gameObject;
        if (player != null)
        {
            carPosition = player.transform;
        }
    }
    private void Update()
    {
        FindPlayer();

        if (isEnemy)
        {
            // Find the Player and make the healthbar look at them 
            carPosition = player.transform;
            
            LookAtPlayer();
        }
    }
    private void LookAtPlayer()
    {
        // Calculate the direction and the rotation to look at the player
        Vector3    direction  =   carPosition.position - transform.position;
        Quaternion rotation   =   Quaternion.LookRotation(direction);
        transform.rotation    =   rotation;
    }
    public void SetMaxHealth(int health)
    {
        // Sets the max health vaule and updates the fill colour accordingly 
        slider.maxValue  =  health;
        UpdateHealthBarColor();
    }
    public void SetHealth(int health)
    {
        // Sets the current health vaule and updates the fill colour accordingly   
        slider.value = health;
        UpdateHealthBarColor();
    }
    public void UpdateHealthBarColor()
    {
        // Updates the fill colour based on the normalized health vaule 
        fill.color = healthBarGradient.Evaluate(slider.normalizedValue);
    }

    public void FindPlayer()
    {
        isEnemy = transform.CompareTag("EnemyHealthBar");


        if (!isEnemy)
        {
            // Find player car and set carPosition 
            player = GetComponentInParent<MovementController>().gameObject;
            if (player != null)
            {
                carPosition = player.transform;
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                carPosition = player.transform;
            }
        }
    }
}
