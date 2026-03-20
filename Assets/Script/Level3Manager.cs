using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Manager : BaseGameManager
{
    public static Level3Manager Instance;

    [Header("UI Win")]
    public GameObject winPanel;

    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (gameEnded && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MenuGame");
        }
    }

    // Đồ án vỡ → thua
    public void ProjectDestroyed()
    {
        if (gameEnded) return;

        gameEnded = true;
        GameOver();
    }

    // Player chết → thua
    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        GameOver();
    }

    // Hoàn thành tất cả wave → thắng
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