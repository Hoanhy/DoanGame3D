using UnityEngine;
using TMPro;

public class Level1Manager : BaseGameManager
{
    public static Level1Manager Instance;

    [Header("=== LOGIC RIÊNG CỦA MÀN 1 ===")]
    public int totalDocuments = 4;
    public int currentDocuments = 0;
    public bool readyToSubmit = false;
    public bool hasStudentCard = false; // Thêm biến này để biết Player đã lấy thẻ chưa


    [Header("Timer")]
    public float timeRemaining = 180f;
    private bool timerRunning = true;

    [Header("UI Thẻ Sinh Viên")]
    public GameObject studentCardPopUp;

    [Header("UI Riêng Màn 1")]
    public TextMeshProUGUI documentText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI missionText;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start(); 

  
        UpdateDocumentUI();
        UpdateTimerUI();
    }

    protected override void Update()
    {
        base.Update(); 

        if (isPaused) return;

        // Đếm giờ
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;

                if (missionText) missionText.text = "Hết thời gian!";
                GameOver(); // Gọi hàm GameOver từ BaseGameManager
            }
            UpdateTimerUI();
        }
    }

    public void CollectDocument()
    {
        currentDocuments++;
        UpdateDocumentUI();

        if (currentDocuments >= totalDocuments)
        {
            readyToSubmit = true;
            if (missionText) missionText.text = "Đã đủ hồ sơ! Hãy gặp giáo viên.";
        }
    }
    public void ReceiveStudentCard()
    {
        hasStudentCard = true;

        // 1. Lệnh hiện Panel ảnh thẻ
        if (studentCardPopUp != null)
        {
            studentCardPopUp.SetActive(true); // Bật bảng lên
            Time.timeScale = 0f;              // Dừng thời gian game
            Cursor.lockState = CursorLockMode.None; // Mở khóa chuột
            Cursor.visible = true;            // Hiện con trỏ chuột
        }

        if (missionText) missionText.text = "Đã nhận thẻ sinh viên! Hãy tìm cổng trường để nhập học.";

        if (documentText != null)
        {
            documentText.gameObject.SetActive(false);
        }

        Debug.Log("Đã nhận thẻ sinh viên, hiện UI thông báo!");
    }

    public void CloseStudentCard()
    {
        if (studentCardPopUp != null)
        {
            studentCardPopUp.SetActive(false); // Ẩn thẻ đi
            Time.timeScale = 1f;             

            // Khóa chuột lại để chơi tiếp
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void UpdateDocumentUI()
    {
        if (documentText) documentText.text = "Hồ sơ: " + currentDocuments + " / " + totalDocuments;
    }

    void UpdateTimerUI()
    {
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = "Thời gian: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
    public override void WinGame()
    {
        //Kiểm tra xem người chơi đã mở khóa Màn 2 chưa?
        int currentUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        if (currentUnlocked < 2)
        {
            PlayerPrefs.SetInt("LevelUnlocked", 2);
            PlayerPrefs.Save(); 
            Debug.Log("Hệ thống: ĐÃ MỞ KHÓA MÀN 2!");
        }

        base.WinGame();
    }
    public void GoToScene2()
    {
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
    }
}