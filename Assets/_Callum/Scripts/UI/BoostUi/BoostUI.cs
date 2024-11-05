using UnityEngine;
using UnityEngine.UI;

//c
// Summary 
// Attached To GameObjects - [ BoostBarSlider ] 
// Purpose -                 [ Updates the BoostBar ] 
// Functions -               [ 1. Updates the Slider Based On The GameObjects CurrentBoost Vaule ]
//                           [ 2. Gradient of the Fill Updates Based on the Vaule ]                           
// Dependencies -            [ BoostManager ]
// Notes - 
public class BoostUI : MonoBehaviour
{
    // Public Fields 
    public Slider boostSlider;
    public Image  fill;
    public Image maxBoostImage;
    public Image highBoostImage;
    public Image midBoostImage;
    public Image lowBoostImage;
    public Image emptyBoostImage;

    // Private Fields 
    private BoostManager boostManager;
    private float currentBoost;
    private float maxBoost;
    private float boost;

    // Serialized Fields
    [SerializeField]  Gradient boostBarGradient;
    private void Start()
    {
        boostManager = GetComponentInParent<MovementController>().GetBoostManager();
    }

    private void Update()
    {
            // Updates the BoostBar slider based on the boost vaule
             currentBoost = boostManager.GetCurrentBoost();
             maxBoost     = boostManager.GetMaxBoost();
             boost        = currentBoost / maxBoost;

            boostSlider.value = boost;

            UpdatBoostBarColor();
        CheckBoost();
    }

    public void UpdatBoostBarColor()
    {
        // Updates the BoostBar fill using the normalised boost vaule
        fill.color = boostBarGradient.Evaluate(boostSlider.normalizedValue);
    }

    private void CheckBoost()
    {
        float boostPercetange = currentBoost / maxBoost;

        if (boostPercetange <= 0.2)
        {
            maxBoostImage.gameObject.SetActive(false);
            highBoostImage.gameObject.SetActive(false);
            midBoostImage.gameObject.SetActive(false);
            lowBoostImage.gameObject.SetActive(false);
            emptyBoostImage.gameObject.SetActive(true);
                
        }
        else if (boostPercetange <= 0.4)
        {
            maxBoostImage.gameObject.SetActive(false);
            highBoostImage.gameObject.SetActive(false);
            midBoostImage.gameObject.SetActive(false);
            lowBoostImage.gameObject.SetActive(true);
            emptyBoostImage.gameObject.SetActive(false);
        }
        else if (boostPercetange <= 0.6)
        {
            maxBoostImage.gameObject.SetActive(false);
            highBoostImage.gameObject.SetActive(false);
            midBoostImage.gameObject.SetActive(true);
            lowBoostImage.gameObject.SetActive(false);
            emptyBoostImage.gameObject.SetActive(false);
        } 
        else if (boostPercetange <= 0.8)
        {
            maxBoostImage.gameObject.SetActive(false);
            highBoostImage.gameObject.SetActive(true);
            midBoostImage.gameObject.SetActive(false);
            lowBoostImage.gameObject.SetActive(false);
            emptyBoostImage.gameObject.SetActive(false);
        }
        else
        {
            maxBoostImage.gameObject.SetActive(true);
            highBoostImage.gameObject.SetActive(false);
            midBoostImage.gameObject.SetActive(false);
            lowBoostImage.gameObject.SetActive(false);
            emptyBoostImage.gameObject.SetActive(false);
        }
    }

}
