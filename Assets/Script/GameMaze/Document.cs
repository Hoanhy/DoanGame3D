using UnityEngine;

public class Document : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem người chạm vào có đúng là Player không
        if (other.CompareTag("Player"))
        {
            // BÁO CÁO CHO LEVEL 1 MANAGER (Thay vì GameManager cũ)
            if (Level1Manager.Instance != null)
            {
                Level1Manager.Instance.CollectDocument();

                // Hủy (biến mất) tập hồ sơ sau khi nhặt
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Level1Manager trong Scene!");
            }
        }
    }
}