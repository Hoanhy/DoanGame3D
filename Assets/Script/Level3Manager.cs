using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Level3Manager : BaseGameManager
{
    public static Level3Manager Instance;

    [Header("UI Win")]
    public GameObject winPanel;

    [Header("Wave UI")]
    public GameObject wavePanel;
    public TextMeshProUGUI waveText;

    private bool gameEnded = false;
    private Coroutine messageCoroutine;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        if (winPanel != null)
            winPanel.SetActive(false);

        if (wavePanel != null)
            wavePanel.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (gameEnded && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MenuGame");
        }
    }

    public void ShowWaveStart(int waveNumber)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        if (waveNumber == 1)
        {
            messageCoroutine = StartCoroutine(ShowMessage("Bắt đầu bảo vệ đồ án! Hội đồng đang đặt câu hỏi", 3f));
        }
        else
        {
            messageCoroutine = StartCoroutine(ShowMessage("Đợt câu hỏi tiếp theo", 2f));
        }
    }

    public void ShowWaveComplete()
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(ShowMessage("Hoàn thành đợt câu hỏi", 2f));
    }

    IEnumerator ShowMessage(string message, float duration)
    {
        wavePanel.SetActive(true);
        waveText.text = message;

        yield return new WaitForSeconds(duration);

        wavePanel.SetActive(false);
    }

    public void ProjectDestroyed()
    {
        if (gameEnded) return;

        gameEnded = true;
        GameOver();
    }

    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        GameOver();
    }

    public void AllWavesCompleted()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}