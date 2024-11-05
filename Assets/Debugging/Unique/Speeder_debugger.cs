using TMPro;
using UnityEngine;

public class Speeder_debugger : MonoBehaviour
{
    [SerializeField] private GameObject toDebug;
    [SerializeField] private TextMeshProUGUI txt_maxBoost;
    [SerializeField] private TextMeshProUGUI txt_currentBoost;
    [SerializeField] private TextMeshProUGUI txt_regenTimer;
    [SerializeField] private TextMeshProUGUI txt_regenRate;
    [SerializeField] private TextMeshProUGUI txt_active;

    private const string prefix_maxBoost = "Max Boost: ";
    private const string prefix_currentBoost = "Current Boost: ";
    private const string prefix_regenTimer = "Regen Timer: ";
    private const string prefix_regenRate = "Regen Rate: ";
    private const string prefix_active = "ACTIVE: ";

    private Speeder speederScript;

    // Start is called before the first frame update
    private void Start()
    {
        speederScript = toDebug.GetComponent<Speeder>();
    }

    // Update is called once per frame
    void Update()
    {
        txt_maxBoost.text = prefix_maxBoost + speederScript.GetMaxBoost();
        txt_currentBoost.text = prefix_currentBoost + speederScript.GetCurrentBoost();
        txt_regenTimer.text = prefix_regenTimer + speederScript.GetTimer().ToString("00.00");
        txt_regenRate.text = prefix_regenRate + speederScript.GetRegenRate();
        txt_active.text = prefix_active + speederScript.GetActive();
    }

}
