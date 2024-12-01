using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_boss : MonoBehaviour
{
    public Transform player;           // Reference to the player's position
    public float moveSpeed = 2f;       // Enemy movement speed
    public float attackRange = 5f;     // Range to start attacking
    public float fireRate = 2f;        // Time between attacks
    public GameObject projectilePrefab; // Projectile prefab
    public Transform firePoint;        // Point where projectiles spawn
    public LayerMask groundLayer;      // Layer for ground detection

    private Rigidbody2D rb;            // Reference to Rigidbody2D
    private Animator animator;         // Reference to Animator
    private float nextFireTime = 0f;   // Timer for attack cooldown
    private bool isGrounded;           // Is the enemy on the ground?
    private bool isFacingRight = true; // To track the direction the enemy is facing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        if (player == null) return; // Ensure player reference is set

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if grounded
        isGrounded = CheckIfGrounded();
        animator.SetBool("IsGrounded", isGrounded);

        // Move towards the player if out of attack range
        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        // Attack if in range
        if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            Attack();
            animator.SetTrigger("Attack");
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Flip the enemy to face the player
        FlipTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void Attack()
    {
        // Instantiate the projectile and set its direction
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * 10f; // Set projectile speed
        }
    }

    bool CheckIfGrounded()
    {
        // Raycast to check if grounded
        float rayLength = 0.1f;
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y);
        return Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
    }

    void FlipTowardsPlayer()
    {
        // Determine if the enemy should flip based on player's position
        if (player != null)
        {
            if (player.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }
            else if (player.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip the enemy by scaling its X-axis
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        // Draw the attack range as a red circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Optional: Draw a blue line to the player for debugging
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
