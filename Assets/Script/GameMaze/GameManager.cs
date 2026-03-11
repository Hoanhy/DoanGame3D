using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Documents")]
    public int totalDocuments = 4;
    public int currentDocuments = 0;

    [Header("Timer")]
    public float timeRemaining = 180f;
    private bool timerRunning = true;

    [Header("UI")]
    public TextMeshProUGUI documentText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI missionText;

    public bool readyToSubmit = false;
    public bool hasStudentCard = false;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Pause UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    [Header("Setting UI")]
    public GameObject settingPanel;
    public TextMeshProUGUI volumePercentText;
    public Slider volumeSlider;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateDocumentUI();
        UpdateTimerUI();
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);
        volumeSlider.value = AudioListener.volume;
        SetVolume(volumeSlider.value); // cập nhật text volume lúc bắt đầu
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                GameOver();
            }

            UpdateTimerUI();
        }
        // Pause bằng ESC (không cho pause khi GameOver)
        if (!gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            // Nếu đang ở Setting -> quay lại Pause
            if (settingPanel.activeSelf)
            {
                BackToPause();
            }
            else
            {
                TogglePause();
            }
        }
    }

    void GameOver()
    {
        missionText.text = "Hết thời gian!";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }
    public void CollectDocument()
    {
        currentDocuments++;
        UpdateDocumentUI();

        if (currentDocuments >= totalDocuments)
        {
            readyToSubmit = true;
            missionText.text = "Đã đủ hồ sơ! Hãy gặp giáo viên.";
        }
    }

    void UpdateDocumentUI()
    {
        documentText.text = "Hồ sơ: " + currentDocuments + " / " + totalDocuments;
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = "Thời gian: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
    // ===== BUTTON FUNCTIONS =====
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuGame"); // tên scene menu của bạn
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OpenSetting()
    {
        pausePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void BackToPause()
    {
        settingPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        int percent = Mathf.RoundToInt(volume * 100);
        volumePercentText.text = percent + "%";
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}