using UnityEngine;

public class RotateStand : MonoBehaviour
{
    public CarSelector carSelector;
    private void FixedUpdate()
    {
        carSelector.activeCar.transform.GetChild(0).Rotate(0, 0.5f, 0);
    }
}
