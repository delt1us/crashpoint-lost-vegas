using UnityEngine;
using TMPro;
//c
// Summary 
// Attached To GameObjects - [ KillCounter ] 
// Purpose -                 [ Increases the Kill Count ] 
// Functions -               [ 1. Updates the Kill Counter ]
// Dependencies -            [ HealthManager ]
// Notes - 
public class KillCounter : MonoBehaviour
{
    // Public Fields 
    public TextMeshProUGUI killCount;

    // Private Fields
    private int KillCount = 0;

    private void Start()
    {
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        killCount.text = "Kills: " + KillCount.ToString();
    }

    public void IncreaseKillCount()
    {
        KillCount++;
        UpdateKillCountText();
    }
}
