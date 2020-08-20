using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttackManager : MonoBehaviour
    {
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        public float AttackVelocity = 50f;
        private void Awake()
        {
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
        }

        private void Update()
        {
        
        } 
        
        public void Attack(Vector3 playerDirection)
        {
            Debug.Log("Atacou!");
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * AttackVelocity, ForceMode2D.Impulse);
        }

        public void AttackEnd()
        {
            EnemyStateMachine.ChangeState(EnemyStateMachine.PreviousState);
            EnemyStateMachine.IsAttacking = false;
            Debug.Log("FIM DO ATAQUE");
        }
        
    }
}
