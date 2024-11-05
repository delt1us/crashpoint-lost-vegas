using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSrc;
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip openMusic;

    private const string mainMenu = "MAINMENU"; 
    private const string game = "GAME"; 
    private const string opener = "OPENER";

    [SerializeField] private AnimationCurve fadeCurve;
    private float maxVolume = .55f;
    private bool fadeOn;
    private bool fadeOut;

    private bool inMainMenu;

    private void Start()
    {
        musicSrc = GetComponent<AudioSource>();
        musicSrc.volume = 0;

        // Only the main menu camera has the tag MainCamera.
        inMainMenu = GetComponentInParent<Camera>().CompareTag("MainCamera");
        if (inMainMenu) PlaySong("mainMenu", false);
    }

    private void Update()
    {
        FadeIn();
        FadeOut();
    }

    public void PlaySong(string song, bool fade)
    {
        song = song.ToUpper();

        switch(song)
        {
            case mainMenu:
                PlayMenuMusic(fade);
                break;

            case game:
                PlayGameMusic(fade);
                break;

            case opener:
                PlayOpenMusic(fade);
                break;
        }

        if (!fade) musicSrc.volume = maxVolume;
    }

    private void PlayGameMusic(bool fade)
    {
        musicSrc.clip = gameMusic;
        musicSrc.Play();
        fadeOn = fade;
    }

    private void PlayMenuMusic(bool fade)
    {
        musicSrc.clip = mainMusic;
        musicSrc.Play();
        fadeOn = fade;
    }

    private void PlayOpenMusic(bool fade)
    {
        musicSrc.clip = openMusic;
        musicSrc.Play();
        fadeOn = fade;
    }

    private void StopMusic(bool fade)
    {
        fadeOut = true;
        fadeOn = fade;
    }

    private void FadeIn()
    {
        if (!(fadeOn && !fadeOut)) return;

        musicSrc.volume += Time.deltaTime * fadeCurve.Evaluate(musicSrc.volume); ;
        musicSrc.volume = Mathf.Clamp(musicSrc.volume, 0, maxVolume);

        if (musicSrc.volume >= maxVolume)
        {
            musicSrc.volume = maxVolume;
            fadeOn = false;
        }
    }

    private void FadeOut()
    {
        if (!(fadeOn && fadeOut)) return;

        musicSrc.volume -= Time.deltaTime * fadeCurve.Evaluate(musicSrc.volume); ;
        musicSrc.volume = Mathf.Clamp(musicSrc.volume, 0, maxVolume);

        if (musicSrc.volume <= 0)
        {
            musicSrc.volume = 0;
            fadeOn = false;
            musicSrc.Stop();
        }
    }
}
