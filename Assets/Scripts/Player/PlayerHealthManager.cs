using Enemy;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHealthManager : MonoBehaviour
    {
        #region Variables

        private int MaxHealth = 100;
        private int CurrentHealth;

        public HealthBarScript HealthBarScript;
    
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            CurrentHealth = MaxHealth;
            HealthBarScript.SetMaxHealth(MaxHealth);
        }

        #endregion

        #region Auxiliar Methods

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            HealthBarScript.SetHealth(CurrentHealth);
        }

        public int GetCurrentHp()
        {
            return CurrentHealth;
        }
    
        #endregion
    }
}
