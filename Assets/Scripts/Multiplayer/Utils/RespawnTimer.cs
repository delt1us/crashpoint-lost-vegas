//*************************************************************************************************************
/*  Respawn Timer
 *  This is a file that updates the UI element of the respawn timer
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 03/08/23 - Player respawning added
 *      Armin - 08/08/23 - Fixed respawn timer
 */
//*************************************************************************************************************

using TMPro;
using UnityEngine;

public class RespawnTimer : MonoBehaviour
{
    [SerializeField] private RespawnTimerScriptableObject respawnTimerScriptableObject;
    [SerializeField] private TMP_Text tmpText;
    
    private void OnEnable()
    {
        respawnTimerScriptableObject.TimerChangedEvent += _UpdateTmpText;
        respawnTimerScriptableObject.TimerStartedEvent += _EnableTmpText;
        respawnTimerScriptableObject.TimerFinishedEvent += _DisableTmpText;
    }

    private void OnDisable()
    {
        respawnTimerScriptableObject.TimerChangedEvent -= _UpdateTmpText;
        respawnTimerScriptableObject.TimerStartedEvent -= _EnableTmpText;
        respawnTimerScriptableObject.TimerFinishedEvent -= _DisableTmpText;
    }

    private void _EnableTmpText()
    {
        tmpText.enabled = true;        
    }

    private void _DisableTmpText()
    {
        tmpText.enabled = false;
    }
    
    private void _UpdateTmpText()
    {
        tmpText.text = $"Respawning in {respawnTimerScriptableObject.CurrentTime.ToString()} seconds";
    }
}
