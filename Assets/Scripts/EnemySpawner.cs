using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;

    [Header("Pathing")]
    public BezierPath[] availablePaths; // Assign your Quadratic and Cubic path objects here

    private float spawnTimer = 0f;

    void Update()
    {
        // Countdown the timer
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval; // Reset the timer
        }
    }

    void SpawnEnemy()
    {
        // Safety check to ensure paths are assigned
        if (availablePaths.Length == 0)
        {
            Debug.LogWarning("No paths assigned to the spawner!");
            return;
        }

        // 1. Pick a random path from the array
        BezierPath selectedPath = availablePaths[Random.Range(0, availablePaths.Length)];

        // 2. Get the exact starting position of that path (t = 0)
        Vector3 spawnPosition = selectedPath.GetPoint(0);

        // 3. Spawn the enemy at the start of the path
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // 4. Assign the selected path to the enemy script so it knows where to go
        Enemy enemyScript = enemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.path = selectedPath;
        }
    }
}