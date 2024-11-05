//*************************************************************************************************************
/*  GameObects to enable
 *  A script to hold 2 lists
 *  It is attached to each car prefab so you can add components and gameobject to re enable when the car is
 *      spawned client side.
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 09/08/23 Made this file to allow easier addition of other cars to multiplayer
 */
//*************************************************************************************************************

using UnityEngine;

public class GameObjectsToEnable : MonoBehaviour
{
    public GameObject[] gameObjectsList;
    public Behaviour[] behavioursList;
}