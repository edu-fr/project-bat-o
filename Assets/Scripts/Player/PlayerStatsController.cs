using System;
using UnityEngine;

namespace Player
{
    public class PlayerStatsController : MonoBehaviour
    {
        private PowerUpController _powerUpController;

        [Header("STATS")]

        [Header("Attack")] [Tooltip("")] 
        [SerializeField] private float baseAttack;
        [SerializeField] private float currentAttack;
        public float CurrentAttack => currentAttack;
        private int _currentAttackLevel;
        
        
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


        [Header("Life recovery")] [Tooltip("Additional percentage. Influences life steal, heal, etc")] 
        [SerializeField] private float baseLifeRecovery;
        [SerializeField] private float currentLifeRecovery;
        public float CurrentLifeRecovery => currentLifeRecovery;
        private int _currentLifeRecoveryLevel;


        [Header("Critical Rate")] [Tooltip("Percentage.")] 
        [SerializeField] private float baseCriticalRate;
        [SerializeField] private float currentCriticalRate;
        public float CurrentCriticalRate => currentCriticalRate;
        private int _currentCriticalRateLevel;

        
        [Header("Base Critical Damage")] [Tooltip("Additional percentage.")] 
        [SerializeField] private float baseCriticalDamage;
        [SerializeField] private float currentCriticalDamage;
        public float CurrentCriticalDamage => currentCriticalDamage;
        private int _currentCriticalDamageLevel;
        
        [Header("Evasion")] [Tooltip("Percentage.")] 
        [SerializeField] private float baseEvasion;
        [SerializeField] private float currentEvasion;
        public float CurrentEvasion => currentEvasion;
        private int _currentEvasionLevel;
        

        [Header("ABILITIES")]
        
        [Tooltip("Percentage of HP recovered per trigger")] 
        [SerializeField] private float baseLifeSteal;
        [SerializeField] private float currentLifeSteal;
        public float CurrentLifeSteal => currentLifeSteal;
        private int _currentLifeStealLevel;

        
        [Header("ELEMENTAL")] 
        
        [Header("Fire")] [Tooltip("")]
        [SerializeField] private float baseTotalFireDamage;
        [SerializeField] private float currentTotalFireDamage;
        public float CurrentTotalFireDamage => currentTotalFireDamage;
        [SerializeField] private float baseFireTotalDuration;
        [SerializeField] private float currentFireTotalDuration;
        public float CurrentFireTotalDuration => currentFireTotalDuration;

        [SerializeField] private int baseFireNumberOfTicks;
        [SerializeField] private int currentFireNumberOfTicks;
        public int CurrentFireNumberOfTicks => currentFireNumberOfTicks; 
        
        [SerializeField] private float basePerTickFireDamage;
        [SerializeField] private float currentPerTickFireDamage;
        public float CurrentPerTickFireDamage => currentPerTickFireDamage;
        [SerializeField] private float baseFirePerTickDuration;
        [SerializeField] private float currentFirePerTickDuration;
        public float CurrentFirePerTickDuration => currentFirePerTickDuration;
        private int _currentFireLevel;


        [Header("Water")] [Tooltip("Percentage of the damage given on the enemy hit.")] 
        [SerializeField] private float baseWaterCleaveDamage;
        [SerializeField] private float currentWaterCleaveDamage;
        public float CurrentWaterCleaveDamage => currentWaterCleaveDamage;
        private int _currentWaterLevel;
        [SerializeField] private float waterCleaveRange;
        [SerializeField] [Range(45, 180)] private float waterCleaveAngle;
        public float WaterCleaveRange => waterCleaveRange;
        public float WaterCleaveAngle => waterCleaveAngle;

        [Header("Wind")] [Tooltip("Percentage of the total stats accumulated.")] 
        [SerializeField] private float BaseTotalWindStack;
        public float CurrentTotalWindStack { get; private set; }

        public int CurrentWindLevel { get; private set; }

        private void Awake()
        {
            _powerUpController = GetComponent<PowerUpController>();
        }
    }
}