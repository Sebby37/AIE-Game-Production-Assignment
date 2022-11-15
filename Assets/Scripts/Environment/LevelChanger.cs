using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Made by Joseph
public class LevelChanger : MonoBehaviour
{
    
    [Header("Level Trigger Scene Name")]
    public string sceneName = null; // String to hold the next scene name

    [Header("Name of the last scene")]
    public static string lastSceneName;  // String to hold the last loaded scene

    // Runs on start to check if there is a scene name loaded into the string
    private void Start()
    {
        if (sceneName == null)
        {
            print("Next scene is unavailable");
        }
    }

    // Loads next scene when player enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
           lastSceneName = SceneManager.GetActiveScene().name;
           SceneManager.LoadScene(sceneName);
        }
    }
}
