using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        private EnemyStateMachine EnemyStateMachine;

        private float MaxHealth { get; set; }
        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
        }

        private void Start()
        {
            MaxHealth = 10;
            CurrentHealth = MaxHealth;
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