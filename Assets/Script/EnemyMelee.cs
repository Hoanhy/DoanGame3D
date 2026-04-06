using System.Collections;
using UnityEngine;
using UnityTutorial.PlayerController;

public class EnemyMelee : EnemyBase
{
    [Header("Book Attack")]
    public Transform book;
    public float pounceDistance = 1.2f;
    public float pounceDuration = 0.2f;
    public float damage = 10f;

    Vector3 bookStartLocalPos;

    protected override void Start()
    {
        base.Start();

        if (book != null)
            bookStartLocalPos = book.localPosition;
    }

    protected override void Attack(Transform target)
    {
        if (isAttacking || target == null) return;

        // ===== DAMAGE NGAY GIỐNG RANGED =====

        if (target.CompareTag("Player"))
        {
            PlayerController pc = target.GetComponent<PlayerController>();

            if (pc != null)
            {
                pc.TakeDamage(damage);
                Debug.Log("Hit Player");
            }
        }

        if (target.CompareTag("Project"))
        {
            ProjectHP hp = target.GetComponent<ProjectHP>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log("Hit Project");
            }
        }

        // chỉ animation riêng
        StartCoroutine(BookAttackAnimation(target));
    }

    IEnumerator BookAttackAnimation(Transform target)
    {
        isAttacking = true;
        agent.isStopped = true;

        Vector3 startPos = book.localPosition;
        Vector3 attackPos = startPos + Vector3.forward * pounceDistance;

        float t = 0f;

        // lao tới
        while (t < 1f)
        {
            t += Time.deltaTime / pounceDuration;
            book.localPosition = Vector3.Lerp(startPos, attackPos, t);
            yield return null;
        }

        // quay về
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / pounceDuration;
            book.localPosition = Vector3.Lerp(attackPos, startPos, t);
            yield return null;
        }

        book.localPosition = bookStartLocalPos;

        agent.isStopped = false;
        isAttacking = false;
    }
}