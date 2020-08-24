using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttackManager : MonoBehaviour
    {
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        private EnemyBehavior EnemyBehavior;
        private float AttackVelocity = 12f;
        private bool AttackEnded = false;
        private float AttackCurrentRecoveryTime = 0;
        private float AttackRecoveryTime = 1.5f;

        private void Awake()
        {
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void Update()
        {
            if (AttackEnded)
            {
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    AttackEnded = false;
                    EnemyStateMachine.IsAttacking = false;
                    EnemyBehavior.AiPath.enabled = true;
                    EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
                }
            }
        } 
        
        public void Attack(Vector3 playerDirection)
        {
            EnemyBehavior.Animator.SetFloat("AttackDirX", playerDirection.x);
            EnemyBehavior.Animator.SetFloat("AttackDirY", playerDirection.y);
            EnemyBehavior.Animator.speed = 3.5f;
            EnemyBehavior.Animator.SetTrigger("Attack");
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * AttackVelocity, ForceMode2D.Impulse);
        }

        public void AttackEnd()
        {
            // Called by animation end
            AttackEnded = true;
            EnemyBehavior.Animator.speed = 1f;
        }
        
    }
}
