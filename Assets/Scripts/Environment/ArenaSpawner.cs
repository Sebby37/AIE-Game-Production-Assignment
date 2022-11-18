using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class ArenaSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject boss;

    [Header("Spawn Config")]
    public Rect spawnBox;
    public float timeBetweenSpawn = 2.5f;
    public float maxEnemies = 5.0f;
    public int enemiesKilledBeforeBoss = 30;
    public int enemiesKilledBeforeSpawnRateIncrease = 10;

    int enemiesKilled = 0;
    bool bossSpawned = false;
    List<GameObject> spawnedEnemies = new List<GameObject>();
    GameObject spawnedBoss;
    
    // Start is called before the first frame update
    void Start()
    {
        // Beginning the coroutine to spawn enemies
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private void Update()
    {
        // Spawning a boss after a certain amount of enemies have been killed
        if (boss != null && enemiesKilled % enemiesKilledBeforeBoss == 0 && enemiesKilled != 0 && !bossSpawned)
            SpawnBoss();

        // Incrementing the enemies killed counter if the boss is dead
        if (bossSpawned && spawnedBoss == null)
        {
            enemiesKilled++;
            bossSpawned = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(spawnBox.center.x, spawnBox.center.y, 0.01f), new Vector3(spawnBox.size.x, spawnBox.size.y, 0.01f));
    }

    // Function to spawn an enemy
    [ContextMenu("Spawn Enemy")]
    void SpawnEnemy()
    {
        // Calculating the spawn position
        float xPos = Random.Range(spawnBox.x, spawnBox.x + spawnBox.width);
        float yPos = Random.Range(spawnBox.y, spawnBox.y + spawnBox.height);
        Vector2 spawnPos = new Vector2(xPos, yPos);

        // Spawning a random enemy to the calculated coordinates
        GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Count)];
        GameObject spawnedEnemy = Instantiate(enemyToSpawn);
        spawnedEnemy.transform.position = spawnPos;

        // Adding the spawned enemy to the list
        spawnedEnemies.Add(spawnedEnemy);
    }

    // Function to spawn the boss
    void SpawnBoss()
    {
        // Setting the boss to be spawned
        bossSpawned = true;

        // Instantiating the boss
        spawnedBoss = Instantiate(boss, new Vector2(0.0f, -2.0f), Quaternion.identity);

        // Starting the boss intro
        GolemBoss bossScript = spawnedBoss.GetComponent<GolemBoss>();
        bossScript.Intro();
    }

    // Coroutine to run and spawn an enemy
    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            // Delaying between enemy spawn
            yield return new WaitForSeconds(timeBetweenSpawn * Random.Range(0.75f, 1.25f));

            // Clearing dead enemies from the list of spawned enemies
            foreach (GameObject enemy in spawnedEnemies.ToArray())
                if (enemy == null)
                {
                    spawnedEnemies.Remove(enemy);
                    enemiesKilled++;
                }

            // Spawning an enemy if the count of enemies spawned is under the maximum
            if (!bossSpawned && spawnedEnemies.Count < maxEnemies + Mathf.Floor(enemiesKilled / (float) enemiesKilledBeforeSpawnRateIncrease))
                SpawnEnemy();
        }
    }
}
