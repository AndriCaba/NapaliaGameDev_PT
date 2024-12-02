using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is the player
        if (collision.collider.CompareTag("Player"))
        {
            // Try to get the HealthBar component from the player
            HealthBar healthBar = collision.collider.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                // Apply damage to the player
                healthBar.TakeDamage(damageAmount);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collided object is the player
        if (collider.CompareTag("Player"))
        {
            // Try to get the HealthBar component from the player
            HealthBar healthBar = collider.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                // Apply damage to the player
                healthBar.TakeDamage(damageAmount);
            }
        }
    }
}
