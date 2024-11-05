using UnityEngine;

public class UFOController : MonoBehaviour
{
    public Transform player;
    public Vector3 detectionSize = new Vector3(10.2f, 135.11f, 18.2f);
    public Vector3 detectionOffset = new Vector3(-18.85f, -66.2f, 5.6f);
    public float hoverForce = 10f;
    public float hoverHeight = 30f; // Adjusted hover height
    public float rotationSpeed = 180f;
    public float movementSpeed = 5f;

    private Rigidbody rb;
    private bool gravityActivated = false;
    private Vector3 originalPosition;
    private bool landed = false;
    private bool moveToPointEnabled = false;
    private Vector3[] targetPositions;
    private int currentTargetIndex = 0;
    private bool returnToOriginal = false;

    private float detectionRadius;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        targetPositions = GenerateTargetPositions(transform.position, 100f, 600f, 5);

        detectionRadius = Mathf.Max(detectionSize.x, detectionSize.y, detectionSize.z /2);
    }

    void Update()
    {
        if (!gravityActivated && Vector3.Distance(transform.position + detectionOffset, player.position) < detectionRadius)
        {
            gravityActivated = true;
            rb.useGravity = true;
            rb.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
        }

        if (gravityActivated && !landed)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            Vector3 targetHoverPosition = originalPosition - new Vector3(0f, hoverHeight, 0f);

            if (!moveToPointEnabled)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetHoverPosition, Time.deltaTime * movementSpeed);
            }

            if (transform.position.y <= targetHoverPosition.y)
            {
                landed = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                moveToPointEnabled = true;
            }
        }

        if (landed && moveToPointEnabled)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (!returnToOriginal)
            {
                Vector3 targetPosition = targetPositions[currentTargetIndex] + new Vector3(0f, -hoverHeight, 0f);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);

                if (transform.position == targetPosition)
                {
                    currentTargetIndex++;

                    if (currentTargetIndex >= targetPositions.Length)
                    {
                        returnToOriginal = true;
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, Time.deltaTime * movementSpeed);

                if (transform.position == originalPosition)
                {
                    moveToPointEnabled = false;
                    returnToOriginal = false;
                    gravityActivated = false;
                    landed = false;
                    rb.useGravity = false;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    ExecuteCode();
                }
            }
        }
    }

    void ExecuteCode()
    {
        targetPositions = GenerateTargetPositions(transform.position, 100f, 600f, 5);
        currentTargetIndex = 0;
        Update();
        Debug.Log("Executing code repeatedly!");
    }

    Vector3[] GenerateTargetPositions(Vector3 center, float distanceRangeMin, float distanceRangeMax, int count)
    {
        Vector3[] positions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-distanceRangeMax, distanceRangeMax), 0f, Random.Range(-distanceRangeMax, distanceRangeMax));
            positions[i] = center + randomOffset;
        }

        return positions;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 detectionPosition = transform.position + detectionOffset;
        Gizmos.DrawWireCube(detectionPosition, detectionSize);
    }
}