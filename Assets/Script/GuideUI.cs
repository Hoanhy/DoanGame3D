using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public GameObject guidePanel;

    void Start()
    {
        guidePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (guidePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        guidePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}