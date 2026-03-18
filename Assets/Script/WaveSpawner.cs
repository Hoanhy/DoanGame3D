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
    public bool autoStart = false; // Tick vào nếu muốn quái tự ra không cần NPC gọi

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    
    [Header("Enemy Prefabs")]
    public GameObject meleeEnemy;
    public GameObject rangedEnemy;

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();
    public float spawnDelay = 0.5f;

    [Header("UI Kết thúc (Tùy chọn)")]
    public GameObject victoryPanel;

    [Header("Hành động khi thắng lợi")]
    public UnityEvent onWavesCompleted;

    int currentWave = 0;
    private bool isSpawningStarted = false;

    // Hàm này sẽ tự động kiểm tra ngay khi mở Scene
    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    // Hàm này dành cho NPC hoặc Nút bấm gọi
    public void StartSpawning()
    {
        if (!isSpawningStarted)
        {
            isSpawningStarted = true;
            StartCoroutine(StartWave());
            Debug.Log("Hệ thống: Bắt đầu thả quái!");
        }
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

        // Bật UI Chiến thắng (Nếu có)
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // BÁO CÁO CHIẾN THẮNG CHO NPC
        if (onWavesCompleted != null)
        {
            onWavesCompleted.Invoke();
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
        Instantiate(enemy, point.position, Quaternion.identity);
    }
}