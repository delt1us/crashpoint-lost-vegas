/**************************************************************************************************************
* Siren lights
* Used in the Siren prefab which is used for the Police car. It turns on one of the lights at a time and alternates between them whenever the timer elapses.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using UnityEngine;

public class SirenLights : MonoBehaviour
{
    [SerializeField] private Light red;
    [SerializeField] private Light blue;

    // Whether or not to turn on the red light (otherwise it would show the blue light when its active)
    private bool isRed;
    private bool on;

    private float timer;
    [SerializeField] private float strobeSpeed = .25f;

    private void Update()
    {
        FlipFlopLight();
    }

    private void FlipFlopLight()
    {
        if (!on) return;

        timer += Time.deltaTime;

        red.gameObject.SetActive(isRed);
        blue.gameObject.SetActive(!isRed);

        // When it elapses
        if (timer > strobeSpeed)
        {
            // Reset the timer and swap the isRed value
            timer = 0;
            isRed = !isRed;
        }
    }

    public void ToggleActive(bool active)
    {
        on = active;

        // Activate/Deactivate both objects depending on the given value.
        red.gameObject.SetActive(on);
        blue.gameObject.SetActive(on);
    }
}
