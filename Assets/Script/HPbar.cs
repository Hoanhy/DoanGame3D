using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;

    public virtual void SetMaxHP(float maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP;
    }

    public virtual void SetHP(float currentHP)
    {
        slider.value = currentHP;
    }
}