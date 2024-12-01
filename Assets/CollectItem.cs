using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public GameObject[] itemsToCollect; // Array of items to be collected
    public GameObject objectToAppear; // The object that will appear
    public string playerTag = "Player"; // Tag to identify the player
    public int collectedItemCount = 0; // Counter for collected items

    void Start()
    {
        // Make sure the object to appear is initially hidden
        if (objectToAppear != null)
        {
            objectToAppear.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the specified player tag
        if (collision.CompareTag(playerTag))
        {
            // Loop through all items in the array and destroy any that are collected
            foreach (var item in itemsToCollect)
            {
                if (item != null && item.activeInHierarchy)
                {
                    Destroy(item);  // Destroy the collected item
                    collectedItemCount++;  // Increment the collected items count
                    Debug.Log("Items collected: " + collectedItemCount);  // Display the updated count
                    break; // Exit loop after the first collected item is destroyed
                }
            }

            // Make the other object appear
            if (objectToAppear != null)
            {
                objectToAppear.SetActive(true);
            }
        }
    }
}
