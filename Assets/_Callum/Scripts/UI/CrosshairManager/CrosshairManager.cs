using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ MuscleCar1_low ] 
// Purpose -                 [  ] 
// Functions -               [ 1. Changes the Cusors Texture Depedning on What its Hovering Over ]
//                           [ 2. Detects if its Hovering Over a Enemy/Ally and Displays a Incator Accordantly ]
//                           [ 3. Gets the Name of the GameObject the Player is Hovering Over and Displays it with the Indicator ]
// Dependencies -            [ EnemyIndicator ]
// Notes - 
//          
public class CrosshairManager : MonoBehaviour
{
   // public Texture2D dotCurstorTexture;
    public Texture2D circleCurstorTexture;

    public Color normalCursorColor;
    public Color targetCursorColor;


    public Camera mainCamera;




    public float circleCursorRange = 10f;
    public float circleCusorSpeed = 5f;


    private Transform target;
    private RectTransform dotCustorUI;
    private RectTransform circularCursorUI;

    private bool isCircleCursorActive = true;

    private void Start()
    {
        

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Canvas carCanvas = GetComponentInChildren<Canvas>();
        if (carCanvas != null)
        {
           // dotCustorUI = new GameObject("DotCustor").AddComponent<RectTransform>();
           // dotCustorUI.SetParent(carCanvas.transform);
           // dotCustorUI.anchoredPosition = new Vector2(0f, 50f);
           // dotCustorUI.sizeDelta = new Vector2(28f, 30f);
           // dotCustorUI.gameObject.AddComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(dotCurstorTexture, new Rect(0, 0, dotCurstorTexture.width, dotCurstorTexture.height), new Vector2(0.5f, 0.5f));

            circularCursorUI = new GameObject("CircleCusor").AddComponent<RectTransform>();
            circularCursorUI.SetParent(carCanvas.transform);
            circularCursorUI.anchoredPosition = new Vector2(2f, 50f);
            circularCursorUI.sizeDelta = new Vector2(60f, 60f);
            circularCursorUI.gameObject.AddComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(circleCurstorTexture, new Rect(0, 0, circleCurstorTexture.width, circleCurstorTexture.height), new Vector2(0.5f, 0.5f));


        }
    }

    private void Update()
    {
       

        if (IsSecondaryReady())
        {
            target = FindClosestEnemy();


            if (target != null)
            {
                UpdateCircleCusor(target.position);
                
            }
            else
            {
                //UpdateCircleCusor(Vector3.zero);
                circularCursorUI.anchoredPosition = new Vector2(2f, 50f);
            }
        }
        else
        {
            circularCursorUI.gameObject.SetActive(false);
        }

       
        
    }

  

      


    private bool IsSecondaryReady()
    {
        return true;
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        Transform closestTarget = null;
        float closestDistance = float.MaxValue;
        Vector3 playerPosition = transform.position;

        foreach (GameObject target in targets)
        {
            Vector3 targetPosition = target.transform.position;
            float distanceToTarget = Vector3.Distance(playerPosition, targetPosition);

            if (distanceToTarget <= circleCursorRange && distanceToTarget < closestDistance)
            {
                closestTarget = target.transform;
                closestDistance = distanceToTarget;
            }
        }
        return closestTarget;
    }

    private void UpdateCircleCusor(Vector3 targetPosition)
    {
        if (target == null)
        {
            

            Vector3 targetScreen = mainCamera.WorldToScreenPoint(targetPosition);
            Vector3 returnPosition = target == null ? new Vector3(590f, 383f, 0f) : targetPosition;
            circularCursorUI.position = Vector3.Lerp(circularCursorUI.position, returnPosition, Time.deltaTime * circleCusorSpeed);
            circularCursorUI.GetComponent<UnityEngine.UI.Image>().color = normalCursorColor;

            if (Vector3.Distance(circularCursorUI.position, returnPosition) <= 1f)
            {
                circleCusorSpeed = 0f;
            }
            else
            {
                circleCusorSpeed = 5f;
            }
        }
        else
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
            circularCursorUI.position = Vector3.Lerp(circularCursorUI.position, screenPosition, Time.deltaTime * circleCusorSpeed);

            if (Vector3.Distance(circularCursorUI.position, screenPosition) <= 1f)
            {
                circleCusorSpeed = 0f;
            }
            else
            {
                circleCusorSpeed = 5f;
            }
            circularCursorUI.GetComponent<UnityEngine.UI.Image>().color = targetCursorColor;
        }
        
    } 

    public void ToggleCircleCursor(bool isActive)
    {
        
        circularCursorUI.gameObject.SetActive(isActive);
    }
   


}
