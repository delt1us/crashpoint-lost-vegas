using UnityEngine;
using UnityEngine.UI;
//c
// Summary 
// Attached To GameObjects - [ S ] 
// Purpose -                 [  ] 
// Functions -               [ 1.  ]
// Dependencies -            [ None ]
// Notes -  Need to Give the car a Specific Spawn Point in the Scene [ Done ]
public class ScoreSlider : MonoBehaviour
{
    // Public Fields
    public Slider slider;
    // public GameObject winScreen;

    public int score;

    [SerializeField] private TeamScoreScriptableObject teamScoreScriptableObject;
    
    void Start()
    {
        score = 0;
        UpdateSlider();
        // winScreen.SetActive(false);

        if(teamScoreScriptableObject) teamScoreScriptableObject.ScoreUpdatedEvent += _UpdateScore;
    }

    private void _UpdateScore()
    {
        score = teamScoreScriptableObject.Score;
        UpdateSlider();
    }
    
    // public void IncreaseScore(int ammount)
    // {
    //     score += ammount;
    //     UpdateSlider();
    //
    //     if (score >= maxScore)
    //     {
    //         ShowWinScreen();
    //     }
    // }
    private void UpdateSlider()
    {
        if (!teamScoreScriptableObject) return;

        float normalizedScore = (float)score / teamScoreScriptableObject.maxScore;
        slider.value = normalizedScore;
    }
    // private void ShowWinScreen()
    // {
    //     winScreen.SetActive(true);
    // }
}
