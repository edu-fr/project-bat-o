using System.Collections;
using Enemy.Attacks;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerHealthManager : MonoBehaviour
    {
        private Renderer Renderer;
        private Material DefaultMaterial;
        [SerializeField] public Material FlashMaterial;
        
        public delegate void HealthChangeHandler(float amount, float max);
        public static event HealthChangeHandler HealthChanged;

        public float CurrentHealth;
        public bool Invincible = false;

        private PlayerController _playerController;
        private PlayerStatsController _playerStats;
        private PowerUpEffects _powerUpEffects;
        private PlayerStateMachine _playerStateMachine;

        private void Awake()
        {
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _powerUpEffects = GetComponent<PowerUpEffects>();
            _playerStats = GetComponent<PlayerStatsController>();
            _playerController = GetComponent<PlayerController>();
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
        }

        public void TakeDamage(float damage)
        {
            if (Invincible) return;
            Invincible = true;

            // Make take hit noise
            AudioManager.instance.Play("Player get hit");

            // Calculate defenses, etc
            var damageTaken = damage - _playerStats.CurrentResistance;

            // Lose HP
            CurrentHealth -= damageTaken > 0 ? damageTaken : 0; // always positive
            
            // Wind blessing
            _playerStats.LoseWindStacks();

            // Update UI
            HealthChanged?.Invoke(CurrentHealth, _playerStats.CurrentMaxHP);
            
            // Flash sprite
            StartCoroutine(FlashSprite());
        }

        public void Heal(float amount)
        {
            if (amount <= 0) return; // Don't heal for negative amounts
            CurrentHealth += amount;
            if (CurrentHealth > _playerStats.CurrentMaxHP) 
                CurrentHealth = _playerStats.CurrentMaxHP;
            HealthChanged?.Invoke(CurrentHealth, _playerStats.CurrentMaxHP);
        }

        public void UpdateMaxHP(float newValue)
        {
            if (CurrentHealth > _playerStats.CurrentMaxHP) CurrentHealth = _playerStats.CurrentMaxHP;
            HealthChanged?.Invoke(CurrentHealth, _playerStats.CurrentMaxHP);
        }

        public void LoseHPFlat(float amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0) CurrentHealth = 0;
            HealthChanged?.Invoke(CurrentHealth, _playerStats.CurrentMaxHP);
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
