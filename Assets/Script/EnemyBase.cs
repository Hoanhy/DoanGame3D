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
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    protected float lastAttackTime;

    protected virtual void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    //mỗi enemy sẽ đánh theo cách riêng
    protected abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Bullet"))
    //    {
    //        TakeDamage(20f);
    //        Destroy(other.gameObject);
    //    }
    //}
}
