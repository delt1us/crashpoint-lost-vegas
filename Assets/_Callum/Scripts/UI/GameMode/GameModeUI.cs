using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ GameMode ] 
// Purpose -                 [ Displays the GameMode Name/Rules ] 
// Functions -               [ 1. Disaplys the GameMode GameObject ]
//                           [ 2. Hides it After a Certain Ammount of TIme ]
// Dependencies -            [ None ]
// Notes - 
public class GameModeUI : MonoBehaviour
{
    //public GameObject GameMode;
    public float displayDuration = 3f;

    public GameObject kingOfTheHill;
    public GameObject hunted;

    public KingOfTheHillScore kingOfTheHillScore;
    public HuntedScore huntedScore;

    public KingofTheHillWinScreen kingofTheHillWinScreen;
    public HuntedWinScreen huntedWinScreen;
    
    public enum GameModeType { KingoftheHill, Hunted }
    public GameModeType gameMode;
    void Start()
    {
        kingOfTheHill.SetActive(false);
        hunted.SetActive(false);
        kingOfTheHillScore.gameObject.SetActive(false);
        huntedScore.gameObject.SetActive(false);
        kingofTheHillWinScreen.gameObject.SetActive(false);
        huntedWinScreen.gameObject.SetActive(false);

        //if (GameMode != null)
        // {
        //     GameMode.SetActive(true);
        //     Invoke("HideGameMode", displayDuration);
        // }

        switch (gameMode)
        {
            case GameModeType.KingoftheHill:
                kingOfTheHill.SetActive(true);
                kingOfTheHillScore.gameObject.SetActive(true);
                kingofTheHillWinScreen.gameObject.SetActive(true);
                break;
            case GameModeType.Hunted:
                hunted.SetActive(true);
                huntedScore.gameObject.SetActive(true);
                huntedWinScreen.gameObject.SetActive(true);
                break;
        }
        Invoke("HideGameMode", displayDuration);

    }
    private void HideGameMode()
    {
        // if (GameMode != null)
        // {
        //     GameMode.SetActive(false);
        // }

        switch (gameMode)
        {
            case GameModeType.KingoftheHill:
                kingOfTheHill.SetActive(false);
                
                break;
            case GameModeType.Hunted:
                hunted.SetActive(false);
                
                break;
        }
    }
}
