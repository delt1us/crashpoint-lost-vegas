//*************************************************************************************************************
/*  Main Scene Loader
 *  A simple script to load the map additively
 *  This is a huge optimization because it means you load the map at the start and never again, saving a lot of time
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *
 */
//*************************************************************************************************************

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneLoader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }
}
