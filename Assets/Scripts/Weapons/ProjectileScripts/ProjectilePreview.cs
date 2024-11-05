using UnityEngine;

public class ProjectilePreview : MonoBehaviour
{
    [SerializeField] private ProjectileStats projStats;
    [SerializeField] private GameObject previewImg;
    [SerializeField] private GameObject spawn;
    [SerializeField, Range(1, 10000)] private float simSpeed = 25;
    [SerializeField, Tooltip("The layers that the simulation will detect...")] private LayerMask collisionMasks;

    private Rigidbody simBody;
    private bool simming;
    private bool landed;

    private void Start()
    {
        simBody = GetComponent<Rigidbody>();
        previewImg.transform.localScale = new(projStats.BlastRadius, 1, projStats.BlastRadius);
    }

    private void Update()
    {
        landed = transform.position.y <= FindFloor();

        if(landed) previewImg.transform.SetParent(GetComponentInParent<ActionController>().transform);
    }

    private void FixedUpdate()
    {
        // Copying the additional gravity that's applied to the projectile.
        if(simBody) simBody.AddForce(Physics.gravity * projStats.GravityMultiplier * simSpeed * simSpeed * simSpeed/15);
    }

    private float FindFloor()
    {
        // Doing a ray cast downwards from where the simulated projectile is...
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100, collisionMasks);

        // Getting the hit point from directly underneath the simmed projectile.
        float floor = hit.point.y;

        previewImg.transform.position = new(landed? previewImg.transform.position.x: transform.position.x, floor, landed ? previewImg.transform.position.z : transform.position.z);

        return floor;
    }

    public void ShowPreview()
    {
        // Only if the preview ISN'T showing...
        if (simming) return;

        transform.localPosition = new();
        transform.localRotation = new();

        // DOESN'T FOLLOW THE CAR'S DIRECTION UNTIL IT LANDED       (converting to m/s)
        simBody.velocity = (GetComponentInParent<MovementController>().GetRawSpeed() * simBody.transform.forward) * simSpeed * .1f;

        // Ensuring that the position and rotation is identical to the weapon whenever the aim button is pressed - so that the trajectories are the same.
        // (Only do it when the object isn't visible.)
        if (previewImg.activeSelf) return;

        simBody.AddForce(transform.forward * projStats.LaunchForce * simSpeed, ForceMode.Impulse);

        // Should only be done once...
        simming = true;
        previewImg.SetActive(true);
    }

    public void HidePreview()
    {
        // Only if the preview IS showing...
        if (!simming) return;

        // Removing its velocity
        simBody.velocity = new();
        transform.SetParent(spawn.transform);
        
        simming = false;
        previewImg.SetActive(false);
    }
}
