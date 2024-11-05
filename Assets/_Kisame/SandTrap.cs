using UnityEngine;

public class SandTrap : MonoBehaviour
{
    public float rotationSpeed = 30f;

    private bool inSandTrap = false;
    private GameObject playerCar;
    private Quaternion initialRotation;
    private Rigidbody carRigidbody;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !inSandTrap)
        {
            inSandTrap = true;
            playerCar = other.gameObject;


            if (playerCar.GetComponent<Rigidbody>()) carRigidbody = playerCar.GetComponent<Rigidbody>();

            else carRigidbody = playerCar.GetComponentInParent<Rigidbody>();

            if (carRigidbody != null)
            {
                carRigidbody.velocity = Vector3.zero;
                carRigidbody.angularVelocity = Vector3.zero;
            }

            initialRotation = playerCar.transform.rotation;
           // carRigidbodies = playerCar.GetComponentsInChildren<Rigidbody>();

            StartCoroutine(SpinCarForDuration());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == playerCar)
        {
            inSandTrap = false;
            playerCar = null;
        }
    }

    private System.Collections.IEnumerator SpinCarForDuration()
    {
        float elapsedTime = 0f;
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            playerCar.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

           
            carRigidbody.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
           
        }

        if (playerCar != null)
        {
            playerCar.transform.rotation = initialRotation;
        }
    }
}
