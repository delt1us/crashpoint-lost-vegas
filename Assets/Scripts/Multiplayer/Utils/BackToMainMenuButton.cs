//*************************************************************************************************************
/*  Back to main menu button
 *  A script to send the player to the main menu
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenuButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("NewMainMenu");
    }
}
