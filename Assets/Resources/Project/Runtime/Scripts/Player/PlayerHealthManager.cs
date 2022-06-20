using System.Collections;
using Game;
using Resources.Project.Runtime.Scripts.Player;
using Resources.Scripts.Enemy.Attacks;
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
            PlayerController = GetComponent<PlayerController>();
            PlayerStatsController = GetComponent<PlayerStatsController>();
        }

        private void Start()
        {
            MaxHealth = PlayerStatsController.MaxHp;
            CurrentHealth = MaxHealth;
            
        }

        private void Update()
        {
            // Avoid less then 0 and more than max hp
            CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth < 0 ? 0 : CurrentHealth;
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
