using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

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
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);

        volumeSlider.value = AudioListener.volume;
        SetVolume(volumeSlider.value);
    }

    void Update()
    {
        if (!gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
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

    // ===== GAME OVER =====
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        SceneManager.LoadScene("MenuGame");
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            // 1. KHI PAUSE: Thả chuột ra để người chơi bấm UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            // 2. KHI TẮT PAUSE (Bằng nút ESC): Giấu chuột đi để chơi tiếp
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    // ===== SETTINGS =====

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