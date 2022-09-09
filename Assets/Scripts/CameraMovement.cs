using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool isSceneCamera;
    
    [Header("Input settings")] 
    [SerializeField] private float sensitivity;

    [Header("Bounds")] 
    [SerializeField, Range(-80, 80)] private float xRotationMin;
    [SerializeField, Range(-80, 80)] private float xRotationMax;
    
    [Header("CameraPositioning")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float distanceToCenterPoint;
    
    private bool canSpin;

    private void Start()
    {
        canSpin = !isSceneCamera;
        SetCursorState(canSpin);

        transform.position = centerPoint.position - transform.forward * distanceToCenterPoint;

        if (isSceneCamera)
        {
            CameraManager.Instance.InitializeCamera(gameObject);
            transform.RotateAround(centerPoint.position, transform.right, (xRotationMin + xRotationMax) / 2);
        }
        
        transform.LookAt(centerPoint);
    }

    private void Update()
    {
        CheckInputs();
        
        SpinCamera();
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            canSpin = !canSpin;
            SetCursorState(canSpin);
        }
    }

    private void SpinCamera()
    {
        if(!canSpin) return;
        
        Vector2 mouseInputs = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        mouseInputs *= sensitivity * Time.deltaTime;

        if (transform.eulerAngles.x < 180)
        {
            if (transform.eulerAngles.x < xRotationMin && mouseInputs.y < 0 ||
                transform.eulerAngles.x > xRotationMax && mouseInputs.y > 0)
                mouseInputs.y = 0;
        }
        else
        {
            if (transform.eulerAngles.x - 360 < xRotationMin && mouseInputs.y < 0 ||
                transform.eulerAngles.x - 360> xRotationMax && mouseInputs.y > 0)
                mouseInputs.y = 0;
        }

        transform.RotateAround(centerPoint.position, transform.up, mouseInputs.x);
        transform.RotateAround(centerPoint.position, transform.right, mouseInputs.y);
        
        transform.LookAt(centerPoint);
    }

    private void SetCursorState(bool state) // true = cursor is locked
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
