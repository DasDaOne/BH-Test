using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject connectedCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        CameraManager.Instance.InitializeCamera(connectedCamera);
        
        transform.LookAt(new Vector3(0, transform.position.y, 0));
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
        
        CameraManager.Instance.ForgetCamera(connectedCamera);
    }

    private void Update()
    {
        if(!isLocalPlayer) return;
        
        Movement();
    }

    private void Movement()
    {
        
    }
}
