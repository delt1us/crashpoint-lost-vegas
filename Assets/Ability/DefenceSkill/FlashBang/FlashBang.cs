/**************************************************************************************************************
* FlashBang
* For FlashBang physic and effect
*
* Created by Envy Cham 2023
* 
* Change Log:
*8/2 Set range for the effect
*
*            
***************************************************************************************************************/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashBang : MonoBehaviour
{
  private Image WhiteImage;
  private Rigidbody rb;
  private ParticleSystem FlashPartice;
  private MeshRenderer Ren;
  private SphereCollider SphereCollider;
  [SerializeField]LayerMask layerMask;
  [SerializeField] private float fuseTime = 2;

  [Space]
  [SerializeField] private AudioSource WhiteNoise;
  [SerializeField]private AudioSource Bang;

   

  private void Start()
  {
    Ren = gameObject.GetComponentInParent<MeshRenderer>();
    WhiteImage = GameObject.FindGameObjectWithTag("WhiteImage").GetComponent<Image>();
    //WhiteNoise = GameObject.FindGameObjectWithTag("WhiteNoise").GetComponent<AudioSource>();
    //Bang = GameObject.FindGameObjectWithTag("Bang").GetComponent<AudioSource>();
    FlashPartice = gameObject.GetComponentInParent<ParticleSystem>();
    rb = gameObject.GetComponentInParent<Rigidbody>();
    rb.velocity = Vector3.zero;
    
    SphereCollider = gameObject.GetComponent<SphereCollider>();
    SphereCollider.enabled = false;
    

  }

 
  private void Update()
  {
    StartCoroutine(Startup());
   // rb.AddForce(transform.forward * 125, ForceMode.Force);
   // Destroy(gameObject, 8f);

  }

  private IEnumerator Startup()
  {
    yield return new WaitForSeconds(3);
    SphereCollider.enabled = true;
  }
  private void OnTriggerEnter(Collider other)
  {
    
    if ((layerMask & 1 << other.gameObject.layer) > 0)
    {
      
      StartCoroutine(WhiteFade());
      Debug.Log("Trigger entered!");
      
    }
  }

  
  private IEnumerator WhiteFade()
  {
    // Added an editable fuse time in the inspector (Dean)
    yield return new WaitForSeconds(fuseTime);

    WhiteImage.color = Color.white;
    FlashPartice.Play();
    //Bang.Play();
    //WhiteNoise.Play();
    Ren.enabled = false;


    float FadeSpeed = 1;
    float Modifier = 0.01f;
    float WaitTime = 0;

    //fading white screen 
    for (int i =0;WhiteImage.color.a > 0;i++)
    {
      WhiteImage.color = new(1, 1, 1, FadeSpeed);
      FadeSpeed -= 0.025f;
      Modifier *= 1.5f;
      WaitTime = 0.5f - Modifier;

      if (WaitTime < 0.1f) WaitTime = 0.1f;
     // WhiteNoise.volume -= 0.05f;
      yield return new WaitForSeconds(WaitTime);

     if (WaitTime <= 0)
      {
        Destroy(WhiteImage);
      }  
    }

   // WhiteNoise.Stop();
   // WhiteNoise.volume = 1;
  }
}
