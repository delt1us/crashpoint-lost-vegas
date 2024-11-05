using TMPro;
using UnityEngine;

public class Wheelie_debugger : MonoBehaviour
{
    [SerializeField] private GameObject toDebug;
    [SerializeField] private TextMeshProUGUI txt_maxTime;
    [SerializeField] private TextMeshProUGUI txt_currentTime;
    [SerializeField] private TextMeshProUGUI txt_centerMass;
    [SerializeField] private TextMeshProUGUI txt_regenTimer;
    [SerializeField] private TextMeshProUGUI txt_regenRate;
    [SerializeField] private TextMeshProUGUI txt_active;

    private const string prefix_maxTime = "Max Time: ";
    private const string prefix_currentTime = "Current Time: ";
    private const string prefix_centerMass = "Center Mass: ";
    private const string prefix_regenTimer = "Regen Timer: ";
    private const string prefix_regenRate = "Regen Rate: ";
    private const string prefix_active = "ACTIVE: ";

    private Wheelie wheelieScript;

    // Start is called before the first frame update
    private void Start()
    {
        wheelieScript = toDebug.GetComponent<Wheelie>();
    }

    // Update is called once per frame
    void Update()
    {
        txt_maxTime.text = prefix_maxTime + wheelieScript.GetMaxTime();
        txt_currentTime.text = prefix_currentTime + wheelieScript.GetCurrentTime();
        txt_centerMass.text = prefix_centerMass + wheelieScript.GetCenterMass();
        txt_regenTimer.text = prefix_regenTimer + wheelieScript.GetTimer().ToString("00.00");
        txt_regenRate.text = prefix_regenRate + wheelieScript.GetRegenRate();
        txt_active.text = prefix_active + wheelieScript.GetActive();
    }

}
