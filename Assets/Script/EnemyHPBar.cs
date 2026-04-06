using UnityEngine;

public class EnemyHPBar : HPBar
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 3.75f;
            transform.forward = Camera.main.transform.forward;
        }
    }
}