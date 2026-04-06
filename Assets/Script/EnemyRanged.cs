using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector3 shootDir = (target.position - firePoint.position).normalized;

        // tạo đạn
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDir)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = shootDir * bulletSpeed;
        }
    }
}