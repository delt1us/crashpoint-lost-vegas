//*************************************************************************************************************
/*  Login Button Handler
 *  Old file that was used to handle the login button
 *  Database is cut content, but it does work 
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 05/07/23 - Brought over to the main git repo
 *      Armin - 28/07/23 - Added to cut content folder
 */
//*************************************************************************************************************

using UnityEngine;
using TMPro;

public class LoginButtonHandler : MonoBehaviour
{    
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    
    [SerializeField]private DatabaseManager _databaseManager;

    // Start is called before the first frame update
    void Start()
    {
        _databaseManager = GameObject.Find("Database Manager").GetComponent<DatabaseManager>();
    }
    
    // Called from login button events
    public void OnLoginPressed()
    {
        string username, password;

        if (usernameField.text == "Username" || passwordField.text == "Password") return;

        username = usernameField.text;
        password = passwordField.text;
        
        Debug.Log("Got here");
        _databaseManager.AddPlayerToDatabaseServerRpc(username, password);
    }
}
