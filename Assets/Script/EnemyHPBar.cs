using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Slider slider;
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 3.75f;
            transform.forward = target.forward;
        }
    }

    public void SetHP(float current, float max)
    {
        slider.value = current / max;
    }
}