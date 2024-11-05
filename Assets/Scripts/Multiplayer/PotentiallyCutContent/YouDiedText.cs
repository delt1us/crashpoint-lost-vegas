//*************************************************************************************************************
/*  You Died Text
 *  This is an old placeholder script used to show that the player was dead. It is no longer used
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - unknown date - Made temporary death screen
 *      Armin - unknown date - Made new respawn UX and removed this one
 */
//*************************************************************************************************************

using TMPro;
using UnityEngine;

public class YouDiedText : MonoBehaviour
{
    [SerializeField] private HealthScriptableObject healthScriptableObject;
    private TMP_Text _thisTMPText;
    
    private void Start()
    {
        _thisTMPText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        healthScriptableObject.DiedEvent += _Show;
    }

    private void OnDisable()
    {
        healthScriptableObject.DiedEvent -= _Show;
    }

    private void _Show()
    {
        _thisTMPText.enabled = true;
    }

    // Not sure when this would be useful but hey 
    private void _Hide()
    {
        _thisTMPText.enabled = false;
    }
}
