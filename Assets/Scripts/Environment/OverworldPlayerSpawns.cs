using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayerSpawns : MonoBehaviour
{
    [Header("World Spawn Positions")]
    public GameObject tutorialSpawn;
    public GameObject fireDungeonSpawn;
    
    private Transform playerPosition;
    [Header("Last Scene")]
    public string lastScene;
    
    
    // Start is called before the first frame update
    void Start()
    {
        lastScene = LevelChanger.lastSceneName;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        switch (lastScene)
        {
            case "Tutorial":
                playerPosition.position = tutorialSpawn.transform.position;
                break;
            case "FireDungeon":
                playerPosition.position = fireDungeonSpawn.transform.position;
                break;
            default:
                playerPosition.position = tutorialSpawn.transform.position;
                break;
        }
    }
}
