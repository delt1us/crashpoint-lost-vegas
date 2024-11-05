using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawScript : MonoBehaviour
{
    public float speed = -1000f;

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime,0f );
    }

}
