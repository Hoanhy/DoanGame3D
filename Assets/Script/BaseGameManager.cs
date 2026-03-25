using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseGameManager : MonoBehaviour
{
    [Header("=== GIAO DIỆN CHUNG TẤT CẢ CÁC MÀN ===")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject settingPanel;
    public GameObject winGamePanel;

    [Header("Settings")]
    public TextMeshProUGUI volumePercentText;
    public Slider volumeSlider;

    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;

    [Header("Fullscreen Settings")]
    public Toggle fullscreenToggle;

    protected bool isPaused = false; // protected để các script con có thể đọc được biến này

    [Header("=== Quản lý hiển thị chồng chéo ===")]
    public GameObject gameplayHUD;

    protected virtual void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (settingPanel) settingPanel.SetActive(false);
        if (winGamePanel) winGamePanel.SetActive(false);

        if (volumeSlider)
        {
            volumeSlider.value = AudioListener.volume;
            SetVolume(volumeSlider.value);
        }
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        if (sensitivitySlider != null)
        {
            // Load lại giá trị cũ, nếu chưa có thì mặc định là 1.0
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        }
    }

    // Hàm Update này chỉ chuyên rình nút ESC
    protected virtual void Update()
    {
        bool isGameOver = (gameOverPanel != null && gameOverPanel.activeSelf) || (winGamePanel != null && winGamePanel.activeSelf);

        // Không cho Pause nếu đã Thắng hoặc Thua
        if (!isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel != null && settingPanel.activeSelf)
            {
                BackToPause();
            }
            else
            {
                TogglePause();
            }
        }
    }

    // ===== HỆ THỐNG PAUSE VÀ CHUỘT THỐNG NHẤT =====

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel) pausePanel.SetActive(isPaused);

        if (gameplayHUD)
        {
            gameplayHUD.SetActive(!isPaused);
        }

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None; // Thả chuột
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked; // Khóa chuột
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (settingPanel) settingPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSetting()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (settingPanel) settingPanel.SetActive(true);
    }

    public void BackToPause()
    {
        if (settingPanel) settingPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(true);
    }

    // ===== HỆ THỐNG KẾT THÚC GAME =====

    public virtual void GameOver(string reason = "Hết thời gian!")
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public virtual void WinGame()
    {
        if (winGamePanel) winGamePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ===== NÚT BẤM CHUNG =====

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

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        if (volumePercentText)
        {
            int percent = Mathf.RoundToInt(volume * 100);
            volumePercentText.text = percent + "%";
        }
    }

    public void SetMouseSensitivity(float value)
    {
        // Lưu giá trị
        PlayerPrefs.SetFloat("CameraSensitivity", value);
        PlayerPrefs.Save();

        Debug.Log("<color=green>Đã cập nhật Độ nhạy mới: </color>" + value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Lưu lại trạng thái
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetAllData()
    {
        // Xóa sạch dữ liệu đã lưu trong máy người chơi
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Đã reset toàn bộ dữ liệu game!");

        // Load lại màn hình Menu để cập nhật trạng thái các nút (khóa màn 2, 3)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuGame");
    }
}