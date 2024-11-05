using UnityEngine;
using UnityEngine.UI;
using TMPro;

//c
// Summary 
// Attached To GameObjects - [ S ] 
// Purpose -                 [  ] 
// Functions -               [ 1.  ]
// Dependencies -            [ None ]
// Notes - 
public class KingOfTheHillScore : MonoBehaviour
{
    // Public Fields
    public Slider sliderTeamP;
    public Slider sliderTeamG;
    public GameObject winScreen;

    public int scoreTeamP;
    public int scoreTeamG;
    public int maxScore = 100;

    public GameObject purpleVictory;
    public GameObject greenVictory;

    public TextMeshProUGUI teamPScoreText;
    public TextMeshProUGUI teamGScoreText;

    void Start()
    {
        scoreTeamP = 0;
        scoreTeamG = 0;
        UpdateSlider(sliderTeamP, scoreTeamP);
        UpdateSlider(sliderTeamG, scoreTeamG);
        winScreen.gameObject.SetActive(false);
    }
    public void IncreaseScore(int ammount, ScoreManager.TeamType teamType)
    {
        if (teamType == ScoreManager.TeamType.TeamP)
        {
            scoreTeamP += ammount;
            scoreTeamP = Mathf.Clamp(scoreTeamP, 0, maxScore);
            UpdateSlider(sliderTeamP, scoreTeamP);

            if (scoreTeamP >= maxScore)
            {
                ShowWinScreen(ScoreManager.TeamType.TeamP);
            }
        }
        else if (teamType == ScoreManager.TeamType.TeamG)
        {
            scoreTeamG += ammount;
            scoreTeamG = Mathf.Clamp(scoreTeamG, 0, maxScore);
            UpdateSlider(sliderTeamG, scoreTeamG);

            if (scoreTeamG >= maxScore)
            {
                ShowWinScreen(ScoreManager.TeamType.TeamG);
            }
        }
        UpdateTeamScores();
    }

    public void UpdateTeamScores()
    {
        teamPScoreText.text = Mathf.RoundToInt(scoreTeamP).ToString();
        teamGScoreText.text = Mathf.RoundToInt(scoreTeamG).ToString();
    }

    private void UpdateSlider(Slider slider, int score)
    {
        float normalizedScore = (float)score / maxScore;
        slider.value = normalizedScore;
        Debug.Log("Update" + normalizedScore);
    }
    private void ShowWinScreen(ScoreManager.TeamType winningTeam)
    {
        winScreen.gameObject.SetActive(true);
        Time.timeScale = 0.0f;

        if (winningTeam == ScoreManager.TeamType.TeamP)
        {
            purpleVictory.SetActive(true);
            greenVictory.SetActive(false);
        }
        else if (winningTeam == ScoreManager.TeamType.TeamG)
        {
            purpleVictory.SetActive(false);
            greenVictory.SetActive(true);
        }
    }
}


