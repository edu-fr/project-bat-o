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

        public float MaxHealth = 100;
        public float CurrentHealth;
        public bool Invincible = false;

        public PlayerController PlayerController;
        public PlayerStatsController PlayerStatsController;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
        }

        private void Start()
        {
            MaxHealth = PlayerStatsController.CurrentMaxHP;
            CurrentHealth = MaxHealth;
            
        }

        public void TakeDamage(float damage)
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
            var damageTaken = damage - PlayerStatsController.CurrentResistance;

            // Lose HP
            CurrentHealth -= damageTaken > 0 ? damage : 0; // always positive
            CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth < 0 ? 0 : CurrentHealth;

            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            
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
