using UnityEngine;

public class ProjectHP : MonoBehaviour
{
    [Header("Project Health")]
    public float maxHP = 200f;
    private float currentHP;

    [Header("Project HP UI")]
    public ProjectHPBar hpBar;

    void Start()
    {
        currentHP = maxHP;

        if (hpBar != null)
        {
            hpBar.SetMaxHP(maxHP);
            hpBar.SetHP(currentHP);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (hpBar != null)
        {
            hpBar.SetHP(currentHP);
        }

        Debug.Log("Project mất " + damage + " HP. Còn: " + currentHP);

        if (currentHP <= 0)
        {
            if (Level3Manager.Instance != null)
            {
                Level3Manager.Instance.ProjectDestroyed();
            }

            Destroy(gameObject);
        }
    }
}