using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBag : MonoBehaviour
{
  public GameObject sandBags;
  public Transform spawnTransforms;

  private void Update()
  {
    if(Input.GetKeyDown(KeyCode.B))
    {
      Instantiate(sandBags, spawnTransforms.position, Quaternion.identity);
    }
  }
}
