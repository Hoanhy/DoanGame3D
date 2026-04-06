using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 100f;
    protected float currentHP;

    [Header("HP Bar")]
    public EnemyHPBar hpBar;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float sprintSpeed = 6f;
    public float sprintDistance = 8f;
    public float detectPlayerRange = 10f;
    protected NavMeshAgent agent;

    protected Transform player;
    protected Transform project;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    protected float lastAttackTime;
    protected bool isAttacking = false;

    [Header("Targeting")]
    public bool huntPlayerAlways = false;

    protected virtual void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        agent.radius = 0.5f;
        agent.stoppingDistance = 1.2f;
        agent.avoidancePriority = Random.Range(20, 80);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        GameObject proj = GameObject.FindGameObjectWithTag("Project");
        if (proj != null) project = proj.transform;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        if (hpBar != null)
        {
            hpBar.SetMaxHP(maxHP);
            hpBar.SetHP(currentHP);

            if (hpBar.target == null)
            {
                hpBar.target = transform;
            }
        }
    }

    protected virtual void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (isAttacking) return;

        Transform target = ChooseTarget();
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            if (huntPlayerAlways && distance >= sprintDistance)
                agent.speed = sprintSpeed;
            else
                agent.speed = moveSpeed;
        }
        else
        {
            agent.ResetPath();
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack(target);
            }
        }
    }

    protected Transform ChooseTarget()
    {
        if (player == null) return project;
        if (project == null) return player;

        if (huntPlayerAlways)
        {
            return player;
        }

        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance < detectPlayerRange)
            return player;

        return project;
    }

    protected abstract void Attack(Transform target);

    public virtual void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (hpBar != null)
        {
            hpBar.SetHP(currentHP);
        }

        Debug.Log(gameObject.name + " mất " + dmg + " HP. Còn: " + currentHP);

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectPlayerRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sprintDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * detectPlayerRange);
    }
}