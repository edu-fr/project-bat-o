using System.Collections;
using Enemy;
using Game;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHealthManager : MonoBehaviour
    {
        private Renderer Renderer;
        private Material DefaultMaterial;
        [SerializeField]
        public Material FlashMaterial;
        
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

        public void TakeDamage(float damage)
        {
            if (Invincible) return;
            if (PlayerController.PlayerStateMachine.State == PlayerStateMachine.States.Dashing)
            {
                PlayerController.DodgeFailed = true;
            }
            Invincible = true;
            FlashSprite();
            
            // Make take hit noise
            AudioManager.instance.Play("Player get hit");
            
            // Lose HP
            CurrentHealth -= damage;
            // Update life bar
            HealthBarScript.UpdateLifeBar();
            
            // Work on it
            Invoke(nameof(EndFlash), 0.1f);
            Invoke(nameof(FlashSprite), 0.2f);
            Invoke(nameof(EndFlash), 0.3f);
            Invoke(nameof(FlashSprite), 0.4f);
            Invoke(nameof(EndFlash), 0.5f);
            Invoke(nameof(EndInvincibility), 0.6f);
        }

        private void FlashSprite()
        {
            Renderer.material = FlashMaterial;
        }

        private void EndFlash()
        {
            Renderer.material = DefaultMaterial;
        }
        public void EndInvincibility ()
        {
            Invincible = false;
        }

        public IEnumerator EndInvincibilityAfterTime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            Invincible = false;
        }
    }
}
