using UnityEngine;
using UnityTutorial.PlayerController;

public class BulletEnemy : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // ===== PLAYER =====
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("Bullet hit Player");
            Destroy(gameObject);
            return;
        }

        // ===== PROJECT =====
        ProjectHP project = collision.gameObject.GetComponentInParent<ProjectHP>();

        if (project != null)
        {
            project.TakeDamage(damage);
            Debug.Log("Bullet hit Project");
            Destroy(gameObject);
            return;
        }

        // ===== hit vật khác =====
        Destroy(gameObject);
    }
}