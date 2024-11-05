using UnityEngine;
using TMPro;
//c
// Summary 
// Attached To GameObjects - [ EnemyIndicator ] 
// Purpose -                 [ Changes the Text to Display the GameObjects Name ] 
// Functions -               [ 1. Updates the Text ]
// Dependencies -            [ CrossHairManager ]
// Notes - 
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///                                                                                                                                  //
/// Changes - Changed the way the indicator is used in the game, made the indicator follow the players position
/// 
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class EnemyIndicator : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Transform playerTransform;
    private Transform cameraTransform;

     void Start()
    {



        gameObject.SetActive(false);

        cameraTransform = Camera.main.transform;
    }
    private void Update()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            Quaternion rotationToPlayer = Quaternion.LookRotation(-directionToPlayer, Vector3.up);

            transform.rotation = rotationToPlayer;
            Text.transform.LookAt(cameraTransform);
        }
    }
    public void UpdateText(string enemyName)
    {
        Text.text = enemyName;
        Debug.Log("Update EnemyName");
    }


}
