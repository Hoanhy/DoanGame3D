using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        int levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        if (levelUnlocked < 2)
            level2Button.interactable = false;

        if (levelUnlocked < 3)
            level3Button.interactable = false;
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Scene2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Scene3");
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("MenuGame");
    }
}