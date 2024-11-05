using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Turn_debugger : MonoBehaviour
{
    [SerializeField] private GameObject toDebug;

    [SerializeField] private TextMeshProUGUI txt_chargeTotal;
    [SerializeField] private TextMeshProUGUI txt_chargeDelay;
    [SerializeField] private TextMeshProUGUI txt_chargeTimer;

    private const string prefix_chargeTotal = "Charges: ";
    private const string prefix_chargeDelay = "Wait Time: ";
    private const string prefix_chargeTimer = "Regen time left: ";


    private QuickTurn turnScript;

    // Start is called before the first frame update
    private void Start()
    {
        turnScript = toDebug.GetComponent<QuickTurn>();
    }

    // Update is called once per frame
    void Update()
    {
        txt_chargeTotal.text = prefix_chargeTotal + turnScript.GetCharges();
        txt_chargeDelay.text = prefix_chargeDelay + turnScript.GetChargeDelay();
        txt_chargeTimer.text = prefix_chargeTimer + turnScript.GetChargeTime().ToString("00.00");
    }
}
