using UnityEngine;

namespace Enemy.Attacks
{
    public abstract class BaseAttack : MonoBehaviour
    {
        public enum DamageType
        {
            Physical,
            Magical,
        }
        
        protected EnemyCombatManager EnemyCombatManager;
        protected EnemyStateMachine EnemyStateMachine;
        protected EnemyMovementHandler EnemyMovementHandler;
        protected EnemyStatsManager EnemyStatsManager;
        protected EnemyAnimationController EnemyAnimationController;

        protected float AttackPreparationCurrentTime;
        protected bool AlreadyPredicted;
        private bool AttackEnded;
        private float AttackCurrentRecoveryTime;
        private float AttackRecoveryTime;
        [HideInInspector] public bool AttackOnCooldown;
        private float AttackCurrentCooldown;
        
        public Vector2 AttackOrigin { get; protected set; }
        
        public bool ProbablyGonnaHit { get; protected set; }
        
        [SerializeField] protected bool TriggeredDuringAnimation;

        protected virtual void Awake()
        {
            AttackOrigin = transform.position;
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
            CheckAttackCooldown();
            CheckRecoverTimeAfterAttacking();
        }

        public abstract void PreparingAttack();

        public abstract void Attack(Vector3 playerDirection);

        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void AttackEnd() // Called by animation end
        {
            AttackEnded = true;
            EnemyMovementHandler.EnemyAnimationController.SetAnimationSpeedToDefault();
            AttackOnCooldown = true;
        }

        private void CheckAttackCooldown()
        {
            if (!AttackOnCooldown) return;
            AttackCurrentCooldown += Time.deltaTime;
            if (AttackCurrentCooldown > EnemyStatsManager.AttackCooldown)
            {
                AttackCurrentCooldown = 0;
                AttackOnCooldown = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CheckRecoverTimeAfterAttacking()
        {
            if (!AttackEnded || EnemyStateMachine.State != EnemyStateMachine.States.Attacking) return;
            EnemyCombatManager.IsAttacking = false;
            AttackCurrentRecoveryTime += Time.deltaTime;
            if (!(AttackCurrentRecoveryTime > AttackRecoveryTime)) return;
            AttackCurrentRecoveryTime = 0;
            AttackEnded = false;
            EnemyStateMachine.IsAttackingNow = false;
            EnemyMovementHandler.AiPath.enabled = true;
            EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
        }

        protected abstract bool WillHitTheTarget(Vector3 playerPositionOrDirection);

    }
}
