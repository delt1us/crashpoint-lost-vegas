using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class DeliveryDebugger : MonoBehaviour
{
    [SerializeField] private GameObject toDebug;

    [SerializeField] private TextMeshProUGUI txt_maxCharges;
    [SerializeField] private TextMeshProUGUI txt_currentCharges;
    [SerializeField] private TextMeshProUGUI txt_cooldown;
    [SerializeField] private TextMeshProUGUI txt_chargeDelay;
    [SerializeField] private TextMeshProUGUI txt_chargeTimer;

    private const string prefix_maxCharges = "Max Boxes: ";
    private const string prefix_currentCharges = "Boxes Left: ";
    private const string prefix_cooldown = "Throw Cooldown: ";
    private const string prefix_chargeDelay = "Recharge Delay: ";
    private const string prefix_chargeTimer = "Recharge Timer: ";

    private SpeedDelivery deliveryScript;

    // Start is called before the first frame update
    private void Start()
    {
        deliveryScript = toDebug.GetComponent<SpeedDelivery>();
    }

    // Update is called once per frame
    void Update()
    {
        txt_maxCharges.text = prefix_maxCharges + deliveryScript.GetMaxCharges();
        txt_currentCharges.text = prefix_currentCharges + deliveryScript.GetCharges();
        txt_cooldown.text = prefix_cooldown + deliveryScript.GetCooldown().ToString("00.00");
        txt_chargeDelay.text = prefix_chargeDelay + deliveryScript.GetChargeDelay();
        txt_chargeTimer.text = prefix_chargeTimer + deliveryScript.GetChargeTimer().ToString("00.00");
    }
}
