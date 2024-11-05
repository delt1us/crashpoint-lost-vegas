/**************************************************************************************************************
* Camera Controller
* The class allows the player to change the camera's distance from the vehicle by cycling through an array of positions when a button is pressed. 
* Responsible for changing the positioning of the camera (when entering/exiting crates and when cycling camera distances).
*
* Created by Dean Atkinson-Walker 2023
*            
***************************************************************************************************************/

using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera normalCam;
    private CinemachineOrbitalTransposer transposer;

    private Vector3[] distances;
    private short index = 1;

    [SerializeField] private Vector3 closeOffset = new(0, 1, -3);
    [SerializeField] private Vector3 mediumOffset = new(0, 1.2f, -5);
    [SerializeField] private Vector3 farOffset = new(0, 1.4f, -7);

    [SerializeField, Tooltip("Whenever the player enters a container, the camera's height will be ")] 
    private float crateHeightOffset = -.05f;

    [SerializeField, Tooltip("Whenever the player enters a container that is at a slope, this multiplier will be applied to the crate heght offset (if the crate is steep enough).")]
    private float steepMultiplier = 10;

    private void Start()
    {
        normalCam = GetComponent<CinemachineVirtualCamera>();
        transposer = normalCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        distances = new Vector3[] { closeOffset, mediumOffset, farOffset };
        transposer.m_FollowOffset = distances[index];
    }

    private void OnChangeCamera()
    {
        index++;
        if(index >= distances.Length) index = 0;

        transposer.m_FollowOffset = distances[index];
    }

    public void EnterCrate(float crateAngle)
    {
        bool steep = Mathf.Abs(crateAngle) > 60;
        // Lowers the vertical camera offset so that the camera is almost parallel to the car
        transposer.m_FollowOffset = new(distances[index].x, steep? crateHeightOffset * steepMultiplier: crateHeightOffset, distances[index].z);
    }

    public void ExitCrate()
    {
        transposer.m_FollowOffset = distances[index];
    }
}
