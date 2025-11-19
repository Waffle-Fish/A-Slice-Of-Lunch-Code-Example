using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set;}
    private CinemachineVirtualCamera GameCamera;
    private CinemachineVirtualCamera PauseCamera;
    private List<CinemachineVirtualCamera> cameras = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        GetComponentsInChildren<CinemachineVirtualCamera>(true, cameras);

        if (cameras[0].name != "Game Camera") Debug.LogError("Cam 0 is " + cameras[0].name + " and NOT GAME CAMERA");
        else GameCamera = cameras[0];

        if (cameras[1].name != "Pause Camera") Debug.LogError("Cam 1 is " + cameras[1].name + " and NOT PAUSE CAMERA");
        else PauseCamera = cameras[1];
    }

    public void EnablePauseCamera()
    {
        PauseCamera.gameObject.SetActive(true);
        GameCamera.gameObject.SetActive(false);
    }
    
    public void EnableGameCamera()
    {
        PauseCamera.gameObject.SetActive(false);
        GameCamera.gameObject.SetActive(true);
    }
}
