using Enemy;
using Game;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHealthManager : MonoBehaviour
    {
        #region Variables

        private Renderer Renderer;
        private Material DefaultMaterial;
        [SerializeField]
        private Material FlashMaterial;
        
        private int MaxHealth = 100;
        private int CurrentHealth;
        private bool Invincible = false;

        public HealthBarScript HealthBarScript;
    
        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
        }
        
        private void Start()
        {
            CurrentHealth = MaxHealth;
            HealthBarScript.SetMaxHealth(MaxHealth);
        }

        #endregion

        #region Auxiliar Methods

        public void TakeDamage(int damage)
        {
            if (Invincible) return;

            Invincible = true;
            FlashSprite();
            
            // Make take hit noise
            AudioManager.instance.Play("Player get hit");
            
            // Lose HP
            CurrentHealth -= damage;
            HealthBarScript.SetHealth(CurrentHealth);
            
            // Work on it
            Invoke(nameof(EndFlash), 0.1f);
            Invoke(nameof(FlashSprite), 0.2f);
            Invoke(nameof(EndFlash), 0.3f);
            Invoke(nameof(FlashSprite), 0.4f);
            Invoke(nameof(EndFlash), 0.5f);
            Invoke(nameof(Endinvincibility), 0.6f);
        }

        public int GetCurrentHp()
        {
            return CurrentHealth;
        }

        private void FlashSprite()
        {
            Renderer.material = FlashMaterial;
        }

        private void EndFlash()
        {
            Renderer.material = DefaultMaterial;
        }
        private void Endinvincibility ()
        {
            Invincible = false;
        }
        
        #endregion
    }
}
