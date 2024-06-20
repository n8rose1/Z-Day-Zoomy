using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = PlayerController.MAX_HEALTH;
        slider.value = PlayerController.health;
    }

    public void SetHealth(int health) {
        slider.value = health;
    }

    public void SetMax(int maxHealth) {
        slider = GetComponent<Slider>();
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
}
