using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HuntedWinScreen : MonoBehaviour
{
    public TextMeshProUGUI silverTeamScoreText;
    public TextMeshProUGUI goldTeamScoreText;



    public HuntedScore scoreSlider;

    private void Update()
    {
        UpdateTeamScores();
    }

    public void UpdateTeamScores()
    {
        silverTeamScoreText.text = Mathf.RoundToInt(scoreSlider.scoreSilver).ToString();
        goldTeamScoreText.text = Mathf.RoundToInt(scoreSlider.scoreGold).ToString();
    }

    public void Exit()
    {
        UITransionManager.leftMatch = true;
        SceneManager.LoadScene("Main Menu");
        Debug.Log("Menu");
    }
}
