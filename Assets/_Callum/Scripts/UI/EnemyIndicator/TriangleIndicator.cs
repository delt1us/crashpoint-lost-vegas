using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleIndicator : MonoBehaviour
{
    public Transform playerTransform;
    private Transform cameraTransform;

    void Start()
    {


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
            
        }
    }
}
