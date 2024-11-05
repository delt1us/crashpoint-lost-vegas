using System.Collections;
using UnityEngine;
using TMPro;
//c
// Summary 
// Attached To GameObjects - [ KilledPopUp ] 
// Purpose -                 [ Creats a Pop Up That Displays who the Player has Killed ] 
// Functions -               [ 1. Updates the Text to the GameObjects Name ]
//                           [ 2. Animates the Text ]
// Dependencies -            [ Health Manager ]
// Notes - Text Currently is Not Animated  
public class KilledPopUp : MonoBehaviour
{
    // Public Fields
    public TextMeshProUGUI popUpText;

    public float disaplyDuration = 3f;

    public Vector3 ScaleBig = new Vector3(1.2f, 1.2f, 1.2f);

    public float ScaleDuration = 0.2f;
    public float ScaleDelay    = 0.4f;

    public float FadeOutDelay    = 0.4f;
    public float FadeOutDuration = 0.4f;

    private void Start()
    {
        gameObject.SetActive(false);
        FadeOut();
    }
    public void SetText(string targetName)
    {
        popUpText.text = targetName;

        
        gameObject.SetActive(true);
        StartCoroutine(DisableGameObject());
    }
    public void FadeOut()
    {
        popUpText.color = Color.red;
        
        popUpText.rectTransform.localScale = Vector3.zero;
        LeanTween.scale(popUpText.gameObject, ScaleBig, ScaleDuration).setOnComplete(FadeOutText);
    }
    public void FadeOutText()
    {
        LeanTween.alpha(popUpText.gameObject, 0f, FadeOutDuration).setDelay(FadeOutDelay).setEaseInCubic();
    }
    private IEnumerator DisableGameObject()
    {
        yield return new WaitForSeconds(disaplyDuration);

        gameObject.SetActive(false);
    }
}
