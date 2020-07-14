using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Gradient gradient = null;
    [SerializeField] private Image fill = null;

    public void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void setHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void setMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
