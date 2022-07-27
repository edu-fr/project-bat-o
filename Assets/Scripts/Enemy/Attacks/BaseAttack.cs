using UnityEngine;

namespace Enemy.Attacks
{
    public abstract class BaseAttack : MonoBehaviour
    {
        /* Components */
        protected EnemyCombatManager EnemyCombatManager;
        protected EnemyStateMachine EnemyStateMachine;
        protected EnemyMovementHandler EnemyMovementHandler;
        protected EnemyStatsManager EnemyStatsManager;
        protected EnemyAnimationController EnemyAnimationController;

        /* Mandatory variables */
        protected float _attackPreparationCurrentTime;
        protected bool _attackEnded;
        protected bool _isRecoveringFromAttack;
        protected float _attackCurrentRecoveryTime;
        public bool AttackOnCooldown { get; private set; }
        protected float AttackCurrentCooldown;
        [Header("For all kinds of attacks")]
        [SerializeField] protected float attackPreparationTime;
        public float AttackPreparationTime => attackPreparationTime;
        [SerializeField] protected float attackCooldown;
        public float AttackCooldown => attackCooldown;
        [SerializeField] protected float attackRecoveryTime;
        public float AttackRecoveryTime => attackRecoveryTime;
        [SerializeField] protected float distanceToAttack;
        public float DistanceToAttack => distanceToAttack;
        public Vector2 AttackOrigin { get; protected set; }
        
        [Header("For ranged attacks")]
        [SerializeField] [Tooltip("Distance until the attack fade")] protected float attackRange;
        public float AttackRange => attackRange;

        [Header("For some type of attacks")]
        [SerializeField] protected float preparationWalkDistance;
        public float PreparationWalkDistance => preparationWalkDistance;
        [SerializeField] [Range(0.1f, 2f)] [Tooltip("Multiply the speed of the enemy attacking")] protected float attackSpeedModifier;
        public float AttackSpeedModifier => attackSpeedModifier;
        [SerializeField] protected bool triggeredDuringAnimation;
        [SerializeField] protected bool hasAttackAnimation;
        [SerializeField] protected bool isOnHalfOfAttackAnimation;

        protected virtual void Awake()
        {
            AttackOrigin = transform.position;
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyMovementHandler = GetComponent<EnemyMovementHandler>();
            EnemyStatsManager = GetComponent<EnemyStatsManager>();
            EnemyAnimationController = GetComponent<EnemyAnimationController>();
        }

        protected virtual void Update()
        {
            AttackOrigin = transform.position;
            if (_isRecoveringFromAttack) 
                RecoverFromAttacking();
            if (AttackOnCooldown)
                CheckAttackCooldown();
        }

        public abstract void PreparingAttack();

        public abstract void Attack(Vector3 playerDirection);

        public virtual void AttackEnd() // Called by animation end
        {
            _attackEnded = true;
            _isRecoveringFromAttack = true;
            EnemyAnimationController.SetAnimationSpeedToDefault();
        }

        private void RecoverFromAttacking()
        {
            _attackCurrentRecoveryTime += Time.deltaTime;
            if (!(_attackCurrentRecoveryTime > AttackRecoveryTime)) return;
            _isRecoveringFromAttack = false;
            _attackCurrentRecoveryTime = 0;
            AttackOnCooldown = true;
            
            /* Get back to action*/
            EnemyStateMachine.isAttackingNow = false;
            EnemyMovementHandler.aiPath.enabled = true;
            EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
        }

        private void CheckAttackCooldown()
        {
            if (!AttackOnCooldown) return;
            AttackCurrentCooldown += Time.deltaTime;
            if (AttackCurrentCooldown > AttackCooldown)
            {
                AttackCurrentCooldown = 0;
                AttackOnCooldown = false;
            }
        }
    }
}
