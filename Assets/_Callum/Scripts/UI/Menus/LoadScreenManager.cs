/**************************************************************************************************************
* Loadingscreen Manager
* Used in the loading screen to create the illusion of an actual loading screen. Gives players a chance to read tips on the game and see art
* which explains the game modes.
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenManager : MonoBehaviour
{
    private Slider loadBar;

    private readonly string[] tips = new string[] 
    {
        "Braking whilst toppled over speeds up the flip timer.",
        "You can Air-roll to do barrel rolls in the air.",
        "You can only Drift-boost while the back wheels are on the floor.",
        "Swapping to your secondary weapon speeds up weapon cooling.",
        "The flamethrower and ice-thrower deal damage over time.",
        "All cars have aerial movement - including an air-roll.",
        "The Delivery van's SpeedPacks can be obtained bu anyone",
        "The Hookshot can be used to deal damage to enemies or traverse the Lost Vegas.",
        "Doc's hover ability can be used to evade incoming fire.",
        "Look for different ways to enter the golf course from the city",
        "Someone chasing you? Drop your defensive ability to slow them down"
    };

    private TextMeshProUGUI tipTxtBox;

    [SerializeField] private Sprite[] loadingImgs;
    private Image imgComponenet;

    private bool loading;
    [SerializeField] private float loadSpeed = .85f;
    [SerializeField, Tooltip("The chance of the load bar pausing... (Higher number = Lower chance)")] 
    private int pauseChance = 25;

    [SerializeField] private float minPauseTime = .25f;
    [SerializeField] private float maxPauseTime = 2.1f;

    private bool pauseTimerOn;
    private float pauseTimer;
    private float pauseCooldown;

    private void Start()
    {
        loadBar = GetComponentInChildren<Slider>();
        imgComponenet = GetComponentInChildren<Image>();
        tipTxtBox = GetComponentInChildren<TextMeshProUGUI>();

        MakeScreen();
    }

    private void Update()
    {
        MoveBar();
        FreezeTimer();
        RandomPause();
    }

    private Sprite GetRandomImg()
    {
        byte rndNum = (byte)Random.Range(0, loadingImgs.Length);
        return loadingImgs[rndNum];
    }

    private string GetRandomTip()
    {
        byte rndNum = (byte)Random.Range(0, tips.Length);
        return tips[rndNum];
    }

    private void MakeScreen()
    {
        // Initiates the movement of the load bar
        loading = true;

        imgComponenet.sprite = GetRandomImg();
        tipTxtBox.text = GetRandomTip();

        pauseCooldown = Random.Range(minPauseTime, maxPauseTime);
        loadBar.value = 0;
    }

    private void MoveBar()
    {
        if (!loading) return;


        float amount = Random.Range(.005f, .5f);
        loadBar.value += amount * Time.deltaTime * loadSpeed;

        // if (loadBar.value >= 1)
        // {
        //     SceneManager.LoadSceneAsync(2);
        //     SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //     loading = false;
        //     return;
        // }
    }


    private void FreezeTimer()
    {
        if (loadBar.value > .6f || !pauseTimerOn) return;

        loading = false;

        pauseTimer += Time.deltaTime;
        if (pauseTimer > pauseCooldown)
        {
            pauseTimer = 0;
            loading = true;

            // Get a new pause cooldown after every elapse
            pauseCooldown = Random.Range(minPauseTime, maxPauseTime);

            pauseTimerOn = false;
        }
    }

    private void RandomPause()
    {
        // Don't pause if the load bar has almost finished.
        if (loadBar.value > .6f) return;

        int rndNum = Random.Range(0, pauseChance);
        bool pause = rndNum == pauseChance - 1;

        if(pause) pauseTimerOn = true;
    }

}
