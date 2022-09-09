using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private List<GameObject> initializedCameras = new List<GameObject>();

    public void InitializeCamera(GameObject newCamera, bool enable = true)
    {
        initializedCameras.Add(newCamera);
        if(enable)
            EnableCamera(newCamera);
    }

    public void ForgetCamera(GameObject cameraToForget)
    {
        initializedCameras.Remove(cameraToForget);
        EnableCamera(initializedCameras.First());
    }
    
    private void EnableCamera(GameObject cameraToEnable)
    {
        foreach (GameObject initializedCamera in initializedCameras)
        {
            initializedCamera.SetActive(false);
        }
        cameraToEnable.SetActive(true);
    }
}
