using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
//c
public class UITransionManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainMenuCamera;
    public CinemachineVirtualCamera playCamera;
    public CinemachineVirtualCamera mapSelectionCamera;
    public CinemachineVirtualCamera optionsCamera;
    public CinemachineVirtualCamera carSelectorCamera;
    public CinemachineVirtualCamera lobbyCamera;
    public CinemachineVirtualCamera quitCamera;

    public Transform carObject;
    public float moveSpeed = 10f;
    public static bool leftMatch = false;

    private bool isHit = false;

    [SerializeField] private NumberOfConnectionsScriptableObject numberOfConnectionsScriptableObject;
    [SerializeField] private PlayersReadyScriptableObject playersReadyScriptableObject;

    // Start is called before the first frame update
    void Start()
    {
        if (leftMatch)
        {
            ActivateCamera(lobbyCamera);
        }

        ActivateCamera(mainMenuCamera);
    }

    public void TransitionToPlayCamera()
    {
       
        BlendToCamera(playCamera, 1f);
    }

    public void TransitionToOptionsCamera()
    {
       
        BlendToCamera(optionsCamera, 1f);
    }

    public void TransitionToMapSelectionCamera()
    {
        
        BlendToCamera(mapSelectionCamera, 1f);
    }

    public void TransionToCarSelectorCamera()
    {
       
        BlendToCamera(carSelectorCamera, 1f);
    }

    public void TransitionToLobbyCamera()
    {
        leftMatch = true;
        BlendToCamera(lobbyCamera, 1f);
    }

    public void TransionToMainMenuCamera()
    {
        leftMatch = false;
        BlendToCamera(mainMenuCamera, 1f);
    }

    public void TransisionToQuitCamera()
    {
        BlendToCamera(quitCamera, 1f);
        StartCarObjectMove();
    }

    private void StartCarObjectMove()
    {
        isHit = true;
    }

    private void CarObjectMove()
    {
        if(isHit)
        {
            carObject.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
          
        }
    }

    private void Update()
    {
        CarObjectMove();
    }

    private void ActivateCamera(CinemachineVirtualCamera camera)
    {
       
        camera.Priority = 10;
    }

    private void DeactivateCamera(CinemachineVirtualCamera camera)
    {
        
        camera.Priority = 0;
    }

    private void BlendToCamera(CinemachineVirtualCamera camera, float duration)
    {
        DeactivateCamera(mainMenuCamera);
        DeactivateCamera(playCamera);
        DeactivateCamera(mapSelectionCamera);
        DeactivateCamera(optionsCamera);
        DeactivateCamera(carSelectorCamera);

        ActivateCamera(camera);
    }

    public void Play()
    {
        numberOfConnectionsScriptableObject.ResetValues();
        playersReadyScriptableObject.ResetValues();
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}