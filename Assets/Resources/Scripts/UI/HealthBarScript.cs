using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarScript : MonoBehaviour
    {
        public Slider Slider;
        public TextMeshProUGUI Amount;
        private PlayerHealthManager PlayerHealthManager;

        private void Start()
        {
            PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
        }

        public void SetHealth(float health)
        {
            Slider.value = health;
            Amount.SetText(PlayerHealthManager.CurrentHealth + "/" + PlayerHealthManager.MaxHealth);
        }

        public void SetMaxHealth(float maxHealth)
        {
            Slider.maxValue = maxHealth;
            Amount.SetText(PlayerHealthManager.CurrentHealth + "/" + PlayerHealthManager.MaxHealth);
        }
    }
}
