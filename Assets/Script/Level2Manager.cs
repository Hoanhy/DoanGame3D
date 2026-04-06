using UnityEngine;
using UnityTutorial.Quiz;
using Unity.Cinemachine;

public class Level2Manager : BaseGameManager
{
    public static Level2Manager Instance;

    [Header("=== CHECKPOINT PHÒNG 2 ===")]
    public Transform room2SpawnPoint; // Kéo điểm hồi sinh ở Phòng 2 vào đây
    public QuizManager quizManager;   // Kéo QuizManager vào để tự pass bài thi

    // Trí nhớ vĩnh cửu: Lưu việc đã sang phòng 2
    public static bool hasReachedRoom2 = false;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        // Tắt UI Pause/GameOver lúc mới vào game
        base.Start();

        // Vừa vào game, kiểm tra ngay xem có phải đang Load lại từ Checkpoint không
        if (hasReachedRoom2)
        {
            RestoreCheckpoint();
        }
    }

    protected override void Update()
    {
        // Rình nút ESC
        base.Update();
    }

    // Hàm này để cục Trigger tàng hình gọi khi đạp trúng
    public void SaveRoom2Checkpoint()
    {
        if (!hasReachedRoom2)
        {
            hasReachedRoom2 = true;
            Debug.Log("Hệ thống: LƯU CHECKPOINT PHÒNG 2 THÀNH CÔNG!");
        }
    }

    // Hàm nội bộ dùng để bế người chơi sang Phòng 2
    private void RestoreCheckpoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && room2SpawnPoint != null)
        {
            Vector3 oldPos = player.transform.position;

            // Tắt vật lý
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // Ép vị trí
            player.transform.position = room2SpawnPoint.position;
            player.transform.rotation = room2SpawnPoint.rotation;
            Physics.SyncTransforms();

            // Bật vật lý
            if (rb != null) rb.isKinematic = false;
            if (cc != null) cc.enabled = true;

            // Kéo Camera
            if (Camera.main != null)
            {
                CinemachineCore.OnTargetObjectWarped(player.transform, room2SpawnPoint.position - oldPos);
            }
        }

        // Tự động báo Đậu bài thi
        if (quizManager != null)
        {
            quizManager.hasPassed = true;
        }
    }

    // Hàm này để cục Trigger tàng hình gọi khi nhân vật chết và muốn quay lại Checkpoint
    public void ResetCheckpoint()
    {
        hasReachedRoom2 = false;
    }
    public void RestartFromBeginning()
    {
        // 1. Xóa luôn việc đã sang phòng 2 để quay về từ đầu
        hasReachedRoom2 = false;

        // 2. Gọi lệnh Load lại màn chơi từ kịch bản người Mẹ (BaseGameManager)
        RestartGame();
    }
    public override void WinGame()
    {
        // Thắng màn 2 thì mở màn 3
        int currentUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        if (currentUnlocked < 3)
        {
            PlayerPrefs.SetInt("LevelUnlocked", 3);
            PlayerPrefs.Save();
            Debug.Log("Hệ thống: ĐÃ MỞ KHÓA MÀN 3!");
        }

        // Hiện bảng UI
        base.WinGame();
    }
    public void GoToScene3()
    {
        // 1. Mở khóa thời gian (vì lúc Win Game đã bị dừng lại)
        Time.timeScale = 1f;

        // 2. Tải Màn 2 (Lưu ý: Chữ "Scene2" phải nhập đúng y hệt tên file Scene của bạn)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene3");
    }
}