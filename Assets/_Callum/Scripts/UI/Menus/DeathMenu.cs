using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathMenu : MonoBehaviour
{
    public HealthManager healthManager;
    public Button respawnButton;
    public TMP_Text countDownText;
    private CarSpawner carSpawner;

    private bool isCooling = false;
    private bool isRespawnButtonClicked = false;
    // add timer to respawn player 
    // make the respawn button not interactble until 5 seconds have passed

    public void Start()
    {

        carSpawner = FindObjectOfType<CarSpawner>();
      
        gameObject.SetActive(false);

        respawnButton = GetComponentInChildren<Button>();
        respawnButton.interactable = false;

        countDownText = respawnButton.GetComponentInChildren<TMP_Text>();

       // respawnButton.onClick.AddListener(CheckButtonPress);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("mainMenu");
    }

    /*
    public void Respawn()
    {
        if (isCooling && isRespawnButtonClicked)
        {
       //     isRespawnButtonClicked = false;
<<<<<<< Updated upstream
            //carSpawner.RespawnCar();
=======
          //  carSpawner.RespawnCar();
// Stashed changes

            Debug.Log("Respawn");
        }
       
       

    }
    */
    public void CheckButtonPress()
    {
        if (isCooling)
        {
            isRespawnButtonClicked = true;
          //  Respawn();
        }
    }

    private IEnumerator RespawnCoolDown()
    {
        float countdownDuration = 5f;
        float currentTime = countdownDuration;
        
        while (currentTime > 0)
        {
            countDownText.text = currentTime.ToString("F0");
            yield return new WaitForSeconds(1f);
            currentTime--;
            
        }
        respawnButton.interactable = true;
        countDownText.gameObject.SetActive(false);
        isCooling = true;

        
        
    }

    // public void ShowDeathScreen()
    // {
    //     if (healthManager != null && healthManager.TakeDamage(0) == true)
    //     {
    //         gameObject.SetActive(true);
    //         countDownText.text = "5";
    //         
    //         StartCoroutine(RespawnCoolDown());
    //         Cursor.lockState = CursorLockMode.None;
    //         Debug.Log("ShowDeath");
    //         
    //     }      
    // }
    //
    //  
}
