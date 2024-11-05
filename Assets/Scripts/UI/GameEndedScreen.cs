//*************************************************************************************************************
/*  Game ended screen
 *  A script to control the game ended screen
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndedScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GameObject.Find("Map GameObject"));
    }
}
