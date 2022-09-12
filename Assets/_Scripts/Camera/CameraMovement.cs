using UnityEngine;
using Utilities;

public class CameraMovement : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private bool isSceneCamera;
    [SerializeField] private bool isDelayed;
    [SerializeField] private Transform centerPoint;

    
    [Header("Input settings")] 
    [SerializeField] private float sensitivity;

    [Header("Bounds")] 
    [SerializeField, Range(-80, 80)] private float xRotationMin;
    [SerializeField, Range(-80, 80)] private float xRotationMax;
    
    private bool delayEnded;
    
    private bool canSpin;
    public bool CanSpin => canSpin;

    private void Start()
    {
        canSpin = !isSceneCamera;

        SwitchNetworkPanel(!canSpin);

        if (isSceneCamera)
        {
            CameraManager.Instance.InitializeCamera(this);
            transform.RotateAround(centerPoint.position, transform.right, (xRotationMin + xRotationMax) / 2);
        }
        
        transform.LookAt(centerPoint);

        if (isDelayed)
        {
            StartCoroutine(TimeUtilities.Timer(0.1f, () => delayEnded = true));
        }
    }

    private void Update()
    {
        if(isDelayed && !delayEnded) return;

        CheckInputs();
        
        SpinCamera();
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            canSpin = !canSpin;
            SwitchNetworkPanel(!canSpin);
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

    private void SwitchNetworkPanel(bool state)
    {
        UiPanelManager.Instance.SetPanelState(PanelId.Network, state);
        
        if(state)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
