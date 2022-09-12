using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Singletons;


public class CameraManager : Singleton<CameraManager>
{
    private List<CameraMovement> initializedCameras = new List<CameraMovement>();

    public void InitializeCamera(CameraMovement newCamera, bool enable = true)
    {
        initializedCameras.Add(newCamera);
        if(enable)
            EnableCamera(newCamera);
    }

    public void ForgetCamera(CameraMovement cameraToForget)
    {
        initializedCameras.Remove(cameraToForget);
        EnableCamera(initializedCameras.First());
    }
    
    private void EnableCamera(CameraMovement cameraToEnable)
    {
        foreach (CameraMovement initializedCamera in initializedCameras)
        {
            initializedCamera.gameObject.SetActive(false);
        }
        cameraToEnable.gameObject.SetActive(true);
    }
}
