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

    // Variables for checking if the player is below a platform
    [SerializeField] private Transform ceilingCheck; // A small empty GameObject above the player to check if there’s a ceiling/platform above
    [SerializeField] private float ceilingCheckRadius = 0.2f; // Radius of the check
    private bool isBelowCeiling;

    // New variable to check if the player is attempting to pass through a platform
    [SerializeField] private LayerMask passablePlatformsLayer; // Set this in the editor for passable platforms
    private bool isPassingThrough;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (enable)
        {
            HandleMovement();
            HandleJump();
        }
    }

    // Handles horizontal movement
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Only apply movement if player is allowed to move
        if (enable)
        {
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Flip character direction when moving left/right
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    // Handles jumping and platform passing
    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isBelowCeiling = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround); // Check if the player is below a ceiling/platform

        // Check if the player is pressing down to pass through a platform
        isPassingThrough = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, passablePlatformsLayer) && Input.GetKey(KeyCode.S);

        // Jump only if the player is grounded, not blocked by a ceiling, and not passing through a platform
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isBelowCeiling && !isPassingThrough)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce); // Apply jump force
        }

        // Allow the player to pass through a platform if they are below and pressing down (and not grounded)
        if (isPassingThrough)
        {
            body.velocity = new Vector2(body.velocity.x, -jumpForce); // Move the player downwards
        }
    }

    // Flip the character when moving left or right
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the character horizontally
        transform.localScale = scale;
    }
}
