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
            UpdateLifeBar();
        }

        private void Update()
        {
            if (!PlayerHealthManager)
            {
                PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
                UpdateLifeBar();
            }
        }

        public void UpdateLifeBar()
        {
            if (PlayerHealthManager)
            {
                Slider.value = PlayerHealthManager.CurrentHealth;
                Slider.maxValue = PlayerHealthManager.MaxHealth;
                Amount.SetText(PlayerHealthManager.CurrentHealth + "/" + PlayerHealthManager.MaxHealth);
            }
        }
    }
}
