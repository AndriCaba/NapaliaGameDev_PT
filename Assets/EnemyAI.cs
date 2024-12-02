using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Configurable parameters
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Combat Settings")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float damageAmount = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Detection")]
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Separation Settings")]
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationForce = 2f;

    // Private fields
    private Rigidbody2D body;
    private Transform player;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 nextPoint;
    private bool movingToB = true;
    private bool isGrounded;
    private Animator animator;
    private float direction = 1f; // 1 for right, -1 for left

    private EnemyState currentState;

    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
        }

        // Set patrol points based on initial position
        pointA = transform.position + Vector3.left * patrolDistance;
        pointB = transform.position + Vector3.right * patrolDistance;
        nextPoint = pointB;
        currentState = EnemyState.Patrol;

        IgnoreEnemyCollisions(); // Prevent enemies from colliding with each other
    }

    private void Update()
    {
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform is not assigned.");
            return;
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Wall check
        if (IsWallDetected())
        {
            ChangePatrolDirection();
        }

        // Determine the state based on player's position
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distanceToPlayer <= chaseRange)
            {
                ChangeState(EnemyState.Chase);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }
        }
    }

    private void FixedUpdate()
    {
        ApplySeparationForce(); // Keep enemies apart

        // Handle behavior based on current state
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                body.velocity = new Vector2(0, body.velocity.y); // Stop movement during attack
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return; // Prevent redundant state transitions
        currentState = newState;

        // Handle state transitions
        switch (newState)
        {
            case EnemyState.Patrol:
                animator.SetBool("isMoving", true);
                break;
            case EnemyState.Chase:
                animator.SetBool("isMoving", true);
                break;
            case EnemyState.Attack:
                animator.SetBool("isMoving", false);
                animator.SetTrigger("Attack");
                break;
        }
    }

    private void Patrol()
    {
        // Move between patrol points
        if (Vector2.Distance(transform.position, nextPoint) < 0.2f)
        {
            movingToB = !movingToB;
            nextPoint = movingToB ? pointB : pointA;
            direction *= -1; // Reverse direction
            Flip();
        }

        Vector2 directionVector = (nextPoint - transform.position).normalized;
        body.velocity = new Vector2(directionVector.x * speed, body.velocity.y);
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // Chase the player
        Vector2 directionVector = (player.position - transform.position).normalized;
        body.velocity = new Vector2(directionVector.x * speed, body.velocity.y);

        // Jump if the player is higher
        if (isGrounded && player.position.y > transform.position.y + 0.5f)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        // Flip based on player position
        if ((directionVector.x > 0 && direction < 0) || (directionVector.x < 0 && direction > 0))
        {
            direction = Mathf.Sign(directionVector.x);
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = direction * Mathf.Abs(scale.x); // Flip based on direction
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deal damage to player on collision
        if (collision.CompareTag("Player"))
        {
            HealthBar healthBar = collision.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageAmount);
            }
        }
    }

    private bool IsWallDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private void ChangePatrolDirection()
    {
        movingToB = !movingToB;
        nextPoint = movingToB ? pointB : pointA;
        direction *= -1; // Reverse direction
        Flip();
    }

    private void IgnoreEnemyCollisions()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy == gameObject) continue;

            Collider2D[] enemyColliders = enemy.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                foreach (Collider2D enemyCollider in enemyColliders)
                {
                    Physics2D.IgnoreCollision(collider, enemyCollider);
                }
            }
        }
    }

    private void ApplySeparationForce()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy == gameObject) continue;

            Vector2 direction = transform.position - enemy.transform.position;
            float distance = direction.magnitude;

            if (distance < separationRadius && distance > 0)
            {
                body.AddForce(direction.normalized * separationForce / distance, ForceMode2D.Impulse);
            }
        }
    }
}
