using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        private EnemyStateMachine EnemyStateMachine;
        private EnemyStatsManager EnemyStatsManager;

        private float MaxHealth { get; set; }
        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyStatsManager = GetComponent<EnemyStatsManager>();
        }

        private void Start()
        {
            CurrentHealth = EnemyStatsManager.MaxHP;
        }

        private void Update()
        {
            // Verify if its alive
            if(!EnemyStateMachine.IsDying && GetCurrentHp() <= 0)
            {
                EnemyStateMachine.ChangeState(EnemyStateMachine.States.Dying);
            }
        }
        
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        private float GetCurrentHp()
        {
            return CurrentHealth;
        }
        
    }
}