using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameplaySceneName = "LevelSelect"; // chuyển sang màn chọn level

    // Nút Play
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Nút Exit
    public void OnQuitClicked()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit();
    }
}