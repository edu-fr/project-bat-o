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
        public GameObject Player;
        private PlayerHealthManager PlayerHealthManager;

        private void Awake()
        {
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
        }

        private void Start()
        {
            UpdateLifeBar();
        }

        public void UpdateLifeBar()
        {
            Slider.value = PlayerHealthManager.CurrentHealth;
            Slider.maxValue = PlayerHealthManager.MaxHealth;
            Amount.SetText(PlayerHealthManager.CurrentHealth + "/" + PlayerHealthManager.MaxHealth);
        }
    }
}
