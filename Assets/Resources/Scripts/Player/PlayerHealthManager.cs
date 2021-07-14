using System;
using System.Collections;
using Game;
using Resources.Scripts.Enemy.Attacks;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHealthManager : MonoBehaviour
    {
        private Renderer Renderer;
        private Material DefaultMaterial;
        [SerializeField] public Material FlashMaterial;

        public float MaxHealth = 100;
        public float CurrentHealth;
        public bool Invincible = false;

        private HealthBarScript HealthBarScript;
        public PlayerController PlayerController;
        public PlayerStatsController PlayerStatsController;

        private void Awake()
        {
            HealthBarScript = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBarScript>();
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
            PlayerController = GetComponent<PlayerController>();
            PlayerStatsController = GetComponent<PlayerStatsController>();
        }

        private void Start()
        {
            MaxHealth = PlayerStatsController.MaxHp;
            CurrentHealth = MaxHealth;
            HealthBarScript.UpdateLifeBar();
        }

        private void Update()
        {
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
                HealthBarScript.UpdateLifeBar();
            }

            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
                HealthBarScript.UpdateLifeBar();
            }

        }

        public void TakeDamage(float damage, BaseAttack.DamageType damageType)
        {
            if (Invincible) return;
            if (PlayerController.PlayerStateMachine.State == PlayerStateMachine.States.Dashing)
            {
                PlayerController.DodgeFailed = true;
            }

            Invincible = true;

            // Make take hit noise
            AudioManager.instance.Play("Player get hit");

            // Calculate defenses, etc
            damage = damageType switch
            {
                BaseAttack.DamageType.Physical => damage - PlayerStatsController.PhysicalDefense,
                BaseAttack.DamageType.Magical => damage - PlayerStatsController.MagicalDefense,
                _ => damage
            };

            // Lose HP
            CurrentHealth -= damage;
            
            // Update life bar
            HealthBarScript.UpdateLifeBar();
            StartCoroutine(FlashSprite());
        }

        private IEnumerator FlashSprite()
        {
            Renderer.material = FlashMaterial;
            yield return new WaitForSeconds(0.1f);
            Renderer.material = DefaultMaterial;
            yield return new WaitForSeconds(0.1f);
            Renderer.material = FlashMaterial;
            yield return new WaitForSeconds(0.1f);
            Renderer.material = DefaultMaterial;
            Invincible = false;
        }

        public IEnumerator EndInvincibilityAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Invincible = false;
        }
    }
}
