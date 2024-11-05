/**************************************************************************************************************
* Quick turn
* Used in the Truck prefab. It allows the player to perform sharp turns by applying forces to the ends of the truck. This also manages its availability.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

public class QuickTurn : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform counterLeft;
    [SerializeField] private Transform counterRight;
    private Rigidbody carBody;

    [Header("Ability")]
    [SerializeField, Range(1, 100), Tooltip("More controllable when Turn Force > Counter Force.")]
    private float turnForce = 15;

    [SerializeField, Range(1, 100), Tooltip("More controllable when Counter Force < Turn Force.")] 
    private float counterForce = 10;

    [SerializeField] private byte by_maxCharges = 4;
    private byte by_charges;
    private Vector3 direction;

    [Header("Recharging")]
    [SerializeField, Tooltip("The time in seconds it takes to regain one box."), Range(1, 60)]
    private float chargeDelay = 20;
    private float chargeTimer;

    private InputManager inputManager;


    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        carBody = GetComponent<Rigidbody>();

        by_charges = by_maxCharges;

        // To make the values more readable in the inspector
        turnForce *= 100;
        counterForce *= 100;
    }

    private void Update()
    {
        ChargeRegen();
    }

    private void OnMovementAbility()
    {
        if (by_charges < 1) return;

        bool pushFromRight = CalculateTurn();
        Vector3 counterDirection = turnForce * (pushFromRight? counterRight.right: counterLeft.right);

        // Don't do anything if there isn't a steer input.
        if (direction == Vector3.zero) return;
        
        // If the truck isn't on the floor... don't do anything..
        if(!GetComponent<MovementController>().IsGrounded()) return;

        carBody.AddForceAtPosition(direction, pushFromRight ? rightPoint.position : leftPoint.position, ForceMode.Force);
        carBody.AddForceAtPosition(counterDirection * counterForce, !pushFromRight ? counterRight.position : counterLeft.position, ForceMode.Force);

        by_charges--;
    }

    private bool CalculateTurn()
    {
        // Always reset the direction
        direction = Vector3.zero;

        float input = inputManager.SteerAction.ReadValue<float>();

        // A direction input has to be given otherwise it won't do anything.
        if (input == 0) return false;
           
        // If left isn't true... the direction is right.
        bool pushFromRight = input > 0;

        // Since all of the points would have the similar rotations - use the transform of one of the points.
        direction = (pushFromRight? rightPoint.right: leftPoint.right) * turnForce;

        return pushFromRight;
    }

    private void ChargeRegen()
    {
        // Continuously do unless the charges are full.
        if (by_charges == by_maxCharges) return;

        chargeTimer += Time.deltaTime;

        // When the timer elapses....
        if (chargeTimer > chargeDelay)
        {
            by_charges++;
            chargeTimer = 0;
        }
    }





    ////////////////////////////////////////// DEBUGGING ////////////////////////////////////////// 


    public byte GetCharges()
    {
        return by_charges;
    }

    public float GetChargeDelay()
    {
        return chargeDelay;
    }

    public float GetChargeTime()
    {
        if (by_charges > 0) return 0;

        return chargeTimer;
    }
}
