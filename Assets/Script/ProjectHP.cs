using UnityEngine;

public class ProjectHP : MonoBehaviour
{
    public float maxHP = 200f;
    float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        // Thêm đoạn này vào:
        if (currentHP <= 0)
        {
            currentHP = 0;
            // Báo động cho Level 3 biết là Đồ án đã banh xác!
            if (Level3Manager.Instance != null)
            {
                Level3Manager.Instance.ProjectDestroyed();
            }

            // Xóa cục đồ án (hoặc chạy animation nổ)
            Destroy(gameObject);
        }
    }
}