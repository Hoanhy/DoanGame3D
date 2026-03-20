using UnityEngine;
using TMPro; // Dùng cho TextMeshPro
using System.Collections; // Dùng cho bộ đếm thời gian

public class SceneTutorial : MonoBehaviour
{
    [Header("Giao diện Hướng dẫn")]
    public GameObject tutorialPanel;     // Kéo cái Bảng nền đen mờ vào đây
    public TextMeshProUGUI tutorialText; // Kéo cái Chữ vào đây

    [Header("Cài đặt nội dung")]
    [TextArea(2, 4)]
    public string message = "Nhập hướng dẫn vào đây...";
    
    [Tooltip("Số giây hiển thị trước khi tự tắt")]
    public float displayTime = 5f; 

    void Start()
    {
        // Khi vừa load Scene (vừa Play game), tự động bật UI và đổi chữ
        if (tutorialPanel != null && tutorialText != null)
        {
            tutorialPanel.SetActive(true);
            tutorialText.text = message;
            
            // Gọi đồng hồ đếm ngược để tự tắt
            StartCoroutine(HideTutorialRoutine());
        }
    }

    IEnumerator HideTutorialRoutine()
    {
        yield return new WaitForSeconds(displayTime); // Chờ đúng số giây đã cài
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false); // Giấu bảng đi
        }
    }
}