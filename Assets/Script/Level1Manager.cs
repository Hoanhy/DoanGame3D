using UnityEngine;
using TMPro;

// CHÚ Ý: Đổi MonoBehaviour thành BaseGameManager
public class Level1Manager : BaseGameManager
{
    public static Level1Manager Instance;

    [Header("=== LOGIC RIÊNG CỦA MÀN 1 ===")]
    public int totalDocuments = 4;
    public int currentDocuments = 0;
    public bool readyToSubmit = false;

    [Header("Timer")]
    public float timeRemaining = 180f;
    private bool timerRunning = true;

    [Header("UI Riêng Màn 1")]
    public TextMeshProUGUI documentText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI missionText;

    void Awake()
    {
        Instance = this;
    }

    // override: Ghi đè lên hàm Start của Mẹ
    protected override void Start()
    {
        base.Start(); // Lệnh này bảo: "Chạy hàm Start của mẹ trước đi (tắt UI, chỉnh âm lượng)"

        // Sau đó mới chạy những thứ riêng của Màn 1
        UpdateDocumentUI();
        UpdateTimerUI();
    }

    protected override void Update()
    {
        base.Update(); // Lệnh này bảo: "Chạy hàm rình nút ESC của mẹ đi"

        // Nếu game đang Pause thì ngưng đếm ngược thời gian
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
                GameOver(); // Gọi hàm GameOver từ BaseGameManager (Người Mẹ)
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
}