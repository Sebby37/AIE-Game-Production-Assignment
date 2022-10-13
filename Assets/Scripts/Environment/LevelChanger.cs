using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Made by Joseph
public class LevelChanger : MonoBehaviour
{
    
    [Header("Level Trigger Scene Name")]
    public string SceneName = null; // String to hold the next scene name

    // Runs on start to check if there is a scene name loaded into the string
    private void Start()
    {
        if (SceneName == null)
        {
            print("Next scene is unavailable");
        }
    }

    // Loads next scene when player enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
           SceneManager.LoadScene(SceneName);
        }
    }
}
