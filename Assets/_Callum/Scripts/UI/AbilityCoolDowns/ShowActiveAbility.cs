/**************************************************************************************************************
* Show Active Ability
* Used to show whether an ability is active for the cars that don't have abilities with cooldowns.
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ShowActiveAbility : MonoBehaviour
{
    // Using a slider since all of the ability cooldowns had a slider on their prefabs
    private GameObject icon;

    private void Start()
    {
        icon = GetComponentInChildren<Slider>().gameObject;
        icon.SetActive(false);
    }

    public void ToggleIcon(bool value)
    {
        if(!icon) return;

        icon.SetActive(value);
    }

}
