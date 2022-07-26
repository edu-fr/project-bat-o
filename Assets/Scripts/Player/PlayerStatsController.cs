using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerStatsController : MonoBehaviour
    {
        
        [Tooltip("Need to have the exact same name of the LevelUpOption optionAttributeName")]
        public enum ElementalBlessing
        {
            Fire,
            Water,
            Wind,
            Lightning,
            None
        }

        [Tooltip("Need to have the exact same name of the LevelUpOption optionAttributeName")]
        public enum ElementalRampage {
            BoilingWave,
            Tsunami,
            HeatCloak,
            StormBringer,
            FireShock,
            GoddessOfTheHunt,
            None
        }

        public enum Ability
        {
            LifeSteal
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
        private int _currentHpRecoveryLevel;


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


        public List<ElementalBlessing> currentElementalBlessingsList;
        public List<ElementalRampage> currentElementalRampagesList;

        private void Start()
        {
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalRampagesList.Add(ElementalRampage.None);
        }
        
        public ElementalRampage GetAvailableRampage()
        {
            return currentElementalBlessingsList[0] switch
            {
                ElementalBlessing.Fire => currentElementalBlessingsList[1] switch
                {
                    ElementalBlessing.Water => ElementalRampage.BoilingWave,
                    ElementalBlessing.Wind => ElementalRampage.HeatCloak,
                    ElementalBlessing.Lightning => ElementalRampage.FireShock,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Water => currentElementalBlessingsList[1] switch
                {
                    ElementalBlessing.Fire => ElementalRampage.BoilingWave,
                    ElementalBlessing.Wind => ElementalRampage.Tsunami,
                    ElementalBlessing.Lightning => ElementalRampage.StormBringer,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Wind => currentElementalBlessingsList[1] switch
                {
                    ElementalBlessing.Fire => ElementalRampage.HeatCloak,
                    ElementalBlessing.Water => ElementalRampage.Tsunami,
                    ElementalBlessing.Lightning => ElementalRampage.GoddessOfTheHunt,
                    _ => ElementalRampage.None
                },
                ElementalBlessing.Lightning => currentElementalBlessingsList[1] switch
                {
                    ElementalBlessing.Fire => ElementalRampage.FireShock,
                    ElementalBlessing.Water => ElementalRampage.StormBringer,
                    ElementalBlessing.Wind => ElementalRampage.GoddessOfTheHunt,
                    _ => ElementalRampage.None
                },
                _ => ElementalRampage.None
            };
        }
        
        public void LevelUpPower()
        {
            _currentPowerLevel++;
            print("Current attack level " + _currentPowerLevel);
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
        
        public void LevelUpHpRecovery()
        {
            _currentHpRecoveryLevel++;
            print("Current Hp Recovery level " + _currentHpRecoveryLevel);
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
        
        public void LevelUpBoilingWave()
        {
            currentElementalRampagesList[0] = ElementalRampage.BoilingWave;
            print("Current elemental rampage: " + ElementalRampage.BoilingWave.ToString());
        }
        
        public void LevelUpTsunami()
        {
            currentElementalRampagesList[0] = ElementalRampage.Tsunami;
            print("Current elemental rampage: " + ElementalRampage.Tsunami);
        }
        
        public void LevelUpHeatCloak()
        {
            currentElementalRampagesList[0] = ElementalRampage.HeatCloak;
            print("Current elemental rampage: " + ElementalRampage.HeatCloak);
        }
        
        public void LevelUpStormBringer()
        {
            currentElementalRampagesList[0] = ElementalRampage.StormBringer;
            print("Current elemental rampage: " + ElementalRampage.StormBringer);
        }
        
        public void LevelUpFireShock()
        {
            currentElementalRampagesList[0] = ElementalRampage.FireShock;
            print("Current elemental rampage: " + ElementalRampage.FireShock);
        }
        
        public void LevelUpGoddessOfTheHunt()
        {
            currentElementalRampagesList[0] = ElementalRampage.GoddessOfTheHunt;
            print("Current elemental rampage: " + ElementalRampage.GoddessOfTheHunt);
        }

        public int GetBlessingCurrentLevel(ElementalBlessing blessing)
        {
            return blessing switch
            {
                ElementalBlessing.Fire => _currentFireLevel,
                ElementalBlessing.Water => _currentWaterLevel,
                ElementalBlessing.Wind => _currentWindLevel,
                ElementalBlessing.Lightning => _currentLightningLevel,
                ElementalBlessing.None => -1,
                _ => throw new ArgumentOutOfRangeException(nameof(blessing), blessing,
                    "Not expected blessing: " + blessing.ToString())
            };
        }

        public int GetAbilityCurrentLevel(string ability)
        {
            return ability switch
            {
                "LifeSteal" => _currentLifeStealLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(ability), ability,
                    "Not expected ability: " + ability.ToString())
            };
        }

    }
}