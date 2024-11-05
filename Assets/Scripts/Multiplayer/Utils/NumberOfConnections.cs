//*************************************************************************************************************
/*  Number of Connections
 *  This is used to update the number of connections text in the loading screen
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 28/07/23 - Temporary connection screen works
 *      Armin - 05/08/23 - Added support for restarting a game
 */
//*************************************************************************************************************

using TMPro;
using UnityEngine;

public class NumberOfConnections : MonoBehaviour
{
    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    private TMP_Text _tmpText;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
        UpdateText();
    }

    private void OnEnable()
    {
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent += UpdateText;
    }

    private void OnDisable()
    {
        numberOfConnectionsScriptableObject.NewPlayerConnectedEvent -= UpdateText;
    }

    private void UpdateText()
    {
        _tmpText.text = $"Connected Players: {numberOfConnectionsScriptableObject.Connections}";
    }
}
