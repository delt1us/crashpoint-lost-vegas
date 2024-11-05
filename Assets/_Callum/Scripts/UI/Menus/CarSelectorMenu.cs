using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelectorMenu : MonoBehaviour
{
    [SerializeField] private AudioSource ButtonSFX;

    public void Back()
    {
        // SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        // ButtonSFX.Play();
    }

    public void Play()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Match");
        // ButtonSFX.Play();
    }

    public void Customise()
    {
        SceneManager.LoadScene("CustomiseCar");
    }
}
