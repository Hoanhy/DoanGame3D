using UnityEngine;
using Unity.Cinemachine;
using UnityTutorial.PlayerController; // Gọi script người chơi

public class Teleporter : MonoBehaviour
{
    [Header("Cài đặt Dịch chuyển")]
    public Transform destinationPoint;
    public SceneFader fader;

    public void TeleportPlayer()
    {
        // 1. KHÓA NGƯỜI CHƠI NGAY LẬP TỨC TRƯỚC KHI FADE MÀN HÌNH
        // Chống việc người chơi bấm phím chạy lung tung lúc màn hình đang tối dần
        LockPlayer();

        if (fader != null)
        {
            StartCoroutine(fader.FadeOutAndTeleport(() => ExecuteTeleport()));
        }
        else
        {
            ExecuteTeleport();
        }
    }

    private void LockPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false; // Tắt di chuyển

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero; // Xóa đà chạy
        }
    }

    private void ExecuteTeleport()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null || destinationPoint == null)
        {
            Debug.LogError("LỖI: Thiếu Player hoặc Destination Point!");
            return;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        // 2. TẮT NỘI SUY (INTERPOLATION) ĐỂ CHỐNG BỊ VẬT LÝ GIẬT LÙI
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        // 3. DỜI VỊ TRÍ
        Vector3 oldPosition = player.transform.position;
        player.transform.position = destinationPoint.position;
        player.transform.rotation = destinationPoint.rotation;

        // 4. ÉP UNITY XÓA SẠCH TRÍ NHỚ VÀ CẬP NHẬT VỊ TRÍ MỚI NGAY LẬP TỨC
        Physics.SyncTransforms();

        // 5. TRẢ LẠI TRẠNG THÁI BÌNH THƯỜNG
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Bật lại làm mượt
            rb.linearVelocity = Vector3.zero;
        }

        // (Script PlayerController sẽ tự động được SceneFader bật lại khi màn hình sáng lên)

        // 6. CẬP NHẬT CAMERA TRÁNH BỊ VĂNG GÓC NHÌN
        if (Camera.main != null)
        {
            CinemachineCore.OnTargetObjectWarped(player.transform, destinationPoint.position - oldPosition);
        }

        Debug.Log("Hệ thống: Dịch chuyển hoàn tất 100% không trượt phát nào!");
    }
}