//*************************************************************************************************************
/*  Score values scriptable object
 *  A scriptable object used to score data about the game modes
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

[CreateAssetMenu(fileName = "ScoreValuesScriptableObject", menuName = "Scriptable Objects/Multiplayer/Score values")]
public class ScoreValuesScriptableObject : ScriptableObject
{
    public int maxScoreHunted;
    public int maxScoreKotH;
    public int scoreForKillingHunted;
    public int scoreForKillingNormal;
    public int scorePerSecondKotH;
    public int scorePerSecondKotHContested;
    public int scoreForKillingKotH;
}
