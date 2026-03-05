using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    protected override void Attack()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Tạo đạn
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(player.position - firePoint.position)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.forward * bulletSpeed;
        }
    }
}