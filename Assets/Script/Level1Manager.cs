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
    public bool hasStudentCard = false; // Thêm biến này để biết Player đã lấy thẻ chưa

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
    public void ReceiveStudentCard()
    {
        hasStudentCard = true;
        if (missionText) missionText.text = "Đã nhận thẻ sinh viên! Hãy chạy ra cổng trường để thoát.";

        // (Tùy chọn) Nếu bạn có một bức tường tàng hình chắn ở cổng, bạn có thể tắt nó đi ở đây
        Debug.Log("Hệ thống: Đã nhận thẻ sinh viên, có thể qua cổng!");
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
        // 1. Kiểm tra xem người chơi đã mở khóa Màn 2 chưa?
        int currentUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        // 2. Nếu chưa tới Màn 2, thì lưu đè lên để mở khóa Màn 2
        if (currentUnlocked < 2)
        {
            PlayerPrefs.SetInt("LevelUnlocked", 2);
            PlayerPrefs.Save(); // Bắt buộc phải có dòng này để lưu vào ổ cứng
            Debug.Log("Hệ thống: ĐÃ MỞ KHÓA MÀN 2!");
        }

        // 3. Gọi kịch bản hiện bảng UI Chiến thắng của người Mẹ
        base.WinGame();
    }
    public void GoToScene2()
    {
        // 1. Mở khóa thời gian (vì lúc Win Game đã bị dừng lại)
        Time.timeScale = 1f;

        // 2. Tải Màn 2 (Lưu ý: Chữ "Scene2" phải nhập đúng y hệt tên file Scene của bạn)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
    }
}