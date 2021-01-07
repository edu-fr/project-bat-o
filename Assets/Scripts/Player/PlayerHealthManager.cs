using System.Collections;
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
        public Material FlashMaterial;

        public PersistentObject PersistentObject;
        
      
        public int MaxHealth = 100;
        public int CurrentHealth;
        public bool Invincible = false;

        public HealthBarScript HealthBarScript;
        public PlayerController PlayerController;
    
        #endregion

      
        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
            PlayerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Only call at Start cause Game Manager create the persistent instance on Awake
            PersistentObject = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PersistentObject>();

            if (PersistentObject.PlayerPreviousHp == 0)
            {
                CurrentHealth = MaxHealth;
                HealthBarScript.SetMaxHealth(MaxHealth);
                HealthBarScript.SetHealth(MaxHealth);
            }
        }

        private void Update()
        {
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            
            HealthBarScript.SetHealth(CurrentHealth);
        }
        
        #region Auxiliar Methods

        
        public void TakeDamage(int damage)
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
            
            // Work on it
            Invoke(nameof(EndFlash), 0.1f);
            Invoke(nameof(FlashSprite), 0.2f);
            Invoke(nameof(EndFlash), 0.3f);
            Invoke(nameof(FlashSprite), 0.4f);
            Invoke(nameof(EndFlash), 0.5f);
            Invoke(nameof(EndInvincibility), 0.6f);
        }

        public void IncreaseMaxHP(int value)
        {
            MaxHealth += value;
            HealthBarScript.SetMaxHealth(MaxHealth);
        }

        public void Heal(int healValue)
        {
            CurrentHealth = CurrentHealth + healValue;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
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
        #endregion
    }
}
