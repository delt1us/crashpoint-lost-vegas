//*************************************************************************************************************
/*  Map destroy disabler
 *  Needed for the new map load system
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *  
 */
//*************************************************************************************************************

using UnityEngine;

public class MapDestroyDisabler : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
