using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyStatsManager : MonoBehaviour
    {
        [SerializeField] private float _maxHP;
        public float MaxHP => _maxHP;
        [SerializeField] private float _physicalDamage;
        public float PhysicalDamage => _physicalDamage;
        [SerializeField] private float _magicalDamage;
        public float MagicalDamage => _magicalDamage;
        [SerializeField] private float _physicalDefense;
        public float PhysicalDefense => _physicalDefense; // divides the knockback strengh and duration (duration * 2)
        [SerializeField] private float _magicalDefense;
        public float MagicalDefense => _magicalDefense;
        [SerializeField] private float _moveSpeed;
        public float MoveSpeed => _moveSpeed;
        [SerializeField] private float _attackSpeed;
        public float AttackSpeed => _attackSpeed;
        [SerializeField] private float _attackPreparationTime;
        public float AttackPreparationTime => _attackPreparationTime;
        [SerializeField] private float _preparationWalkDistance;
        public float PreparationWalkDistance => _preparationWalkDistance;
        [SerializeField] private float _attackCooldown;
        public float AttackCooldown => _attackCooldown;
        [SerializeField] private float _attackRecoveryTime;
        public float AttackRecoveryTime => _attackRecoveryTime;
        [SerializeField] private float _distanceToAttack;
        public float DistanceToAttack => _distanceToAttack;
        [SerializeField] private float _distanceToLosePlayerSight;
        public float DistanceToLosePlayerSight => _distanceToLosePlayerSight;
        [SerializeField] private float _expDropQuantity;
        public float ExpDropQuantity => _expDropQuantity;
        [SerializeField] [Range(0, 360)] private float _fieldOfViewValue;
        public float FieldOfViewValue => _fieldOfViewValue;
        [SerializeField] [Range(0, 15)] private float _fieldOfViewDistance;
        public float FieldOfViewDistance => _fieldOfViewDistance;
        [SerializeField] [Range(0, 360)] private float _areaOfEffect;
        public float AreaOfEffect => _areaOfEffect;
        
        [SerializeField] [Range(0, 15)] private float _attackRange;
        public float AttackRange => _attackRange;
        
        [SerializeField] [Range(0, 5)] private float _crowdControlDuration;
        public float CrowdControlDuration => _crowdControlDuration;

        [SerializeField] [Range(0, 2)] private float _timeToPredictIfWillHitTheTarget;
        public float TimeToPredictIfWillHitTheTarget => _timeToPredictIfWillHitTheTarget;
    }
    
    
}
