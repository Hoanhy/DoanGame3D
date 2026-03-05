using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 100f;
    protected float currentHP;

    [Header("Movement")]
    public float moveSpeed = 2f;
    protected NavMeshAgent agent;
    protected Transform player;

    [Header("Attack (Common)")]
    public float attackRange = 6f;
    public float attackCooldown = 2f;
    protected float lastAttackTime;

    protected virtual void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        // Tìm player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }

        // Đảm bảo enemy nằm trên NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }

    protected virtual void Update()
    {
        // Không chạy nếu player hoặc agent chưa ở NavMesh
        if (player == null || !agent.isOnNavMesh) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Quay mặt về player
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack();
            }
        }
    }

    protected abstract void Attack();

    public virtual void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}