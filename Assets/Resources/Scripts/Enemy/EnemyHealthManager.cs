using UnityEngine;

namespace Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        public int MaxHealth = 200;
        public float CurrentHealth;
        
        private void Start()
        {
            CurrentHealth = MaxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        public float GetCurrentHp()
        {
            return CurrentHealth;
        }
    }
}