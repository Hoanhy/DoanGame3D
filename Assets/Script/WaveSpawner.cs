using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public int meleeCount;
    public int rangedCount;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefabs")]
    public GameObject meleeEnemy;
    public GameObject rangedEnemy;

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    public float spawnDelay = 0.5f;

    int currentWave = 0;

    void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        while (currentWave < waves.Count)
        {
            Wave wave = waves[currentWave];

            Debug.Log("Wave " + (currentWave + 1));

            yield return StartCoroutine(SpawnEnemies(wave));

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            currentWave++;

            yield return new WaitForSeconds(3f);
        }

        Debug.Log("ALL WAVES COMPLETED");
    }

    IEnumerator SpawnEnemies(Wave wave)
    {
        for (int i = 0; i < wave.meleeCount; i++)
        {
            SpawnEnemy(meleeEnemy);
            yield return new WaitForSeconds(spawnDelay);
        }

        for (int i = 0; i < wave.rangedCount; i++)
        {
            SpawnEnemy(rangedEnemy);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemy, point.position, Quaternion.identity);
    }
}