using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject objectToAppear; // The object that will appear after collecting items
    public string playerTag = "Player"; // Tag to identify the player
    public int collectedItemCount = 0; // Counter for collected items
    public int No_ItemCollected = 5; // Number of items required to collect before showing the object

    void Start()
    {
        // Make sure the object to appear is initially hidden
        if (objectToAppear != null)
        {
            objectToAppear.SetActive(false);
        }
    }

    // Method to handle the collection of an item
    public void CollectItem()
    {
        collectedItemCount++;  // Increment the collected items count
        Debug.Log("Items collected: " + collectedItemCount);  // Display the updated count

        // Make the other object appear after collecting the required number of items
        if (collectedItemCount == No_ItemCollected)
        {
            if (objectToAppear != null)
            {
                objectToAppear.SetActive(true);
            }
        }
    }
}
