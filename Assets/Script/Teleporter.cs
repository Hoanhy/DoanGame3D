using UnityEngine;
using Unity.Cinemachine; // Dùng cho Unity 6 để điều khiển Camera

public class Teleporter : MonoBehaviour
{
    public Transform destinationPoint; // Điểm đến ở Phòng 2
    public SceneFader fader;           // Kéo cái Canvas (có script SceneFader) vào đây

    public void TeleportPlayer()
    {
        if (fader != null)
        {
            // Gọi hiệu ứng chớp mắt: Tối dần -> Dịch chuyển -> Sáng dần
            StartCoroutine(fader.FadeOutAndTeleport(() => ExecuteTeleport()));
        }
        else
        {
            // Nếu quên gắn fader thì dịch chuyển tức thời như cũ
            ExecuteTeleport();
        }
    }

    private void ExecuteTeleport()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && destinationPoint != null)
        {
            Vector3 oldPosition = player.transform.position;

            // 1. Dịch chuyển vị trí
            player.transform.position = destinationPoint.position;
            player.transform.rotation = destinationPoint.rotation;

            // 2. Xóa đà di chuyển
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            // 3. THÔNG BÁO CHO CINEMACHINE (Dành cho Unity 6)
            // Lệnh này cực kỳ quan trọng để camera không bị văng khi Player biến mất đột ngột
            CinemachineCore.OnTargetObjectWarped(player.transform, destinationPoint.position - oldPosition);

            Debug.Log("Hệ thống: Đã dịch chuyển người chơi sang Phòng 2!");
        }
    }
}