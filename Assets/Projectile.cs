using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 10f; // Damage dealt by the projectile

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after a certain time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile hits the player
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit!");

            // Get the HealthBar component from the player and apply damage
            HealthBar healthBar = collision.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy the projectile
        }
    }
}
