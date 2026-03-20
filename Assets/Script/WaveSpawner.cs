using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Wave
{
    public int meleeCount;
    public int rangedCount;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Cài đặt khởi động")]
    public bool autoStart = false;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefabs")]
    public GameObject meleeEnemy;
    public GameObject rangedEnemy;

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();
    public float spawnDelay = 0.5f;

    [Header("UI & Âm thanh")]
    public GameObject winWaveUI;
    public AudioClip winWaveSound;

    [Header("Hành động khi hoàn thành")]
    public UnityEvent onWavesCompleted;

    int currentWave = 0;
    private bool isSpawningStarted = false;

    void Start()
    {
        if (winWaveUI != null) winWaveUI.SetActive(false);
        if (autoStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (!isSpawningStarted)
        {
            isSpawningStarted = true;
            StartCoroutine(StartFirstWave());
            Debug.Log("Bắt đầu spawn quái");
        }
    }

    IEnumerator StartFirstWave()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        while (currentWave < waves.Count)
        {
            Wave wave = waves[currentWave];

            if (Level3Manager.Instance != null)
            {
                Level3Manager.Instance.ShowWaveStart(currentWave + 1);
            }

            // Wave đầu hiện lâu hơn
            if (currentWave == 0)
                yield return new WaitForSeconds(3f);
            else
                yield return new WaitForSeconds(2f);

            yield return StartCoroutine(SpawnEnemies(wave));

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            if (Level3Manager.Instance != null)
            {
                Level3Manager.Instance.ShowWaveComplete();
            }

            currentWave++;

            yield return new WaitForSeconds(3f);
        }

        Debug.Log("ALL WAVES COMPLETED");

        // Bật UI Thông báo
        if (winWaveUI != null)
        {
            winWaveUI.SetActive(true);
            StartCoroutine(HideWinUIAfterSeconds(3f));
        }

        if (onWavesCompleted != null)
        {
            onWavesCompleted.Invoke();
        }

        if (Level3Manager.Instance != null)
        {
            Level3Manager.Instance.AllWavesCompleted();
        }
    }
    IEnumerator HideWinUIAfterSeconds(float seconds)
    {
        // Chờ đúng số giây bạn muốn
        yield return new WaitForSeconds(seconds);

        // Sau đó tự động giấu bảng UI đi
        if (winWaveUI != null)
        {
            winWaveUI.SetActive(false);
        }
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
        if (spawnPoints.Length == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        Instantiate(enemy, point.position + offset, Quaternion.identity);
    }
}