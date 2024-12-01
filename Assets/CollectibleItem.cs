using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemManager itemManager;  // Reference to the ItemManager
    public bool isCollected = false;  // Track if the item is already collected

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the specified player tag and the item hasn't been collected
        if (collision.CompareTag(itemManager.playerTag) && !isCollected)
        {
            // Notify the ItemManager that an item has been collected
            itemManager.CollectItem();

            // Mark this item as collected and destroy it
            isCollected = true;
            Destroy(gameObject);  // Destroy the collected item
        }
    }
}
