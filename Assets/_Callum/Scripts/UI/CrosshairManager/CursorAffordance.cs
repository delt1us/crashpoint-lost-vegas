using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ MuscleCar1_low ] 
// Purpose -                 [  ] 
// Functions -               [ 1. Changes the Cusors Texture Depedning on What its Hovering Over ]
//                           [ 2. Detects if its Hovering Over a Enemy/Ally and Displays a Incator Accordantly ]
//                           [ 3. Gets the Name of the GameObject the Player is Hovering Over and Displays it with the Indicator ]
// Dependencies -            [ EnemyIndicator ]
// Notes -  Planning to Change the Name Of the Script to "Cursor Affordance" [Done]
//          Need to Add the GameObject Name Change for the Ally Indicator
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///                                                                                                                                  //
/// Changes - Changed the way the cusor affodance worked before it detected it if the player manually hovered over it with the mouse //
//  now their is a empty gameobject that casts a ray and if it hits a target it will change colour and display the indicator.        //
//                                                                                                                                   //
//  The Cursor is now also in a set fixed postion instead of being moved by the player inputs.      
//
//  Changed the way the indicator worked now instead of spawing a new one each time a player hovers over a target, each target now has
//  its own which will activate and deactivate depeding on the situration. 
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class CursorAffordance : MonoBehaviour
{
    // Public Fields 
    public Texture2D crosshairTexture;
    public Color normalCursorColor;
    public Color enemyCursorColor;
    public Color allyCusorColor;
   public Texture2D enemyCrossHairTexture;
    public Texture2D allyCrossHairTexture;

    private RectTransform dotCustorUI;

    
    public GameObject raycastHolder;
    public GameObject allyRaycastHolder;

    private Vector3 crosshairPos;

    public GameObject enemyIndicatorPrefab;
    public GameObject allyIndicatorPrefab;

    public Camera Camera;

    // Private Fields
    private GameObject currentEnemyIndicator;
    private GameObject currentAllyIndicator;

    private EnemyIndicator enemyIndicator;
    private EnemyIndicator allyIndicator;

    private bool isHoveringEnemy;
    private bool isHoveringAlly;

    public CrosshairManager crosshairManager;
    private void Start()
    {
        if (Camera == null)
        {
            Camera = Camera.main;
        }
        Canvas carCanvas = GetComponentInChildren<Canvas>();
        if (carCanvas != null)
        {
            dotCustorUI = new GameObject("DotCustor").AddComponent<RectTransform>();
            dotCustorUI.SetParent(carCanvas.transform);
            dotCustorUI.anchoredPosition = new Vector2(0f, 50f);
            dotCustorUI.sizeDelta = new Vector2(28f, 30f);
            dotCustorUI.gameObject.AddComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(crosshairTexture, new Rect(0, 0, crosshairTexture.width, crosshairTexture.height), new Vector2(0.5f, 0.5f));
        }

        dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = normalCursorColor;
    }

    private void Update()
    {
        DetectEnemy();
    }



    private void DetectEnemy()
    {
        // Casts a Ray from the mouse position to detect for enemy
        Vector3 rayOrigin = raycastHolder.transform.position;
        Vector3 cursorWorldPosition = dotCustorUI.position;
        cursorWorldPosition = Camera.ScreenToWorldPoint(cursorWorldPosition);
        Ray ray = new Ray(rayOrigin, cursorWorldPosition - rayOrigin);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue);


        //  Ray ray = new Ray(cursorWorldPosition, Camera.main.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue);

        bool foundEnemy = Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Target");


        // If a enemy has been detected and was not previously hoveing over a enemy
        if (foundEnemy && !isHoveringAlly)
        {

            // isHoveringEnemy = true;

            // Changes the Cursor to the enemyCrossHairTexture
            //  Cursor.SetCursor(enemyCrossHairTexture, Vector2.zero, CursorMode.Auto);
            isHoveringEnemy = true;
            isHoveringAlly = false;
            dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = enemyCursorColor;
            Debug.Log("Looking at Enemy");

            enemyIndicator = hit.collider.GetComponentInChildren<EnemyIndicator>(true);

            if (enemyIndicator != null)
            {
                enemyIndicator.gameObject.SetActive(true);
                enemyIndicator.UpdateText(hit.collider.gameObject.name);
            }

        }
        else
        {
            isHoveringEnemy = false;

            dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = normalCursorColor;
            DetectAlly();

            if (enemyIndicator != null)
            {
                enemyIndicator.gameObject.SetActive(false);
            }
        }
        /*
        // Instantates the EnemyIndicator Prefab
        if (currentEnemyIndicator == null)
        {
            currentEnemyIndicator = Instantiate(enemyIndicatorPrefab, hit.collider.transform);
            enemyIndicator = currentEnemyIndicator.GetComponent<EnemyIndicator>();
        }

        // Sets the Indicator as a child of the Canvas and updates its position
        currentEnemyIndicator.transform.SetParent(hit.collider.GetComponentInChildren<Canvas>().transform, false);
        Vector3 indicatorPosition = hit.collider.transform.position + new Vector3(-0.8f, 1f, 0f);
        currentEnemyIndicator.transform.position = indicatorPosition;

        // Updates the Text using the GameIndicator Script
        if (enemyIndicator != null)
        {
            enemyIndicator.UpdateText(hit.collider.gameObject.name);


        }


    

        // Sets the CrossHair back to its defeult if the Player was previously hovering over the Enemy and no Enemy is then detected 
        else if (!foundEnemy)
        {
           // isHoveringEnemy = false;
           // Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);
            dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = normalCursorColor;
            // Destroys the EnemyIndicator
            if (currentEnemyIndicator != null)
            {
                Destroy(currentEnemyIndicator);
                currentEnemyIndicator = null;
            }
        }*/
    
    }
    
    private void DetectAlly()
    {
        Vector3 rayOrigin = allyRaycastHolder.transform.position;
        Vector3 cursorWorldPosition = dotCustorUI.position;
        cursorWorldPosition = Camera.ScreenToWorldPoint(cursorWorldPosition);
        Ray ray = new Ray(rayOrigin, cursorWorldPosition - rayOrigin);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        RaycastHit hit;
        bool foundAlly = Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Ally");

        if (foundAlly && !isHoveringEnemy)
        {

            isHoveringAlly = true;
            isHoveringEnemy = false;
            dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = allyCusorColor;
            Debug.Log("Looking at Ally");

            allyIndicator = hit.collider.GetComponentInChildren<EnemyIndicator>(true);

            if (allyIndicator != null)
            {
                allyIndicator.gameObject.SetActive(true);
                allyIndicator.UpdateText(hit.collider.gameObject.name);
            }
        }
        else
        {
            isHoveringAlly = false;
            dotCustorUI.GetComponent<UnityEngine.UI.Image>().color = normalCursorColor;
            

            if (allyIndicator != null)
            {
                allyIndicator.gameObject.SetActive(false);
            }

        }
            /*
            // Casts a Ray from the mouse position to detect for ally
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool foundAlly = Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Ally");

            // If a enemy has been detected and was not previously hoveing over a ally
            if (foundAlly && !isHoveringAlly)
            {
                // Changes the Cursor to the allyCrossHairTexture
                isHoveringAlly = true;
                isHoveringEnemy = false;
                Cursor.SetCursor(allyCrossHairTexture, Vector2.zero, CursorMode.Auto);
                Debug.Log("Looking at Ally");

                // Instantates the AllyIndicator Prefab
                if (currentAllyIndicator == null)
                {
                    currentAllyIndicator = Instantiate(allyIndicatorPrefab, hit.collider.transform);
                }

                // Sets the Indicator as a child of the Canvas and updates its position
                currentAllyIndicator.transform.SetParent(hit.collider.GetComponentInChildren<Canvas>().transform, false);
                Vector3 indicatorPosition = hit.collider.transform.position + new Vector3(-0.8f, 1f, 0f);
                currentAllyIndicator.transform.position = indicatorPosition;
            }

            // Sets the CrossHair back to its defeult if the Player was previously hovering over the Ally and no Ally is then detected 
            else if (!foundAlly && isHoveringAlly)
            {
                isHoveringAlly = false;
                Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);

                if (currentAllyIndicator != null)
                {
                    Destroy(currentAllyIndicator);
                    currentAllyIndicator = null;
                }
            }
            */
    
        }
            

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 100f);

        }
}

