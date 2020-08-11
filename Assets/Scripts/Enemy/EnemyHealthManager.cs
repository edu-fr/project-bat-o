using UnityEngine;

namespace Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        #region Variables

        public int MaxHealth = 200;
        public int CurrentHealth;
        
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        #endregion

        #region Auxiliar Methods

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
        }

        public int GetCurrentHp()
        {
            return CurrentHealth;
        }
    
        #endregion
    }
}