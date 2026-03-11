using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameplaySceneName = "Scene1"; // Tên scene đầu

    // Hàm gán cho nút Play
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Hàm gán cho nút Exit
    public void OnQuitClicked()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit();
    }
}