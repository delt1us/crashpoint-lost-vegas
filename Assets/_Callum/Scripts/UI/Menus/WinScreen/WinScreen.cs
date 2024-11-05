using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI teamPScoreText;
    public TextMeshProUGUI teamGScoreText;



    public KingOfTheHillScore scoreSlider;

    private void Update()
    {
        UpdateTeamScores();
    }

    public void UpdateTeamScores()
    {
        teamPScoreText.text = Mathf.RoundToInt(scoreSlider.scoreTeamP).ToString();
        teamGScoreText.text = Mathf.RoundToInt(scoreSlider.scoreTeamG).ToString();
    }

    public void Exit()
    {
        UITransionManager.leftMatch = true;
        SceneManager.LoadScene("Main Menu");
        Debug.Log("Menu");
    }
}
