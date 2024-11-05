using System.Collections;
using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ PointP, PointG ] 
// Purpose -                 [ Increases the Slider Depending on the Score ] 
// Functions -               [ 1. Checks for Collision With the Player ]
//                           [ 2. If the Player is Touching the Point the Score Will Keep Increasing ]
//                           [ 3. Updates the Slider Depedning on the Current Score ]
// Dependencies -            [ None ]
// Notes -  Need to Make it Team Specific 
// CLASS NAME NEEDS TO BE CHANGED TO POINTSSCORE
public class ScoreManager : MonoBehaviour
{
    
    // Public Fields
    public KingOfTheHillScore scoreSlider;

    // Private Fields
    private int score = 0;
    private bool isTouching = false;

    public enum TeamType {TeamP, TeamG }
    public TeamType teamType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouching = true;
            StartCoroutine(KeepIncreasingScore());
            Debug.Log("Player On Point");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouching = false;
        }
    }

    private IEnumerator KeepIncreasingScore()
    {
        while (isTouching)
        {
            IncreaseScore(1);
            yield return new WaitForSeconds(1f);
        }
    }

    private void IncreaseScore(int ammount)
    {
        score += ammount;
        scoreSlider.IncreaseScore(ammount, teamType);
        
    }
}

    
