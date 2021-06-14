using UnityEngine;

namespace Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        public int MaxHealth = 200;
        public int CurrentHealth;
        
        private void Start()
        {
            CurrentHealth = MaxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
        }

        public int GetCurrentHp()
        {
            return CurrentHealth;
        }
    }
}