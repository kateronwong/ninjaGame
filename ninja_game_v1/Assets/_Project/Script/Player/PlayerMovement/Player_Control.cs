using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// backup of Player_Movement ( grappling on not included)
public class Player_Control : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchHeight;
    private float originalHeight;

    [Header("Ground Check")]
    public LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    public bool grounded;
    public float groundCheckRadius = 0.3f;

    public LayerMask wallMask;
    public bool onWall;

    [Header("Slop Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit; 
    private bool exitingSlope;

    [Header("References")]
    public Transform orientation;
    public Transform PlayerCam;
    
    float horizontalInput;
    float verticalInput;

    private Vector3 Movedirection;

    Rigidbody rb;

    [Header("State Monitor")]
    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        inAir
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        originalHeight = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        onWall = Physics.CheckSphere(groundCheck.position, groundCheckRadius, wallMask);

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded || onWall)
            rb.linearDamping = 5;
        else   
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void MyInput()
    {
        //base x, z movement 
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || onWall))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouch 
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, originalHeight, transform.localScale.z);
        }
    }

    private void Movement()
    {
        Movedirection = Vector3.zero;

        if(PlayerCam == null)
        {
            Movedirection += orientation.right * horizontalInput;
			Movedirection += orientation.forward * verticalInput;
        }
        else
        {
            Movedirection += Vector3.ProjectOnPlane(PlayerCam.right, orientation.up).normalized * horizontalInput;
			Movedirection += Vector3.ProjectOnPlane(PlayerCam.forward, orientation.up).normalized * verticalInput;
        }

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
            
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        if (grounded || onWall)	
        {
            rb.AddForce(Movedirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }	
        else if (!grounded || !onWall)
        {
            // apply air control
            Vector3 airDirection = Movedirection.normalized;
            airDirection.y = 0f;
            rb.AddForce(airDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            // apply gravity
            rb.AddForce(Vector3.down * 30 * rb.mass, ForceMode.Force);
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (Input.GetKey(sprintKey) && (grounded || onWall))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded || onWall)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else 
        {
            state = MovementState.inAir;
        }
    }

    private void SpeedControl()
    {
        if (onWall || (OnSlope() && !exitingSlope))
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2 * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;

        }
            
        
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(Movedirection, slopeHit.normal).normalized;
    }
}
