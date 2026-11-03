using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float speed = 5f;
    private float moveInput;
    private float facingDirection = 1f;

    playerStateList playerState;
    private Rigidbody2D rb;
    private float defaultGravity;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float groundCheckX = 0.5f;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Air Jumps")]
    [SerializeField] private int maxAirJumps = 1;
    private int airJumpsLeft;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;
    private bool canDash = true;
    private float dashTimer;
    private float dashCooldownTimer;

    void Awake()
    {
        playerState = GetComponent<playerStateList>();
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        airJumpsLeft = maxAirJumps;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();

        if (moveInput != 0f)
        {
            facingDirection = Mathf.Sign(moveInput);
        }
    }

    private void Move()
    {
        Vector3 movement = new Vector3(moveInput, 0f, 0f)
                           * speed * Time.deltaTime;

        transform.Translate(movement);
    }

    void Update()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (playerState.dashing)
        {
            HandleDash();
            return;
        }

        Move();
        RefillAirJumps();
        RefillDash();
        HandleJumpBuffer();
    }

    private void RefillAirJumps()
    {
        if (Grounded() && rb.linearVelocity.y <= 0.01f)
        {
            airJumpsLeft = maxAirJumps;
        }
    }

    private void RefillDash()
    {
        if (Grounded())
        {
            canDash = true;
        }
    }

    private void HandleJumpBuffer()
    {
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter <= 0f) return;

        if (Grounded())
        {
            Jump();
            jumpBufferCounter = 0f;
        }
        else if (airJumpsLeft > 0)
        {
            Jump();
            airJumpsLeft--;
            jumpBufferCounter = 0f;
        }
    }

    public bool Grounded()
    {
        if (
            Physics2D.Raycast(groundCheckpoint.position, Vector2.down, groundCheckY, WhatIsGround)
            ||
            Physics2D.Raycast(groundCheckpoint.position + new Vector3(groundCheckX, 0f, 0f), Vector2.down, groundCheckY, WhatIsGround)
            ||
            Physics2D.Raycast(groundCheckpoint.position - new Vector3(groundCheckX, 0f, 0f), Vector2.down, groundCheckY, WhatIsGround)
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnJump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        playerState.jumping = true;
    }

    void OnDash()
    {
        if (playerState.dashing || !canDash || dashCooldownTimer > 0f) return;

        StartDash();
    }

    private void StartDash()
    {
        playerState.dashing = true;
        canDash = false;
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
    }

    private void HandleDash()
    {
        dashTimer -= Time.deltaTime;
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

        if (dashTimer <= 0f)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        playerState.dashing = false;
        rb.gravityScale = defaultGravity;
        rb.linearVelocity = Vector2.zero;
    }
}