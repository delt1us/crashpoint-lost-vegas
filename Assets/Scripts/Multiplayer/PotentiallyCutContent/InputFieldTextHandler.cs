//*************************************************************************************************************
/*  Input Field Text Handler
 *  A script to handle the text from input fields and to give them to the database manager
 *  Cut content as the database was removed from the scope of the project
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 05/07/23 - Brought over to the main git repo
 *      Armin - 28/07/23 - Added to cut content folder
 */
//*************************************************************************************************************

using TMPro;
using UnityEngine;

// This is a class to have preview text in the textbox whenever it is not selected
public class InputFieldTextHandler : MonoBehaviour
{
    private TMP_InputField _inputField;

    private string _startingText;
    // Start is called before the first frame update
    void Start()
    {
        _inputField = transform.GetComponent<TMP_InputField>();
        _startingText = _inputField.text;
    }

    // Removes preview text
    public void OnSelected()
    {
        _inputField.text = "";
    }

    // Brings preview text back if text field was left empty
    public void OnDeselected()
    {
        if (_inputField.text != "") return;
        _inputField.text = _startingText;
    }
}
