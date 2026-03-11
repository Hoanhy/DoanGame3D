using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
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