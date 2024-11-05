//*************************************************************************************************************
/*  Countdown Timer
 *  Class that counts down from a set number of seconds and handles all events for this
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 30/07/23 - Created file
 *      Armin - 31/07/23 - Timer works
 *      Armin - 01/08/23 - Timer works properly across the network
 */
//*************************************************************************************************************


using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountdownTimer : NetworkBehaviour
{
    public bool finished = false;
    
    [SerializeField] private TimerScriptableObject timerScriptableObject;
    [SerializeField] private TMP_Text tmpText;
    
    private NetworkVariable<int> _timerNetworkVariable = new NetworkVariable<int>();

    public delegate void TimerStarted();
    public event TimerStarted TimerStartedEvent;

    public delegate void TimerFinished();
    public event TimerFinished TimerFinishedEvent;
    
    private void OnEnable()
    {
        _timerNetworkVariable.OnValueChanged += _OnNetworkVariableChanged;
        tmpText.text = _GetTimerString(timerScriptableObject.startingTime);
    }

    private void OnDisable()
    {
        _timerNetworkVariable.OnValueChanged -= _OnNetworkVariableChanged;
    }

    public void StartTimer()
    {
        if (!IsServer) return;
        _timerNetworkVariable.Value = timerScriptableObject.startingTime;
        _StartTimerClientRpc();
        StartCoroutine(TimerCoroutine());
        
        IEnumerator TimerCoroutine()
        {
            while (_timerNetworkVariable.Value >= 0)
            {
                yield return new WaitForSeconds(1);
                _timerNetworkVariable.Value--;
            }
        }
    }

    [ClientRpc]
    protected void _StartTimerClientRpc()
    {
        tmpText.enabled = true;
        TimerStartedEvent?.Invoke();
    }
    
    protected void _OnNetworkVariableChanged(int oldValue, int newValue)
    {
        tmpText.text = _GetTimerString(newValue);
        if (newValue == -1)
        {
            tmpText.enabled = false;
            finished = true;
            TimerFinishedEvent?.Invoke();
        }
    }

    protected virtual string _GetTimerString(int time)
    {
        return time.ToString();
    }
}
