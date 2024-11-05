/**************************************************************************************************************
* Gun animator
* Used to make the barrels of the duel/quad-mounted guns spin whenever the player is aiming or shooting. 
* 
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

public class GunAnimator : MonoBehaviour
{
    [SerializeField] private Transform[] rotators; 

    [SerializeField, Range(10, 1000)] private float maxSpinRate = 1;

    private float spinRate;

    private bool active; 
    private bool aiming;
    private bool shooting;

    private void Update()
    {
        // Since the rotator can sometimes move whilst rotating... (?)
        foreach (Transform t in rotators) t.localPosition = new(-4.6f, 5, -6.48f);

        StopAnimation();
        BarrelSpin();
    }

    private void BarrelSpin()
    {
        // The barrel should spin whenever the player is aiming or shooting...
        active = aiming || shooting;
        if (!active) return;

        foreach(Transform animator in rotators) animator.RotateAround(animator.position, animator.up, spinRate * 10 * Time.deltaTime); 
    }

    public void StopAnimation()
    {
        if (active) return;

        spinRate = 0;
        active = false;
    }

    public void SetAiming(bool aiming)
    {
        this.aiming = aiming;
        spinRate = aiming ? maxSpinRate : spinRate;
    }

    public void SetShooting(bool shooting)
    {
        this.shooting = shooting;
        spinRate = shooting ? maxSpinRate : spinRate;
    }
}
