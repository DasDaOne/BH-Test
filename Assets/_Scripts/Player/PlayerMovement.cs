using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement properties")] 
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;
    
    private float movementSpeed;
    private bool canMove = true;

    private Vector3 movementDirection;
    
    [Header("Dash properties")] 
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private LayerMask dashCollisionLayerMask;

    private bool isDashing;
    
    private Vector3 dashEndPoint;
    
    [Header("Camera")]
    [SerializeField] private Transform cameraOrientation;
    [SerializeField] private CameraMovement cameraMovement;

    [Header("Other")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform visualModel;
    [SerializeField] private float colliderWidth;
    [SerializeField] private DashColliderChecker dashColliderChecker;


    private void Start()
    {
        visualModel.LookAt(new Vector3(0, transform.position.y, 0));
    }
    
    private void Update()
    {
        if(!isLocalPlayer || !canMove || !cameraMovement.CanSpin) return;

        if(Input.GetKeyDown(KeyCode.Mouse0))
            StartDash();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
       
        Movement(); 
        
        RotatePlayer();

        Dashing();
    }

    private void StartDash()
    {
        dashColliderChecker.CmdStartChecking();
        isDashing = true;
        canMove = false;
        dashEndPoint = GetDashEndPosition();
    }
    
    private Vector3 GetDashEndPosition()
    {
        Vector3 direction;
        if (movementDirection.magnitude > 0.01f)
            direction = movementDirection.normalized;
        else
            direction = visualModel.forward;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit,
            dashDistance, dashCollisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point - direction * colliderWidth;
        }
    
        return transform.position + direction * dashDistance;
    }
    
    private void Dashing()
    {
        if(!isDashing) return;
        
        transform.position = Vector3.MoveTowards(transform.position, dashEndPoint, dashSpeed * Time.fixedDeltaTime);
    
        if ((transform.position - dashEndPoint).magnitude < 0.01f)
        {
            isDashing = false;
            canMove = true;
            dashColliderChecker.CmdStopChecking();
        }
    }

    private void Movement()
    {
        if(!canMove || !cameraMovement.CanSpin) return;

        movementDirection = cameraOrientation.forward * Input.GetAxisRaw("Vertical") +
                            cameraOrientation.right * Input.GetAxisRaw("Horizontal");
        
        movementSpeed = Mathf.Lerp(movementSpeed, maxSpeed, movementAcceleration * Time.fixedDeltaTime);
        
        rb.AddForce(movementDirection.normalized * movementSpeed, ForceMode.Acceleration);
    }
    
    private void RotatePlayer()
    {
        if(movementDirection.magnitude < 0.01f) return;
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized);
         
        visualModel.rotation = Quaternion.Slerp(visualModel.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
