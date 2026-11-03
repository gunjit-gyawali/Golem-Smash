using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float speed = 5f;
    private float moveInput;

    playerStateList playerState;
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float groundCheckX = 0.5f;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.15f; // how long a jump press is remembered
    private float jumpBufferCounter;

    void Awake()
    {
        playerState = GetComponent<playerStateList>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(moveInput, 0f, 0f)
                           * speed * Time.deltaTime;

        transform.Translate(movement);
    }

    void Update()
    {
        Move();
        HandleJumpBuffer();
    }

    private void HandleJumpBuffer()
    {
        // Count down the buffer every frame
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // If a jump was requested recently and we're now grounded, jump
        if (jumpBufferCounter > 0f && Grounded())
        {
            Jump();
            jumpBufferCounter = 0f; // consume the buffered input
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

    // Input callback: just records the press; Update decides when to actually jump
    void OnJump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        playerState.jumping = true;
    }
}