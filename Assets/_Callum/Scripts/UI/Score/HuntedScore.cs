using UnityEngine;
using UnityEngine.UI;
using TMPro;

//c
// Summary 
// Attached To GameObjects - [  ] 
// Purpose -                 [  ] 
// Functions -               [ 1.  ]
// Dependencies -            [ None ]
// Notes - 
public class HuntedScore : MonoBehaviour
{
    public Slider silverTeamSlider;
    public Slider goldTeamSlider;

    public GameObject winScreen;

    public GameObject silverVictory;
    public GameObject goldVictory;

    public int scoreSilver;
    public int scoreGold;
    public int maxScore;
   

    public TextMeshProUGUI silverTeamText;
    public TextMeshProUGUI goldTeamText;

    public enum TeamType
    {
        SilverTeam,
        GoldTeam
    }
    public TeamType teamType;

    private void Start()
    {
        scoreSilver = 0;
        scoreGold = 0;
        UpdateSlider(silverTeamSlider, scoreSilver);
        UpdateSlider(goldTeamSlider, scoreGold);
        winScreen.gameObject.SetActive(false);
    }
    public void IncreaseScore(TeamType teamType)
    {
        if (teamType == TeamType.SilverTeam)
        {
            scoreSilver += 1;
            UpdateSlider(silverTeamSlider, scoreSilver);
            silverTeamText.text = silverTeamSlider.value.ToString();

            if (scoreSilver >= maxScore)
            {
                ShowWinScreen(TeamType.SilverTeam);
            }
        }
        else if (teamType == TeamType.GoldTeam)
        {
           scoreGold += 1;
            UpdateSlider(goldTeamSlider, scoreGold);
            goldTeamText.text = goldTeamSlider.value.ToString();

            if (scoreGold >= maxScore)
            {
                ShowWinScreen(TeamType.GoldTeam);
            }
        }
        UpdateTeamScores();
    }

    public void UpdateTeamScores()
    {
        silverTeamText.text = Mathf.RoundToInt(scoreSilver).ToString();
        goldTeamText.text = Mathf.RoundToInt(scoreGold).ToString();
    }

    private void UpdateSlider(Slider slider, int score)
    {
        float normalizedScore = (float)score / maxScore;
        slider.value = normalizedScore;
        Debug.Log("Update for Hunted" + normalizedScore);
    }

    private void ShowWinScreen(TeamType winningTeam)
    {
        winScreen.gameObject.SetActive(true);
        Time.timeScale = 0.0f;

        if (winningTeam == TeamType.SilverTeam)
        {
            silverVictory.SetActive(true);
            goldVictory.SetActive(false);
        }
        else if (winningTeam == TeamType.GoldTeam)
        {
            silverVictory.SetActive(false);
            goldVictory.SetActive(true);
        }
    }
}


