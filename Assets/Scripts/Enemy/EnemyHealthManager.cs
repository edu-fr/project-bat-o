using UnityEngine;

namespace Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;
        [SerializeField] private EnemyStatsManager enemyStatsManager;

        private float maxHealth { get; set; }
        public float currentHealth { get; private set; }


        private void Start()
        {
            currentHealth = enemyStatsManager.MaxHP;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            // Verify if its alive
            if(!enemyStateMachine.IsDying && GetCurrentHp() <= 0)
            {
                enemyStateMachine.ChangeState(EnemyStateMachine.States.Dying);
            }
        }

        private float GetCurrentHp()
        {
            return currentHealth;
        }
        
    }
}