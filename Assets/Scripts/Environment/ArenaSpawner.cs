using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class ArenaSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Spawn Config")]
    public Rect spawnBox;
    public float timeBetweenSpawn = 2.5f;
    public float maxEnemies = 5.0f;

    List<GameObject> spawnedEnemies = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        // Beginning the coroutine to spawn enemies
        StartCoroutine(SpawnEnemyCoroutine());
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
                    spawnedEnemies.Remove(enemy);

            // Spawning an enemy if the count of enemies spawned is under the maximum
            if (spawnedEnemies.Count < maxEnemies)
                SpawnEnemy();
        }
    }
}
