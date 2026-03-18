using UnityEngine;
using TMPro;

// Kế thừa từ BaseGameManager để có sẵn Pause, UI Setting, Restart...
public class Level3Manager : BaseGameManager
{
    public static Level3Manager Instance;

    [Header("=== LOGIC RIÊNG CỦA MÀN 3 ===")]
    public GameObject missionText;

    [Header("Bảo vệ Đồ Án")]
    public float surviveTime = 120f; // Sống sót và bảo vệ trong 120 giây (2 phút)
    private bool isDefending = true;

    [Header("NPC Trả Nhiệm Vụ")]
    public TeacherNPC npcMan3; // Kéo ông NPC Màn 3 vào đây để báo cáo khi thủ nhà xong

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start(); // Bắt buộc gọi để tắt các bảng UI Pause/GameOver của Mẹ
    }

    protected override void Update()
    {
        base.Update(); // Chạy lệnh rình nút ESC của Mẹ

        // Nếu game đang Pause hoặc đã thủ nhà xong/thua thì không đếm giờ nữa
        if (isPaused || !isDefending) return;

        // Đếm ngược thời gian
        surviveTime -= Time.deltaTime;

        // Hiển thị thời gian lên UI
        if (missionText != null)
        {
            int minutes = Mathf.FloorToInt(surviveTime / 60);
            int seconds = Mathf.FloorToInt(surviveTime % 60);
        }

        // KHI HẾT GIỜ MÀ ĐỒ ÁN CHƯA VỠ -> THẮNG KÉO BẦY QUÁI
        if (surviveTime <= 0)
        {
            surviveTime = 0;
            isDefending = false;
            ProjectSaved();
        }
    }

    // ===== CÁC HÀM XỬ LÝ THẮNG/THUA =====

    // Hàm này sẽ được gọi từ script ProjectHP khi cục đồ án bị quái đánh nát (Máu <= 0)
    public void ProjectDestroyed()
    {
        if (!isDefending) return;

        isDefending = false;

        GameOver(); // Gọi hàm GameOver hiện UI thua của BaseGameManager
    }

    // Hàm này sẽ được gọi từ script PlayerController khi nhân vật chết
    public void PlayerDied()
    {
        if (!isDefending) return;

        isDefending = false;

        GameOver();
    }

    // Hàm xử lý khi hết giờ mà đồ án vẫn an toàn
    private void ProjectSaved()
    {

        // Báo cho ông NPC biết là đã xong nhiệm vụ để ổng lật sang Kịch bản 2 (khen ngợi)
        if (npcMan3 != null)
        {
            npcMan3.SetCombatCleared();
        }
        else
        {
            // Nếu bạn quên kéo NPC vào Inspector thì tự động Win luôn cho an toàn
            WinGame();
        }
    }
}