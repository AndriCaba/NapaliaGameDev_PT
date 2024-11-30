using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalCollider : MonoBehaviour
{
    public string playerTag = "Player";
    public string sceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object collided has the specified player tag
        if (other.gameObject.CompareTag(playerTag))
        {
            // Ensure the scene name is not empty
            if (!string.IsNullOrEmpty(sceneName))
            {
                // Debug log to confirm the portal is activated
                Debug.Log("Portal activated. Loading scene: " + sceneName);

                // Load the scene by name
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // If the scene name is not set, log an error
                Debug.LogError("Scene name is not set!");
            }
        }
    }
}
