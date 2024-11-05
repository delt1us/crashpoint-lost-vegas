using UnityEngine;
//c
// Summary 
// Attached To GameObjects - [ DamageText ] 
// Purpose -                 [ Provies A Animated Damage Number ] 
// Functions -               [ 1. Animates the Damage Number Using TeenTween ]
//                           [ 2. Rotates to Look at the Player ]
// Dependencies -            [ WeaponController ]
// Notes -  
public class DamageText : PoolableObject
{
    // Public Fields
    public Vector3 Offset   = new Vector3(0f, 2f, 0f);
    public Vector3 ScaleBig = new Vector3(1.2f, 1.2f, 1.2f);

    public float ScaleDuration = 0.2f;
    public float ScaleDelay    = 0.4f ;

    public float FadeOutDelay    = 0.4f;
    public float FadeOutDuration = 0.4f;

    public float MoveAmmount = 35;

    public TextMesh  Text;
    public Font font;
    private Material originalFontMaterial;
   

    public Transform carPosition;

    private void Awake()
    {
        Text = GetComponent<TextMesh>();
    }
    private void Update()
    {
        LookAtPlayer();

        carPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void LookAtPlayer()
    {
        Vector3    direction = carPosition.position - transform.position;
        Quaternion rotation  = Quaternion.LookRotation(direction);
        transform.rotation   = rotation * Quaternion.Euler(0f, 180f, 0f); 
    
       
        
        }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            carPosition = player.transform;
        }

        FadeOut();
    }
    
    private void FadeOut()
    {
        Text.color = Color.white;
        // Text.rectTransform.localPosition = new Vector2(0, 25);
        Text.transform.localPosition = new Vector3(0f, 0f, 0f);
        Text.transform.localScale = new Vector3(1f, 1f, 1f);
       // CanvasGroup.alpha = 1;
        LeanTween.scale(gameObject, ScaleBig, ScaleDuration).setOnComplete(ScaleToNormal);
    }

    private void ScaleToNormal()
    {
        
        LeanTween.scale(gameObject, Vector3.one, ScaleDuration).setDelay(ScaleDelay).setOnComplete(FadeOutText);
    }
    private void FadeOutText()
    {
        LeanTween.alpha(gameObject, 0, FadeOutDuration).setDelay(FadeOutDelay);
        LeanTween.moveLocalY(gameObject,transform.localPosition.y + MoveAmmount, FadeOutDuration).setDelay(FadeOutDelay).setOnComplete(OnDisable);
        
    }
  

    public void SetDamageText(string damage)
    {
        Text.text = damage;
    }

    private void OnEnable()
    {   
        
        FadeOut();
    }

   
    private void OnDisable()
    {
       
        Debug.Log("FadeOut");
        gameObject.SetActive(false);
        base.OnDisable();
        
    }
}
