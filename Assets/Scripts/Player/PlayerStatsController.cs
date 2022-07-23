using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Player
{
    public class PlayerStatsController : MonoBehaviour
    {
        enum ElementalBlessing
        {
            Fire,
            Water,
            Wind,
            Lightning 
        }
        
        enum ElementalRampage {
            BoilingWave,
            Tsunami,
            HeatCloak,
            StormBringer,
            FireShock,
            GoddessOfTheHunt,
            None
        }

        [Header("STATS")]

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
        [SerializeField] private float baseFireTickDuration;
        [SerializeField] private float currentFireTickDuration;
        public float CurrentFireTickDuration => currentFireTickDuration;
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
        [SerializeField] private float baseTotalWindMoveSpeedStack;
        [SerializeField] private float currentTotalWindMoveSpeedStack;
        public float CurrentTotalWindMoveSpeedStack => currentTotalWindMoveSpeedStack;
        [SerializeField] private float baseTotalWindAttackSpeedStack;
        [SerializeField] private float currentTotalWindAttackSpeedStack;
        public float CurrentTotalWindAttackSpeedStack => currentTotalWindAttackSpeedStack;
        [SerializeField] private float baseTotalWindAttackStack;
        [SerializeField] private float currentTotalWindAttackStack;
        public float CurrentTotalWindAttackStack => currentTotalWindAttackStack;
        [SerializeField] private float baseTotalWindCriticalRateStack;
        [SerializeField] private float currentTotalWindCriticalRateStack;
        public float CurrentTotalWindCriticalRateStack => currentTotalWindCriticalRateStack;
        [SerializeField] private int numberOfWindStacks;
        public int NumberOfWindStacks => numberOfWindStacks;
        private int _currentWindLevel;

        
        [Header("Lightning")] [Tooltip("")]
        [SerializeField] private float baseTotalLightningDamage;
        [SerializeField] private float currentTotalLightningDamage;
        public float CurrentTotalLightningDamage => currentTotalLightningDamage;
        [SerializeField] private float baseLightningAoE;
        [SerializeField] private float currentLightningAoE;
        public float CurrentLightningAoE => currentLightningAoE;
        private int _currentLightningLevel;
        
        
        private ElementalBlessing _firstElementalBlessing;
        private ElementalBlessing _secondElementalBlessing;
        private ElementalRampage _currentElementalRampage;

        private ElementalRampage GetCurrentElementalRampage()
        {
            return _firstElementalBlessing switch
            {
                ElementalBlessing.Fire => _secondElementalBlessing switch
                {
                    ElementalBlessing.Water => ElementalRampage.BoilingWave,
                    ElementalBlessing.Wind => ElementalRampage.HeatCloak,
                    ElementalBlessing.Lightning => ElementalRampage.FireShock,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Water => _secondElementalBlessing switch
                {
                    ElementalBlessing.Fire => ElementalRampage.BoilingWave,
                    ElementalBlessing.Wind => ElementalRampage.Tsunami,
                    ElementalBlessing.Lightning => ElementalRampage.StormBringer,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Wind => _secondElementalBlessing switch
                {
                    ElementalBlessing.Fire => ElementalRampage.HeatCloak,
                    ElementalBlessing.Water => ElementalRampage.Tsunami,
                    ElementalBlessing.Lightning => ElementalRampage.GoddessOfTheHunt,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Lightning => _secondElementalBlessing switch
                {
                    ElementalBlessing.Fire => ElementalRampage.FireShock,
                    ElementalBlessing.Water => ElementalRampage.StormBringer,
                    ElementalBlessing.Wind => ElementalRampage.GoddessOfTheHunt,
                    _ => ElementalRampage.None
                },
                _ => ElementalRampage.None
            };
        }
        
        public void LevelUpAttackSpeed()
        {
            _currentAttackSpeedLevel++;
            print("Current attack speed level " + _currentAttackSpeedLevel);
        }
        
        public void LevelUpResistance()
        {
            _currentResistanceLevel++;
            print("Current resistance level " + _currentResistanceLevel);
        }
        
        public void LevelUpMaxHP()
        {
            _currentMaxHPLevel++;
            print("Current Max HP level " + _currentMaxHPLevel);
        }
        
        public void LevelUpLifeRecovery()
        {
            _currentLifeRecoveryLevel++;
            print("Current Life Recovery level " + _currentLifeRecoveryLevel);
        }
        
        public void LevelUpCriticalRate()
        {
            _currentCriticalRateLevel++;
            print("Current Critical Rate level " + _currentCriticalRateLevel);
        }
        
        public void LevelUpCriticalDamage()
        {
            _currentCriticalDamageLevel++;
            print("Current critical damage level " + _currentCriticalDamageLevel);
        }
        
        public void LevelUpLifeSteal()
        {
            _currentLifeStealLevel++;
            print("Current Life Steal level " + _currentLifeStealLevel);
        }
        
        public void LevelUpEvasion()
        {
            _currentEvasionLevel++;
            print("Current evasion level " + _currentEvasionLevel);
        }

        public MethodInfo GetLevelUpFunction(string variableName)
        {
            var functionName = "LevelUp" + variableName;
            var method = GetType().GetMethod(functionName);
            if (method == null)
                throw new NullReferenceException("Method not found: " + functionName);
            return method;
        }
        
        public void LevelUpFireBlessing()
        {
            _currentFireLevel++;
            print("Current fire level " + _currentFireLevel);
        }
        
        public void LevelUpWaterBlessing()
        {
            _currentWaterLevel++;
            print("Current water level " + _currentWaterLevel);
        }
        
        public void LevelUpWindBlessing()
        {
            _currentWindLevel++;
            
            print("Current wind level " + _currentWindLevel);
        }
        
        public void LevelUpLightningBlessing()
        {
            _currentLightningLevel++;
            print("Current lightning level " + _currentLightningLevel);
        }
        
        public static void LevelUpPower(PlayerStatsController playerRef)
        {
            playerRef._currentPowerLevel++;
            print("Current attack level " + playerRef._currentPowerLevel);
        }

    }
}