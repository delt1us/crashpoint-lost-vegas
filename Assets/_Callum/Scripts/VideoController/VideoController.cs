using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
//c
// Summary 
// Attached To GameObjects - [ VideoManager ] 
// Purpose -                 [ Controls the Intro Video ] 
// Functions -               [ 1.  ]
// Dependencies -            [ None ]
// Notes - Need to add in a Fade Black Screen to Hide the Flicker
public class VideoController : MonoBehaviour
{
    // Public Fields
    public VideoPlayer titleScreen;
    public VideoPlayer blackScreenVideo;
    public GameObject  blackScreen;
    
   // Private Fields
    private bool hasPressed = false;

    void Start()
    {
        titleScreen.loopPointReached += TitleFinished;
        titleScreen.Play();
    }
    void Update()
    {
        CheckMouseClick();
        CheckIfTitleOver();
    }
    private void TitleFinished(VideoPlayer vp)
    {
        hasPressed = true;
    }
    private void PlayBlackScreen()
    {
        titleScreen.gameObject.SetActive(false);
        blackScreen.SetActive(true);

        blackScreenVideo.Play();
    }
    private void CheckMouseClick()
    {
        if (hasPressed && !blackScreenVideo.isPlaying && Input.GetMouseButtonUp(0))
        {
            PlayBlackScreen();
        }
    }
   private void CheckIfTitleOver()
    {
        if (!blackScreenVideo.isPlaying && blackScreen.activeSelf)
        {
            MoveToMainMenu();
        }
    }
    private void MoveToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
   
}
