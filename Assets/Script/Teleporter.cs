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

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        Vector3 oldPosition = player.transform.position;
        player.transform.position = destinationPoint.position;
        player.transform.rotation = destinationPoint.rotation;

        Physics.SyncTransforms();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Bật lại làm mượt
            rb.linearVelocity = Vector3.zero;
        }

        if (Camera.main != null)
        {
            CinemachineCore.OnTargetObjectWarped(player.transform, destinationPoint.position - oldPosition);
        }

        Debug.Log("Hệ thống: Dịch chuyển hoàn tất 100% không trượt phát nào!");
    }
}