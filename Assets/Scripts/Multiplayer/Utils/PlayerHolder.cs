//*************************************************************************************************************
/*  Player Holder
 *  A script used to DontDestroyOnLoad the player holder.
 *      The player holder is simply a gameobject that holds all the players
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 28/07/23 - Player holder made
 */
//*************************************************************************************************************

using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
