using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{

    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float GroundDrag;
    private bool isMoving;
    private Vector2 _move; // for input system
    private bool SprintPressed; // for input system

    //for smoothly speed changing 
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    [SerializeField] private float speedChangeFactor;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    bool readyToJump = true;
    private bool JumpPressed; // for input system

    [Header("Dash")]
    public float dashSpeed;
    public bool dashing;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchHeight;
    private float originalHeight;
    private bool CrouchPressed; // for input system

    [Header("Grapping")]
    public bool freeze;
    [SerializeField] private bool activeGrapple;
    [SerializeField] private float grappingSpeedMu;
    private bool enableMovementOnNextTouch;

    [Header("Swing")]
    [SerializeField] private float SwingSpeed;
    public bool swinging;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool grounded;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Wall Handle")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private static bool onWall;
    private bool exitingWall;
    [SerializeField] private float wallJumpForce;

    [Header("Slop Handle")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float SlopDownforce;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    [SerializeField] private static bool onSlop;

    [Header("Step Handle")]
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float stepSmooth = 2f;
    [SerializeField] private float UpperRayLength, LowerRayLength;


    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform PlayerCam;
    [SerializeField] private Grappling GrapplingScript;
    [SerializeField] private Transform playerobj;
    [SerializeField] private PlayerController playerController;

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
        dashing,
        freeze,
        swinging,
        grappling,
        inAir
    }

    //animation 
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private bool Anim_isJumping;
    private bool Anim_isFalling;
    private bool Anim_isGrounded;
    private bool Anim_isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Start is called before the first frame update
    private void Start()
    {

        rb.freezeRotation = true;

        readyToJump = true;

        originalHeight = transform.localScale.y;

        InputHandler();
    }

    // Update is called once per frame
    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        onWall = Physics.CheckSphere(groundCheck.position, groundCheckRadius, wallMask);

        MyInput();
        SpeedControl();
        StateHandler();

        if ((grounded || onWall) && !activeGrapple && !dashing)
            rb.linearDamping = GroundDrag;
        else   
            rb.linearDamping = 0;

        //animation 
        if (animator != null)
            AnimationState();

        Debug.Log("speed : " + rb.linearVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        Movement();

        if (isMoving && readyToJump)
            stepClimb();
    }

    // User input Handle 
    private void MyInput()
    {
        //base x, z movement 
        horizontalInput = _move.x;
        verticalInput = _move.y;

        if (horizontalInput != 0 || verticalInput != 0)
            isMoving = true;
        else isMoving = false;

        // Jump
        if (JumpPressed && readyToJump && (grounded || onWall) && !freeze)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        
    }

    //Player movement handle ( speed & direction )
    private void Movement()
    {
        if (activeGrapple) return;
        if (swinging) return;

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

        if (onSteepSlope())
        {
            Vector3 slideDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up,slopeHit.normal);
            rb.AddForce( - slideDirection * moveSpeed * 50f, ForceMode.Force);
        }

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
            
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * SlopDownforce, ForceMode.Force);
        }
        else if (grounded || onWall)	
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

        if (OnSlope())
            onSlop = true;
        else 
            onSlop = false;

    }

    //State Handle
    private void StateHandler()
    {
        if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = SwingSpeed;
        }
        else if (activeGrapple)
        {
            state = MovementState.grappling;
        }
        else if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 1;
            rb.linearVelocity = Vector3.zero;
        }
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
        }
        else if (CrouchPressed)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (SprintPressed && (grounded || onWall))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded || onWall)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else 
        {
            state = MovementState.inAir;
        }

        bool speedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (speedHasChanged)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            StopAllCoroutines();
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    //Speed Control Function
    private void SpeedControl()
    {
        if (activeGrapple) return;



        if (exitingWall) 
            return;
        else if ((onWall) || (OnSlope() && !exitingSlope))
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

    //for speed smoothing 
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time/difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;

    }

    //Jumping Function
    private void Jump()
    {

        exitingSlope = true;
            
        if (onWall)
        {
            exitingWall = true;

            Vector3 Jumpdir = Vector3.Lerp(Vector3.up, transform.up, 0.5f) ;
            if ((Jumpdir.x < 0.1f && Jumpdir.y < 0.1f && Jumpdir.z < 0.1f))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(transform.up * jumpForce * 0.5f, ForceMode.Impulse);
            } 
            else if (Jumpdir.y >= 0.9f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
            else 
                rb.AddForce(Vector3.Lerp(Vector3.up, transform.up, 0.5f) * wallJumpForce, ForceMode.Impulse);
                
        }
        else 
        {
            // reset y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        //animation 
        if (animator != null)
        {
            Anim_isJumping = true;
            animator.SetBool("isJumping", Anim_isJumping);
        }
        
    }

    private void ResetJump()
    {
        exitingSlope = false;

        exitingWall = false;

        readyToJump = true;

        //animation 
        if (animator != null)
        {
            Anim_isJumping = false;
            animator.SetBool("isJumping", Anim_isJumping);
        }
    }

    //Slop Handle Functions
    public bool OnSlope()
    {
        if (!grounded) return false;

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2 * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;

        }
             
        return false;
    }

    private bool onSteepSlope()
    {
        if (!grounded) return false;

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2 * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > maxSlopeAngle && angle != 0;
        }
             
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(Movedirection, slopeHit.normal).normalized;
    }

    
    void stepClimb()
    {
        if (onSlop) return;

        Collider[] lowerColliders = Physics.OverlapSphere(stepRayLower.transform.position, LowerRayLength);
        Collider[] upperColliders = Physics.OverlapSphere(stepRayUpper.transform.position, UpperRayLength);

        bool hitLower = false;
        bool hitUpper = false;

        foreach (Collider collider in lowerColliders)
        {
            if (collider.gameObject != playerobj)
            {
                hitLower = true;
                break;
            }
        }

        foreach (Collider collider in upperColliders)
        {
            if (collider.gameObject != playerobj)
            {
                hitUpper = true;
                break;
            }
        }

        if (hitLower && !hitUpper)
        {
            Vector3 upward = playerobj.up;
            rb.position += upward * (stepSmooth * Time.deltaTime);
        }

    }

    //debug drawing 
    void OnDrawGizmos()
    {
        //Debug.DrawLine(stepRayLower.transform.position, stepRayLower.transform.position + orientation.forward * 0.1f, Color.red);
        Debug.DrawLine(stepRayLower.transform.position, stepRayLower.transform.position + orientation.TransformDirection(Vector3.forward) * LowerRayLength, Color.red);
        //Debug.DrawLine(stepRayUpper.transform.position, stepRayUpper.transform.position + orientation.forward* 0.2f, Color.red);
        Debug.DrawLine(stepRayUpper.transform.position, stepRayUpper.transform.position + orientation.TransformDirection(Vector3.forward) * UpperRayLength, Color.red);
    }


    //Grappling Functions
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y ;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return (velocityXZ + velocityY) * grappingSpeedMu;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        
        Invoke(nameof(SetVelocity), 0.1f);

        //Invoke(nameof(ResetRestrictions), 10f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GrapplingScript.StopGrapple();
        }
    }

    //animation state function 
    private void AnimationState()
    {
        if (animator ==  null)
            return; 

        Anim_isGrounded = grounded || onWall;
        animator.SetBool("isGrounded", Anim_isGrounded);
        if (!grounded && !onWall)
            Anim_isFalling = true;
        else
            Anim_isFalling = false;
        animator.SetBool("isFalling", Anim_isFalling);
    }

    //get & set function 
    public Vector3 GetVelocity()
    { 
        return Movedirection.normalized * moveSpeed;
    }

    public Vector2 Get_Move()
    {
        return _move;
    }

    public Vector3 GetDirection()
    {
        return Movedirection.normalized;
    }

    public bool GetGrounded()
    {
        return grounded;
    }

    public static bool GetOnWall()
    {
        return onWall;
    }

    public static bool GetonSlop()
    {
        return onSlop;
    }

    //input system 
    private void InputHandler()
    {
        playerController = new PlayerController();
        playerController.Player.Enable();

        playerController.Player.Jump.performed += OnJump;
        playerController.Player.Jump.canceled += OnJump;

        playerController.Player.Move.performed += OnMove;
        playerController.Player.Move.canceled += OnMove;

        //playerController.Player.Dash.performed += OnSprint;
        //playerController.Player.Dash.canceled += OnSprint;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpPressed = true;
        }
        else if (context.canceled)
        {
            JumpPressed = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SprintPressed = true;
        }
        else if (context.canceled)
        {
            SprintPressed = false;
        }
    }
}
