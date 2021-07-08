using UnityEngine;

namespace Resources.Scripts.Enemy.Attacks
{
    public abstract class BaseAttack : MonoBehaviour
    {
        protected EnemyCombatManager EnemyCombatManager;
        protected EnemyStateMachine EnemyStateMachine;
        protected EnemyMovementHandler EnemyMovementHandler;
        protected EnemyStatsManager EnemyStatsManager;
        protected EnemyAnimationController EnemyAnimationController;

        private bool AttackEnded;
        private float AttackCurrentRecoveryTime;
        private float AttackRecoveryTime;
        public bool ProbablyGonnaHit { get; protected set; }

        protected virtual void Awake()
        {
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyMovementHandler = GetComponent<EnemyMovementHandler>();
            EnemyStatsManager = GetComponent<EnemyStatsManager>();
            EnemyAnimationController = GetComponent<EnemyAnimationController>();
        }
        protected virtual void Start()
        {
            AttackRecoveryTime = EnemyStatsManager.AttackRecoveryTime;
            // get cooldowns, etc
        }

        protected virtual void Update()
        {
            if (AttackEnded && EnemyStateMachine.State == EnemyStateMachine.States.Attacking) // Only continue if the enemy was attacking and the attack ended
            {
                EnemyCombatManager.IsAttacking = false;
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    AttackEnded = false;
                    EnemyStateMachine.IsAttackingNow = false;
                    EnemyMovementHandler.AiPath.enabled = true;
                    EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
                }
            }

        }

        public abstract void PreparingAttack();

        public abstract void Attack(Vector3 playerDirection);

        // ReSharper disable once MemberCanBeProtected.Global
        public virtual void AttackEnd() // Called by animation end
        {
            AttackEnded = true;
            EnemyMovementHandler.EnemyAnimationController.SetAnimationSpeedToDefault();
        }
    }
}
