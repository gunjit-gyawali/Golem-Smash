
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float speed = 30f;
    private float moveInput;

    private Rigidbody2D rb;
    private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float groundCheckX = 0.5f;


    void Awake()
    {
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
        if (Grounded())
        {
            Debug.Log("Jumping");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
