using UnityEngine;
using TMPro; // Thư viện để điều khiển TextMeshPro

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 3f; // Tốc độ nhấp nháy (số càng lớn nháy càng nhanh)
    private TextMeshProUGUI textMesh;

    void Start()
    {
        // Lấy cái Component chữ gắn trên chính object này
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textMesh != null)
        {
            // Lấy màu hiện tại của chữ
            Color color = textMesh.color;

            // Dùng hàm Toán học Sin để làm cho độ mờ (Alpha) chạy lên chạy xuống mượt mà từ 0 đến 1
            color.a = Mathf.Abs(Mathf.Sin(Time.unscaledTime * blinkSpeed));

            // Ép màu mới vào lại chữ
            textMesh.color = color;
        }
    }
}