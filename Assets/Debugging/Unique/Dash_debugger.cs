using TMPro;
using UnityEngine;

public class Dash_debugger : MonoBehaviour
{
    [SerializeField] private GameObject toDebug;
    [SerializeField] private TextMeshProUGUI txt_chargeTotal;
    [SerializeField] private TextMeshProUGUI txt_chargeTimer;
    [SerializeField] private TextMeshProUGUI txt_chargeDelay;

    private const string prefix_chargeTotal = "Charges: ";
    private const string prefix_chargeTimer = "Regen time left: ";
    private const string prefix_chargeDelay = "Charge Time: ";

    private Dasher dashScript;

    // Start is called before the first frame update
    private void Start()
    {
        dashScript = toDebug.GetComponent<Dasher>();
    }

    // Update is called once per frame
    void Update()
    {
        txt_chargeTotal.text = prefix_chargeTotal + dashScript.GetChargeTotal();
        txt_chargeTimer.text = prefix_chargeTimer + dashScript.GetChargeTimer().ToString("00.00");
        txt_chargeDelay.text = prefix_chargeDelay + dashScript.GetChargeDelay();
    }
}
