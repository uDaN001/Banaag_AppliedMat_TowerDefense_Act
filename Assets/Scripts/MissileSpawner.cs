using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    public GameObject missilePrefab;
    public float spawnInterval = 3f;

    [Header("Difficulty Scaling")]
    public int baseMissilesPerWave = 1;
    public float difficultyInterval = 10f;

    private float spawnTimer = 0f;
    private float timeSurvived = 0f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        int extraMissiles = Mathf.FloorToInt(timeSurvived / difficultyInterval);
        int currentWaveCount = baseMissilesPerWave + extraMissiles;

        for (int i = 0; i < currentWaveCount; i++)
        {
            Vector3 spawnPos = GetPositionOutside2DCameraView();
            Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetPositionOutside2DCameraView()
    {
        int edge = Random.Range(0, 4);
        float viewportX = 0f;
        float viewportY = 0f;

        switch (edge)
        {
            case 0: 
                viewportX = -0.2f;
                viewportY = Random.Range(-0.2f, 1.2f);
                break;
            case 1: 
                viewportX = 1.2f;
                viewportY = Random.Range(-0.2f, 1.2f);
                break;
            case 2: 
                viewportX = Random.Range(-0.2f, 1.2f);
                viewportY = 1.2f;
                break;
            case 3: 
                viewportX = Random.Range(-0.2f, 1.2f);
                viewportY = -0.2f;
                break;
        }

      
        float zDistance = Mathf.Abs(mainCam.transform.position.z);
        Vector3 worldPoint = mainCam.ViewportToWorldPoint(new Vector3(viewportX, viewportY, zDistance));
        worldPoint.z = 0f; 
        return worldPoint;
    }
}