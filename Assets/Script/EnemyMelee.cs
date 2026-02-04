using UnityEngine;
using System.Collections;

public class EnemyMelee : EnemyBase
{
    [Header("Book Attack")]
    public Transform book;
    public float pounceDistance = 0.8f;
    public float pounceDuration = 0.18f;

    Vector3 bookStartLocalPos;
    bool isAttacking;
    float defaultSpeed;

    protected override void Start()
    {
        base.Start();
        defaultSpeed = agent.speed;
        bookStartLocalPos = book.localPosition;
    }

    protected override void Attack()
    {
        if (isAttacking) return;
        StartCoroutine(BookAttack());
    }

    IEnumerator BookAttack()
    {
        isAttacking = true;
        agent.isStopped = true;

        Vector3 startPos = book.position;
        Vector3 dir = (player.position - startPos).normalized;
        Vector3 attackPos = startPos + dir * pounceDistance; 

        float t = 0f;

        // VỒ TỚI
        while (t < 1f)
        {
            t += Time.deltaTime / pounceDuration;
            book.position = Vector3.Lerp(startPos, attackPos, t);
            yield return null;
        }

        t = 0f;

        // QUAY VỀ
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
