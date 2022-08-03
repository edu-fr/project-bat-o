using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game;
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
        [SerializeField] private float startingPower;
        private float basePower;
        [SerializeField] private float currentPower;
        public float CurrentPower => currentPower;
        private int _currentPowerLevel;
        
        
        [Header("Attack speed")] [Tooltip("Value divides the animation speed.")] 
        [SerializeField] private float startingAttackSpeed;
        private float baseAttackSpeed;
        [SerializeField] private float currentAttackSpeed;
        public float CurrentAttackSpeed => currentAttackSpeed;
        private int _currentAttackSpeedLevel;


        [Header("Resistance")] [Tooltip("Reduce damage receive and the CC time.")] 
        [SerializeField] private float startingResistance;
        private float baseResistance;
        [SerializeField] private float currentResistance;
        public float CurrentResistance => currentResistance;
        private int _currentResistanceLevel;


        [Header("Max HP")] [Tooltip("")] 
        [SerializeField] private float startingMaxHP;
        private float baseMaxHP;
        [SerializeField] private float currentMaxHP;
        public float CurrentMaxHP => currentMaxHP;
        private int _currentMaxHPLevel;


        [Header("Life recovery")] [Tooltip("Additional percentage. Influences life steal, heal, etc")] 
        [SerializeField] private float startingLifeRecovery;
        private float baseLifeRecovery;
        [SerializeField] private float currentLifeRecovery;
        public float CurrentLifeRecovery => currentLifeRecovery;
        private int _currentLifeRecoveryLevel;


        [Header("Critical Rate")] [Tooltip("Percentage.")] 
        [SerializeField] private float startingCriticalRate;
        private float baseCriticalRate;
        [SerializeField] private float currentCriticalRate;
        public float CurrentCriticalRate => currentCriticalRate;
        private int _currentCriticalRateLevel;

        
        [Header("Base Critical Damage")] [Tooltip("Additional percentage.")] 
        [SerializeField] private float startingCriticalDamage;
        private float baseCriticalDamage;
        [SerializeField] private float currentCriticalDamage;
        public float CurrentCriticalDamage => currentCriticalDamage;
        private int _currentCriticalDamageLevel;
        
        [Header("Evasion")] [Tooltip("Percentage.")] 
        [SerializeField] private float startingEvasion;
        private float baseEvasion;
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
        [SerializeField] private int blessingMaxLevel;
        
        [Header("Fire")] [Tooltip("")]
        [SerializeField] private float[] totalFireDamage;
        public float[] TotalFireDamage => totalFireDamage;
        [SerializeField] private float[] intervalBetweenTicks;
        public float[] IntervalBetweenTicks => intervalBetweenTicks;
        [SerializeField] private int[] numberOfFireTicks;
        public int[] NumberOfFireTicks => numberOfFireTicks;
        public int CurrentFireLevel { get; private set; }

        
        [Header("Water")] [Tooltip("Percentage of the damage given on the enemy hit. Divide it by 100.")] 
        [SerializeField] private float[] currentWaterCleaveDamagePercentage;
        public float[] CurrentWaterCleaveDamagePercentage => currentWaterCleaveDamagePercentage;
        [SerializeField] private float[] waterCleaveRange;
        [SerializeField] [Range(45, 180)] private float waterCleaveAngle;
        public float[] WaterCleaveRange => waterCleaveRange;
        public float WaterCleaveAngle => waterCleaveAngle;
        public float splashDelay;
        public int CurrentWaterLevel { get; private set; }
        

        
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
        public int CurrentWindLevel { get; private set; }

        
        [Header("Lightning")] [Tooltip("")]
        [SerializeField] private float baseTotalLightningDamage;
        [SerializeField] private float currentTotalLightningDamage;
        public float CurrentTotalLightningDamage => currentTotalLightningDamage;
        [SerializeField] private float baseLightningAoE;
        [SerializeField] private float currentLightningAoE;
        public float CurrentLightningAoE => currentLightningAoE;
        public int CurrentLightningLevel { get; private set; }
        

        public List<ElementalBlessing> currentElementalBlessingsList;
        public List<ElementalRampage> currentElementalRampagesList;

        private void Start()
        {
            /* Start without blessings or rampages */
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalRampagesList.Add(ElementalRampage.None);
            /***/
            
            /* Initialize stats */
            UpdatePower();
            currentPower = basePower;
            
            UpdateAttackSpeed();
            currentAttackSpeed = startingAttackSpeed;
            
            UpdateResistance();
            currentResistance = startingResistance;

            UpdateMaxHP();
            currentMaxHP = startingMaxHP;
            
            UpdateLifeRecovery();
            currentLifeRecovery = startingLifeRecovery;
            
            UpdateCriticalRate();
            currentCriticalRate = startingCriticalRate;
            
            UpdateCriticalDamage();
            currentCriticalDamage = startingCriticalDamage;
            
            UpdateEvasion();
            currentEvasion = startingEvasion;
            /***/
        }
        
        public void Update() {
            /* DEBUG */
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                LevelUpFire();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                LevelUpWater();
            }
            
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                LevelUpWind();
            }
            
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                LevelUpLightning();
            }
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
            UpdatePower();
        }

        public void LevelUpAttackSpeed()
        {
            _currentAttackSpeedLevel++;
            print("Current attack speed level " + _currentAttackSpeedLevel);
            UpdateAttackSpeed();
        }
        
        public void LevelUpResistance()
        {
            _currentResistanceLevel++;
            print("Current resistance level " + _currentResistanceLevel);
            UpdateResistance();
        }
        
        public void LevelUpMaxHP()
        {
            _currentMaxHPLevel++;
            print("Current Max HP level " + _currentMaxHPLevel);
            UpdateMaxHP();
        }
        
        public void LevelUpLifeRecovery()
        {
            _currentLifeRecoveryLevel++;
            print("Current Hp Recovery level " + _currentLifeRecoveryLevel);
            UpdateLifeRecovery();
        }
        
        public void LevelUpCriticalRate()
        {
            _currentCriticalRateLevel++;
            print("Current Critical Rate level " + _currentCriticalRateLevel);
            UpdateCriticalRate();
        }
        
        public void LevelUpCriticalDamage()
        {
            _currentCriticalDamageLevel++;
            print("Current critical damage level " + _currentCriticalDamageLevel);
            UpdateCriticalDamage();
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
            UpdateEvasion();
        }

        public MethodInfo GetLevelUpFunction(string variableName)
        {
            var functionName = "LevelUp" + variableName;
            var method = GetType().GetMethod(functionName);
            if (method == null)
                throw new NullReferenceException("Method not found: " + functionName);
            return method;
        }
        
        public void LevelUpFire()
        {
            SetBlessing(ElementalBlessing.Fire, CurrentFireLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Fire)) return;
            CurrentFireLevel++;
            print("Current fire level " + CurrentFireLevel);
        }
        
        public void LevelUpWater()
        {
            SetBlessing(ElementalBlessing.Water, CurrentWaterLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Water)) return; 
            CurrentWaterLevel++;
            print("Current water level " + CurrentWaterLevel);
        }
        
        public void LevelUpWind()
        {
            SetBlessing(ElementalBlessing.Wind, CurrentWindLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Wind)) return;
            CurrentWindLevel++;
            print("Current wind level " + CurrentWindLevel);
        }
        
        public void LevelUpLightning()
        {
            SetBlessing(ElementalBlessing.Lightning, CurrentLightningLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Lightning)) return;
            CurrentLightningLevel++;
            print("Current lightning level " + CurrentLightningLevel);
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
                ElementalBlessing.Fire => CurrentFireLevel != blessingMaxLevel ? CurrentFireLevel : -1,
                ElementalBlessing.Water => CurrentWaterLevel != blessingMaxLevel ? CurrentWaterLevel : -1,
                ElementalBlessing.Wind => CurrentWindLevel != blessingMaxLevel ? CurrentWindLevel : -1,
                ElementalBlessing.Lightning => CurrentLightningLevel != blessingMaxLevel ? CurrentLightningLevel : -1,
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

        private void SetBlessing(ElementalBlessing blessing, int matchingLevelVariable)
        {
            if (matchingLevelVariable == 0) // still don't have the blessing
            {
                for (int i = 0; i < currentElementalBlessingsList.Count; i++)
                {
                    if (currentElementalBlessingsList[i] == ElementalBlessing.None)
                    {
                        currentElementalBlessingsList[i] = blessing;
                        print("Blessing equipped on slot " + i);
                        return;
                    } 
                }
                print("No slots available to equip the blessing " + blessing.ToString());
                return;
            }
            print(blessing + " level greater than zero");
        }

        private bool BlessingIsEquipped(ElementalBlessing blessing)
        {
            return currentElementalBlessingsList.Any(blessingEquipped => blessing == blessingEquipped);
        }

        private void UpdatePower()
        {
            // Power formula
            basePower = startingPower + (10 * _currentPowerLevel);
        }
        
        private void UpdateAttackSpeed()
        {
            // Attack speed formula
            baseAttackSpeed = startingAttackSpeed + (0.2f * _currentAttackSpeedLevel);
        }

        private void UpdateMaxHP()
        {
            // Max HP formula
            baseMaxHP = startingMaxHP + (5 * _currentMaxHPLevel);
        }

        private void UpdateResistance()
        {
            // Resistance formula
            baseResistance = startingResistance + (2 * _currentResistanceLevel);
        }
        
        
        private void UpdateCriticalRate()
        {
            // Critical rate formula
            baseCriticalRate = startingCriticalRate + (0.1f * _currentCriticalRateLevel);
        }
        
        
        private void UpdateCriticalDamage()
        {
            //  formula
            baseCriticalDamage = startingCriticalDamage + (10 * _currentCriticalDamageLevel);
        }
        
        
        private void UpdateLifeRecovery()
        {
            //  formula
            baseLifeRecovery = startingLifeRecovery + (10 * _currentLifeRecoveryLevel);
        }
        
        private void UpdateEvasion()
        {
            //  formula
            baseEvasion = startingEvasion + (4 * _currentEvasionLevel);
        }
        
    }
}