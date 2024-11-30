using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public GameObject itemToCollect; // The item to be collected
    public GameObject objectToAppear; // The object that will appear
    public string playerTag = "Player"; // Tag to identify the player

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
            // Destroy the item
            if (itemToCollect != null)
            {
                Destroy(itemToCollect);
            }

            // Make the other object appear
            if (objectToAppear != null)
            {
                objectToAppear.SetActive(true);
            }
        }
    }
}

