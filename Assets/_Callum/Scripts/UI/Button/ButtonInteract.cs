using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInteract : MonoBehaviour, IPointerClickHandler
{
    public RectTransform interactable;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 clickPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(interactable, eventData.position, eventData.pressEventCamera, out clickPos);

        if (interactable.rect.Contains(clickPos))
        {
            Debug.Log("Button Press");
        }
    }

}
