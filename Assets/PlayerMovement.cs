using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 10f; // Adjustable force for the jump
  public bool enable = true;
    private Rigidbody2D body;
    private Animator animator;
    private bool facingRight = true;

    // Ground checking variables
    [SerializeField] private Transform groundCheck; // Assign a child GameObject to check for ground
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround; // Set this in the editor
    private bool isGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        float moveInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce); // Apply jump force
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the character horizontally
        transform.localScale = scale;
    }
}
