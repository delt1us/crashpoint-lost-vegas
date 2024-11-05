using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarInfoUIButtonsScriptableObject", menuName = "Scriptable Objects/UI/Car Info UI Buttons")]
public class CarInfoUIButtonsScriptableObject : ScriptableObject
{
    public delegate void NextCarButtonPressed();
    public event NextCarButtonPressed NextCarButtonPressedEvent;

    public delegate void PreviousCarButtonPressed();
    public event PreviousCarButtonPressed PreviousCarButtonPressedEvent;

    public delegate void BackButtonPressed();
    public event BackButtonPressed BackButtonPressedEvent;

    public delegate void ConfirmButtonPressed();
    public event ConfirmButtonPressed ConfirmButtonPressedEvent;

    public void InvokeNextButton()
    {
        NextCarButtonPressedEvent?.Invoke();
    }

    public void InvokePreviousButton()
    {
        PreviousCarButtonPressedEvent?.Invoke();
    }

    public void InvokeBackButton()
    {
        BackButtonPressedEvent?.Invoke();
    }

    public void InvokeConfirmButton()
    {
        ConfirmButtonPressedEvent?.Invoke();
    }
}
