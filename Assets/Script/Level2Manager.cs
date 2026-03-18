using UnityEngine;

public class Level2Manager : BaseGameManager
{
    public static Level2Manager Instance;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        // Gọi lệnh của BaseGameManager để tự động ẩn các bảng Pause, Setting lúc mới vào game
        base.Start();
    }

    protected override void Update()
    {
        // Gọi lệnh của BaseGameManager để tự động rình nút ESC và giấu/hiện chuột
        base.Update();

        // Nếu game đang Pause thì không làm gì thêm
        if (isPaused) return;
    }

    // Nếu nhân vật chết ở Màn 2, bạn có thể gọi Level2Manager.Instance.GameOver() 
    // Nó sẽ tự động dùng hàm GameOver của BaseGameManager!
}