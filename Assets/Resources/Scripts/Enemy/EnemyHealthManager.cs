using System;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyHealthManager : MonoBehaviour
    {
        private EnemyStateMachine EnemyStateMachine;

        public int MaxHealth = 200;
        public float CurrentHealth;

        private void Awake()
        {
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
        }

        private void Start()
        {
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

        public float GetCurrentHp()
        {
            return CurrentHealth;
        }
        
    }
}