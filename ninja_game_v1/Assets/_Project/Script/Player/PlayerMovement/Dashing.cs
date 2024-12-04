using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Dashing : MonoBehaviour
{
    [Header("Dashing")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private PlayerController playerController;
    private Vector3 forceToApply;

    [Header("Cooldown")]
    [SerializeField] private float dashCD;
    private float dashCDTimer;
    private bool inCD;

    [Header("References")]
    [SerializeField] private Transform playerCamera;
    private Rigidbody rb;
    private Player_Movement pm;
    private PlayerStatusHandler playerStatusHandler;

    private bool DashPressed;
    public bool dashing;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Player_Movement>();
        playerStatusHandler = GetComponent<PlayerStatusHandler>();  
    }

    private void Start()
    {
        InputHandler();
    }

    private void Update()
    {
        if (dashCDTimer > 0)
        {
            inCD = true;
            dashCDTimer -= Time.deltaTime;
        }
        else     
        {
            inCD = false;
        }
            

        if (DashPressed && playerStatusHandler.canDash && !inCD)
        {
            playerStatusHandler.StaminaDash();
            Dash();
        }
    }

    private void Dash()
    {
        if (dashCDTimer > 0) return;
            else dashCDTimer = dashCD;

        dashing = true;
        pm.dashing = dashing;

        Vector3 direction = CaculateDirection();

        forceToApply = direction * dashForce;

        Invoke(nameof(DelayDash), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {
        dashing = false;
        pm.dashing = dashing;
        DashPressed = false;
    }

    private void DelayDash()
    {
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private Vector3 CaculateDirection()
    {
        Vector3 direction = new Vector3();

        direction = playerCamera.forward * pm.Get_Move().y + playerCamera.right * pm.Get_Move().x;

        if (pm.Get_Move() == Vector2.zero) 
        {
            direction = playerCamera.forward;
        }

        return direction.normalized;
    }

    private void InputHandler()
    {
        playerController = new PlayerController();
        playerController.Player.Enable();

        playerController.Player.Dash.performed += OnDash;
        playerController.Player.Dash.canceled += OnDash;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (! DashPressed)
                DashPressed = true;
        }
        else if (context.canceled)
        {
            DashPressed = false;
        }
    }
}
