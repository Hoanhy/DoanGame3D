using System.Collections;
using UnityEngine;
using UnityTutorial.PlayerController;

public class EnemyMelee : EnemyBase
{
    [Header("Book Attack")]
    public Transform book;
    public float pounceDistance = 0.8f;
    public float pounceDuration = 0.18f;
    public float damage = 10f;

    Vector3 bookStartLocalPos;
    bool isAttacking;
    float defaultSpeed;

    protected override void Start()
    {
        base.Start();
        defaultSpeed = agent.speed;
        bookStartLocalPos = book.localPosition;
    }

    protected override void Attack(Transform target)
    {
        if (isAttacking) return;
        StartCoroutine(BookAttack(target));
    }

    IEnumerator BookAttack(Transform target)
    {
        isAttacking = true;
        agent.isStopped = true;

        Vector3 startPos = book.position;
        Vector3 dir = (target.position - startPos).normalized;
        Vector3 attackPos = startPos + dir * pounceDistance;

        float t = 0f;

        // Vồ tới
        while (t < 1f)
        {
            t += Time.deltaTime / pounceDuration;
            book.position = Vector3.Lerp(startPos, attackPos, t);
            yield return null;
        }

        // gây damage
        if (target.CompareTag("Player"))
        {
            PlayerController player = target.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        if (target.CompareTag("Project"))
        {
            ProjectHP hp = target.GetComponent<ProjectHP>();
            if (hp != null)
                hp.TakeDamage(damage);
        }

        t = 0f;

        // quay về
        while (t < 1f)
        {
            t += Time.deltaTime / pounceDuration;
            book.position = Vector3.Lerp(attackPos, startPos, t);
            yield return null;
        }

        agent.isStopped = false;
        isAttacking = false;
    }
}