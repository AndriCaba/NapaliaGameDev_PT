using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class portalScene : MonoBehaviour
{


    // This flag will ensure the scene is loaded only once when entering the second collider
    public string sceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Load the scene if the object with the "Player" tag enters the collider
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
