using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMovement : MonoBehaviour
{
    public GameObject objectToMove;
    public Vector3 targetPosition;
    public float movementSpeed = 5f;

    private Vector3 originalPosition;
    private bool playerInsideCollider;
    private Coroutine movementCoroutine;

    private void Start()
    {
        originalPosition = objectToMove.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCollider = true;
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(MoveObject(targetPosition));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCollider = false;
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(MoveObject(originalPosition));
        }
    }

    private IEnumerator MoveObject(Vector3 target)
    {
        while (Vector3.Distance(objectToMove.transform.position, target) > 0.01f)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, target, movementSpeed * Time.deltaTime);
            yield return null;
        }
    }
}



