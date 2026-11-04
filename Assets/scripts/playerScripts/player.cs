using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float speed = 5f;
    private float moveInput;
    private float facingDirection = 1f;

    playerStateList playerState;
    private PlayerInput playerInput;
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
    [SerializeField] private TrailRenderer dashTrail;
    private bool canDash = true;
    private float dashTimer;
    private float dashCooldownTimer;

    [Header("Attack")]
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private Vector2 slashOffset = new Vector2(1f, 0f);
    [SerializeField] private Vector2 slashUpOffset = new Vector2(0f, 1f);
    [SerializeField] private Vector2 slashDownOffset = new Vector2(0f, -1f);
    [SerializeField] private float slashLifetime = 0.2f;
    [SerializeField] private float attackCooldown = 0.4f;
    private float attackCooldownTimer;

    void Awake()
    {
        playerState = GetComponent<playerStateList>();
        playerInput = GetComponent<PlayerInput>();
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

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
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
        dashTrail.emitting = true;
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
        dashTrail.emitting = false;
    }

    void OnAttack()
    {
        if (playerState.dashing || attackCooldownTimer > 0f) return;

        Slash();
    }

    private void Slash()
    {
        attackCooldownTimer = attackCooldown;

        float verticalInput = playerInput.actions["Vertical"].ReadValue<float>();
        bool slashUp = verticalInput > 0.5f;
        bool slashDown = !Grounded() && verticalInput < -0.5f;

        if (slashUp)
        {
            SpawnSlash(slashUpOffset, 90f, 1f);
        }
        else if (slashDown)
        {
            SpawnSlash(slashDownOffset, -90f, 1f);
        }
        else
        {
            Vector2 sideOffset = new Vector2(facingDirection * slashOffset.x, slashOffset.y);
            SpawnSlash(sideOffset, 0f, facingDirection);
        }
    }

    private void SpawnSlash(Vector2 offset, float angle, float flip)
    {
        Vector3 spawnPosition = transform.position + (Vector3)offset;
        GameObject slashObject = Instantiate(slashEffect, spawnPosition, Quaternion.Euler(0f, 0f, angle), transform);

        Vector3 scale = slashObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * flip;
        slashObject.transform.localScale = scale;

        Destroy(slashObject, slashLifetime);
    }
}