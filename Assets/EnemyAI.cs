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

    // Private fields
    private Rigidbody2D body;
    private Transform player;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 nextPoint;
    private bool movingToB = true;
    private bool isGrounded;
    private Animator animator;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set patrol points based on initial position
        pointA = transform.position + Vector3.left * patrolDistance;
        pointB = transform.position + Vector3.right * patrolDistance;
        nextPoint = pointB;
        currentState = EnemyState.Patrol;
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Wall check
        if (IsWallDetected())
        {
            ChangePatrolDirection();
        }

        // Determine the state based on player's position
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

    private void FixedUpdate()
    {
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
                // No movement in attack state
                body.velocity = new Vector2(0, body.velocity.y);
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
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
            Flip();
        }

        Vector2 direction = (nextPoint - transform.position).normalized;
        body.velocity = new Vector2(direction.x * speed, body.velocity.y);
    }

    private void ChasePlayer()
    {
        // Chase the player
        Vector2 direction = (player.position - transform.position).normalized;
        body.velocity = new Vector2(direction.x * speed, body.velocity.y);

        // Jump if the player is higher
        if (isGrounded && player.position.y > transform.position.y + 0.5f)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        // Flip based on player position
        if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(body.velocity.x) * Mathf.Abs(scale.x); // Flip based on velocity direction
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

    // Check if the enemy hits a wall or obstacle
    private bool IsWallDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(transform.localScale.x), wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    // Change the patrol direction when a wall is detected
    private void ChangePatrolDirection()
    {
        movingToB = !movingToB;
        nextPoint = movingToB ? pointB : pointA;
        Flip();
    }
}
