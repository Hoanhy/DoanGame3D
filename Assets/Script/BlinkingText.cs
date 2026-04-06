using UnityEngine;
using TMPro; 

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 3f; // Tốc độ nhấp nháy
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textMesh != null)
        {
            Color color = textMesh.color;

            // Dùng hàm Toán học Sin để làm cho độ mờ (Alpha)
            color.a = Mathf.Abs(Mathf.Sin(Time.unscaledTime * blinkSpeed));

            textMesh.color = color;
        }
    }
}