//*************************************************************************************************************
/*  Connect button
 *  A script to initiate matchmaking
 *  No longer used
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      
 */
//*************************************************************************************************************

using UnityEngine;

public class ConnectButton : MonoBehaviour
{
    [SerializeField] private ConnectedBooleanScriptableObject connectedBooleanScriptableObject;

    [SerializeField] private GameObject carSelectGameObject;

    private void OnEnable()
    {
        connectedBooleanScriptableObject.ConnectionEstablishedEvent += Disable;
    }

    private void OnDisable()
    {
        connectedBooleanScriptableObject.ConnectionEstablishedEvent -= Disable;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        carSelectGameObject.SetActive(true);
    }
}
