using UnityEngine;

namespace Enemy
{
    public class EnemyStatsManager : MonoBehaviour
    {
        [Header("Attack")] [Tooltip("")] 
        [SerializeField] private float basePower;
        [SerializeField] private float currentPower;
        public float CurrentPower => currentPower;
        private int _currentPowerLevel;
        
        [Header("Attack speed")] [Tooltip("Value divides the animation speed.")] 
        [SerializeField] private float baseAttackSpeed;
        [SerializeField] private float currentAttackSpeed;
        public float CurrentAttackSpeed => currentAttackSpeed;
        private int _currentAttackSpeedLevel;
        
        [Header("Resistance")] [Tooltip("Reduce damage receive and the CC time.")] 
        [SerializeField] private float baseResistance;
        [SerializeField] private float currentResistance;
        public float CurrentResistance => currentResistance;
        private int _currentResistanceLevel;
        
        [Header("Max HP")] [Tooltip("")] 
        [SerializeField] private float baseMaxHP;
        [SerializeField] private float currentMaxHP;
        public float CurrentMaxHP => currentMaxHP;
        private int _currentMaxHPLevel;

        [Header("Height")] [Tooltip("How light the enemy is. 0 => Don't take knock back. 20 => Take an absurd amount of knock back.")]
        [SerializeField] [Range(0, 20)] private float lightness;
        public float Lightness => lightness;
        
        [SerializeField] private bool attackIsStoppedByPlayer;
        public bool AttackIsStoppedByPlayer => attackIsStoppedByPlayer;

        [Header("Speed")]
        [SerializeField] [Range(0f, 10f)] private float moveSpeed;
        public float MoveSpeed => moveSpeed;
        [SerializeField] [Range(0f, 15f)] private float chasingSpeed;
        public float ChasingSpeed => chasingSpeed;
        
        [Header("Player chase related variables")]
        [SerializeField] private float distanceToLosePlayerSight;
        public float DistanceToLosePlayerSight => distanceToLosePlayerSight;
        
        [SerializeField] [Tooltip("0 to 360º")] [Range(0, 360)] private float fieldOfViewValue;
        public float FieldOfViewValue => fieldOfViewValue;
        [SerializeField] [Range(0, 15)] private float fieldOfViewDistance;
        public float FieldOfViewDistance => fieldOfViewDistance;

        [SerializeField] [Range(0f, 15f)] private float searchForAlliesRange;
        public float SearchForAlliesRange => searchForAlliesRange;
        public float immunitySeconds;

    }
    
    
}
