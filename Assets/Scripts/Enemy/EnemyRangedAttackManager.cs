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
        private float AttackRecoveryTime = 1.5f;
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
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    AttackEnded = true;
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
            EnemyBehavior.Animator.SetTrigger("Attack");
            PlayerDirection = playerDirection;
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
