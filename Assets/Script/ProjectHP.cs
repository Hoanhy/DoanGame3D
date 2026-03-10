using UnityEngine;

public class ProjectHP : MonoBehaviour
{
    public float maxHP = 200f;
    float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        Debug.Log("Project HP: " + currentHP);

        if (currentHP <= 0)
        {
            Debug.Log("ĐỒ ÁN BỊ PHÁ!");
            Destroy(gameObject);
        }
    }
}