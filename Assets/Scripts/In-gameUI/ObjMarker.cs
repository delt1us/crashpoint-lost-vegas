/**************************************************************************************************************
* Objective Marker
* Used in King of the Hill's capture points to make the position of the point clearer. Responsible for making the marker spin and bob
*
* Created by Dean Atkinson-Walker 2023
*
***************************************************************************************************************/

using Unity.Netcode;
using UnityEngine;

public class ObjMarker : NetworkBehaviour
{
    [SerializeField] private Material white;
    [SerializeField] private Material green;
    [SerializeField] private Material purple;

    [Header("Bob")]
    [SerializeField] private Transform[] moveTransforms;
    [SerializeField] private float lerpSpeed = 1;
    private int index;

    // How close the indicator has to be in order to go to the next point
    private const float moveThreshold = 2;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!IsServer) return;

        Bob();
    }

    private void Bob()
    {
        // slowly rotate the the cubes
        transform.localEulerAngles = new(transform.localEulerAngles.x, transform.localEulerAngles.y + .3f, transform.localEulerAngles.z);

        Vector3 targetPos = moveTransforms[index].position;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);

        if (Vector3.Distance(transform.position, targetPos) < moveThreshold)
        {
            index++;
            index = index == moveTransforms.Length ? 0 : index;
        }
    }


    public void ChangeColour(string colour)
    {
        Material newMat = colour switch
        {
            "white" => white,
            "purple" => purple,
            "green" => green,
            _ => white,
        };
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) renderer.material = newMat;
    }
}
