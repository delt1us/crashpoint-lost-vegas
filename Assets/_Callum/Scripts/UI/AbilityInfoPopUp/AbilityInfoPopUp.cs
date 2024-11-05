using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityInfoPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject abilityDescriptionPanel;
    public GameObject abilityDescription;

    private void Start()
    {
        abilityDescriptionPanel.SetActive(false);
        abilityDescription.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ability Image Clicked");
        abilityDescriptionPanel.SetActive(true);
        abilityDescription.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ability Image Removed");
        abilityDescriptionPanel.SetActive(false);
        abilityDescription.SetActive(false);
    }
}
