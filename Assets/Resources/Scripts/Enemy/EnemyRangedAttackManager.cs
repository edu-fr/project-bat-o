using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyRangedAttackManager : MonoBehaviour
    { 
        public ParticleSystem ParticleSystem;
        private ArrowParticleScript ArrowParticleScript;
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        private EnemyBehavior EnemyBehavior;
        private bool AttackEnded = false;
        private float AttackCurrentRecoveryTime = 0;
        private float AttackRecoveryTime = 0.3f;
        private Vector3 PlayerDirection;
        

        private void Awake()
        {
            ArrowParticleScript = ParticleSystem.GetComponent<ArrowParticleScript>();
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void Update()
        {
            if (AttackEnded)
            {
                EnemyCombatManager.IsAttacking = false;
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    EnemyBehavior.AiPath.enabled = true;
                    EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
                    EnemyStateMachine.IsAttackingNow = false;
                }
            }
        }
        
        
        public void Attack(Vector3 playerDirection)
        {    
            PlayerDirection = playerDirection;
            EnemyBehavior.Animator.SetFloat("AttackDirX", playerDirection.x);
            EnemyBehavior.Animator.SetFloat("AttackDirY", playerDirection.y);
            EnemyBehavior.Animator.SetTrigger("Attack");
            EnemyCombatManager.IsAttacking = true;
            
            // Correcting particle system position. (All arbitrary values that could change according to the enemy sprite) 
            if (Math.Abs(PlayerDirection.x) > Math.Abs(playerDirection.y))
            {
                if (playerDirection.y < 0)
                    ParticleSystem.transform.position = transform.position + (new Vector3(playerDirection.x * 0.5f, (playerDirection.y * 0.8f) + 0.4f));
                else 
                    ParticleSystem.transform.position = transform.position + (new Vector3(playerDirection.x * 0.5f, playerDirection.y * 0.8f));
            }
            else
                ParticleSystem.transform.position = transform.position + (new Vector3(playerDirection.x * 0.5f, playerDirection.y * 0.3f));

        }

        public void ShootArrowDuringAnimation()
        {
            ArrowParticleScript.ShootArrow(PlayerDirection);
        }

        public void AttackEndRanged()
        {
            // Called by animation end
            AttackEnded = true;
            EnemyBehavior.Animator.speed = 1f;
        }
    }
}
