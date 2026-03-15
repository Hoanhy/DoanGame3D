using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 100f;
    protected float currentHP;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float sprintSpeed = 6f;       // Tốc độ khi đuổi
    public float sprintDistance = 8f;    // Khoảng cách kích hoạt đuổi
    protected NavMeshAgent agent;

    protected Transform player;
    protected Transform project; // đồ án

    [Header("Attack (Common)")]
    public float attackRange = 6f;
    public float attackCooldown = 2f;
    protected float lastAttackTime;
    protected bool isAttacking = false;

    [Header("Targeting Setting")]
    public bool huntPlayerAlways = false; // Tick vào nếu muốn quái chỉ săn Player

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

        // Tìm đồ án
        GameObject proj = GameObject.FindGameObjectWithTag("Project");
        if (proj != null)
        {
            project = proj.transform;
        }
        else
        {
            Debug.LogWarning("Project not found!");
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
        if (!agent.isOnNavMesh) return;
        if (isAttacking) return;

        Transform target = ChooseTarget();

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Quay mặt về target
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

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
            agent.isStopped = true;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack(target);
            }
        }
    }

    // Chọn mục tiêu
    protected Transform ChooseTarget()
    {
        if (player == null) return project;
        if (project == null) return player;

        // 1. NẾU BẬT CÔNG TẮC SĂN PLAYER: Bỏ qua Đồ án, lao thẳng vào người chơi từ bất kỳ đâu
        if (huntPlayerAlways)
        {
            return player;
        }

        // 2. NẾU TẮT CÔNG TẮC: Ưu tiên đánh Đồ án
        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance < 5f) // Nếu player chạy lại gần 5m thì tấn công
            return player;

        return project;
    }

    protected abstract void Attack(Transform target);

    public virtual void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        // --- THÊM DÒNG DEBUG NÀY ĐỂ BÁO CÁO MÁU ---
        Debug.Log("Quái vật " + gameObject.name + " bị chém mất " + dmg + " máu! Máu còn lại: " + currentHP);

        if (currentHP <= 0)
        {
            // Thêm 1 dòng báo tử vong luôn cho rõ ràng
            Debug.Log("Quái vật " + gameObject.name + " ĐÃ BỊ TIÊU DIỆT!");
            Destroy(gameObject);
        }
    }

}