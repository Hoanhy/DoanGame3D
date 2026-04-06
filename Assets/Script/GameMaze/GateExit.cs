using UnityEngine;

public class GateExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có đúng là Player chạm vào không
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem Màn 1 có đang chạy không
            if (Level1Manager.Instance != null)
            {
                // Nếu đã được NPC phát thẻ sinh viên
                if (Level1Manager.Instance.hasStudentCard)
                {
                    Debug.Log("Hệ thống: Qua cổng thành công!");
                    Level1Manager.Instance.WinGame(); // Gọi giao diện chiến thắng
                }
                else
                {
                    // Nếu chạy ra cổng mà chưa có thẻ
                    Debug.Log("Hệ thống: Bạn cần nộp hồ sơ cho giáo viên để nhận thẻ trước!");

                    // Bạn có thể đổi dòng missionText để nhắc nhở người chơi
                    if (Level1Manager.Instance.missionText)
                    {
                        Level1Manager.Instance.missionText.text = "Cần thẻ sinh viên! Tìm hồ sơ nộp cho giáo viên.";
                    }
                }
            }
        }
    }
}