using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public Transform pointA; // First position (assigned via a GameObject in the scene)
    public Transform pointB; // Second position (assigned via a GameObject in the scene)
    public float speed = 2f; // Movement speed

    private Transform target;

    void Start()
    {
        target = pointB; // Start moving toward point B
    }

    void Update()
    {
        // Move the platform toward the target position
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if the platform reached the target position
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            // Switch to the other target position
            target = target == pointA ? pointB : pointA;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Parent the player to the platform
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Unparent the player from the platform
            collision.collider.transform.SetParent(null);
        }
    }
}
