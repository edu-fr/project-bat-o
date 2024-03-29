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
        
        
        [Header("Attack speed")] [Tooltip("Animation speed. Default = 2")] 
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
        public delegate float MaxHealthChangeHandler(float max);
        public static event MaxHealthChangeHandler MaxHealthChanged;


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
        
        [Header("Move Speed")] [Tooltip("Flat values")] 
        [SerializeField] private float startingMoveSpeed;
        private float baseMoveSpeed;
        [SerializeField] private float currentMoveSpeed;
        public float CurrentMoveSpeed => currentMoveSpeed;
        private int _currentMoveSpeedLevel;

        
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
        [SerializeField] private int maxWindStacks;
        public int MaxWindStacks => maxWindStacks;
        [SerializeField] private float timeLimitToLoseStacks;
        public float TimeLimitToLoseStacks => timeLimitToLoseStacks;
        public int currentWindStacks;
        public int CurrentWindLevel { get; private set; }

        
        [Header("Lightning")] [Tooltip("")]
        [SerializeField] private float[] currentLightningDamage;
        public float[] CurrentLightningDamage => currentLightningDamage;
        [Header("Percentage of happening on a attack")] [Range(0, 100f)]
        [SerializeField] private float[] currentLightningRate;
        public float[] CurrentLightningRate => currentLightningRate;
        [Header("Area of effect of the lightning")] [Range(0, 5f)]
        [SerializeField] private float[] currentLightningAoE;
        public float[] CurrentLightningAoE => currentLightningAoE;
        public float lightningDelay;
        public int CurrentLightningLevel { get; private set; }
        
        
        public List<ElementalBlessing> currentElementalBlessingsList;
        public List<ElementalRampage> currentElementalRampagesList;

        private PlayerHealthManager _playerHealthManager;

        private void Awake()
        {
            _playerHealthManager = GetComponent<PlayerHealthManager>();
        }

        private void Start()
        {
            /* Start without blessings or rampages */
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalBlessingsList.Add(ElementalBlessing.None);
            currentElementalRampagesList.Add(ElementalRampage.None);
            /***/
            
            /* Initialize stats */
            UpdateBaseMaxHP();
            UpdateCurrentMaxHP();
            _playerHealthManager.Heal(CurrentMaxHP);

            UpdateBasePower();
            UpdateCurrentPower();

            UpdateBaseAttackSpeed();
            UpdateCurrentAttackSpeed();
            
            UpdateBaseResistance();
            UpdateCurrentResistance();
            
            UpdateBaseLifeRecovery();
            UpdateCurrentLifeRecovery();
            
            UpdateBaseCriticalRate();
            UpdateCurrentCriticalRate();
            
            UpdateBaseCriticalDamage();
            UpdateCurrentCriticalDamage();

            UpdateBaseEvasion();
            UpdateCurrentEvasion();
            
            UpdateBaseMoveSpeed();
            UpdateCurrentMoveSpeed();
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
            UpdateBasePower();
            UpdateCurrentPower();
        }

        public void LevelUpAttackSpeed()
        {
            _currentAttackSpeedLevel++;
            print("Current attack speed level " + _currentAttackSpeedLevel);
            UpdateBaseAttackSpeed();
            UpdateCurrentAttackSpeed();
        }
        
        public void LevelUpResistance()
        {
            _currentResistanceLevel++;
            print("Current resistance level " + _currentResistanceLevel);
            UpdateBaseResistance();
            UpdateCurrentResistance();
        }
        
        public void LevelUpMaxHP()
        {
            _currentMaxHPLevel++;
            print("Current Max HP level " + _currentMaxHPLevel);
            UpdateBaseMaxHP();
            UpdateCurrentMaxHP();
        }
        
        public void LevelUpLifeRecovery()
        {
            _currentLifeRecoveryLevel++;
            print("Current Hp Recovery level " + _currentLifeRecoveryLevel);
            UpdateBaseLifeRecovery();
            UpdateCurrentLifeRecovery();
        }
        
        public void LevelUpCriticalRate()
        {
            _currentCriticalRateLevel++;
            print("Current Critical Rate level " + _currentCriticalRateLevel);
            UpdateBaseCriticalRate();
            UpdateCurrentCriticalDamage();
        }
        
        public void LevelUpCriticalDamage()
        {
            _currentCriticalDamageLevel++;
            print("Current critical damage level " + _currentCriticalDamageLevel);
            UpdateBaseCriticalDamage();
            UpdateCurrentCriticalDamage();
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
            UpdateBaseEvasion();
            UpdateCurrentEvasion();
        }
        
        public void LevelUpMoveSpeed()
        {
            _currentMoveSpeedLevel++;
            print("Current move speed level " + _currentMoveSpeedLevel);
            UpdateBaseMoveSpeed();
            UpdateCurrentMoveSpeed();
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
            if (CurrentFireLevel >= blessingMaxLevel) return; 
            SetBlessing(ElementalBlessing.Fire, CurrentFireLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Fire)) return;
            CurrentFireLevel++;
            print("Current fire level " + CurrentFireLevel);
        }
        
        public void LevelUpWater()
        {
            if (CurrentWaterLevel >= blessingMaxLevel) return; 
            SetBlessing(ElementalBlessing.Water, CurrentWaterLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Water)) return; 
            CurrentWaterLevel++;
            print("Current water level " + CurrentWaterLevel);
        }
        
        public void LevelUpWind()
        {
            if (CurrentWindLevel >= blessingMaxLevel) return; 
            SetBlessing(ElementalBlessing.Wind, CurrentWindLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Wind)) return;
            CurrentWindLevel++;
            print("Current wind level " + CurrentWindLevel);
        }
        
        public void LevelUpLightning()
        {
            if (CurrentLightningLevel >= blessingMaxLevel) return; 
            SetBlessing(ElementalBlessing.Lightning, CurrentLightningLevel);
            if (!BlessingIsEquipped(ElementalBlessing.Lightning)) return;
            CurrentLightningLevel++;
            print("Current lightning level " + CurrentLightningLevel);
        }
        
        public void LevelUpBoilingWave()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
            currentElementalRampagesList[0] = ElementalRampage.BoilingWave;
            print("Current elemental rampage: " + ElementalRampage.BoilingWave.ToString());
        }
        
        public void LevelUpTsunami()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
            currentElementalRampagesList[0] = ElementalRampage.Tsunami;
            print("Current elemental rampage: " + ElementalRampage.Tsunami);
        }
        
        public void LevelUpHeatCloak()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
            currentElementalRampagesList[0] = ElementalRampage.HeatCloak;
            print("Current elemental rampage: " + ElementalRampage.HeatCloak);
        }
        
        public void LevelUpStormBringer()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
            currentElementalRampagesList[0] = ElementalRampage.StormBringer;
            print("Current elemental rampage: " + ElementalRampage.StormBringer);
        }
        
        public void LevelUpFireShock()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
            currentElementalRampagesList[0] = ElementalRampage.FireShock;
            print("Current elemental rampage: " + ElementalRampage.FireShock);
        }
        
        public void LevelUpGoddessOfTheHunt()
        {
            if (currentElementalRampagesList[0] != ElementalRampage.None) return;
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

        private void UpdateBasePower()
        {
            // Power formula
            basePower = startingPower + (10 * _currentPowerLevel);
        }

        public void UpdateCurrentPower()
        {
            currentPower = basePower;

            // Wind blessing
            if (CurrentWindLevel >= 3)
            {
                currentPower += basePower * (0.05f * currentWindStacks);
            }
        }
        
        private void UpdateBaseAttackSpeed()
        {
            // Attack speed formula
            baseAttackSpeed = startingAttackSpeed + (0.2f * _currentAttackSpeedLevel);
        }
        
        public void UpdateCurrentAttackSpeed()
        {
            currentAttackSpeed = baseAttackSpeed;
            
            // Wind blessing
            if (CurrentWindLevel > 0)
            {
                currentAttackSpeed += baseAttackSpeed * (0.07f * currentWindStacks);
            }
        }
        
        private void UpdateBaseMaxHP()
        {
            // Max HP formula
            baseMaxHP = startingMaxHP + (5 * _currentMaxHPLevel);
        }
        
        public void UpdateCurrentMaxHP()
        {
            var amount = baseMaxHP - currentMaxHP;
            currentMaxHP = baseMaxHP;
            _playerHealthManager.UpdateMaxHP(CurrentMaxHP);
            _playerHealthManager.Heal(amount);
        }

        private void UpdateBaseResistance()
        {
            // Resistance formula
            baseResistance = startingResistance + (2 * _currentResistanceLevel);
        }
        
        private void UpdateCurrentResistance()
        {
            currentResistance = baseResistance;
        }
        
        
        private void UpdateBaseCriticalRate()
        {
            // Critical rate formula
            baseCriticalRate = startingCriticalRate + (0.1f * _currentCriticalRateLevel);
            
            // Setting the ceiling
            if (baseCriticalRate > 100) baseCriticalRate = 100;
        }
        
        public void UpdateCurrentCriticalRate()
        {
            currentCriticalRate = baseCriticalRate;
            
            // Wind blessing
            if (CurrentWindLevel >= 4)
            {
                currentCriticalRate += baseCriticalRate * (0.07f * currentWindStacks);
            }

            // Setting the ceiling
            if (currentCriticalRate > 100) currentCriticalRate = 100;
        }
        
        private void UpdateBaseCriticalDamage()
        {
            // CriticalDamage formula
            baseCriticalDamage = startingCriticalDamage + (10 * _currentCriticalDamageLevel);
        }
        
        public void UpdateCurrentCriticalDamage()
        {
            currentCriticalDamage = baseCriticalDamage;
        }
        
        private void UpdateBaseLifeRecovery()
        {
            // LifeRecovery formula
            baseLifeRecovery = startingLifeRecovery + (10 * _currentLifeRecoveryLevel);
        }
        
        public void UpdateCurrentLifeRecovery()
        {
            currentLifeRecovery = baseLifeRecovery;
        }
        
        private void UpdateBaseEvasion()
        {
            // Evasion formula
            baseEvasion = startingEvasion + (4 * _currentEvasionLevel);
        }
        
        public void UpdateCurrentEvasion()
        {
            currentEvasion = baseEvasion;
        }

        private void UpdateBaseMoveSpeed()
        {
            // MoveSpeed formula
            baseMoveSpeed = startingMoveSpeed + (0.05f * _currentMoveSpeedLevel);
        }
        
        public void UpdateCurrentMoveSpeed()
        {
            currentMoveSpeed = baseMoveSpeed;
            
            // Wind blessing
            if (CurrentWindLevel > 0)
            {
                currentMoveSpeed += baseMoveSpeed * (0.03f * currentWindStacks);
            }
        }
        
        /* Wind blessing */
        public void IncreaseWindStacks()
        {
            if (currentWindStacks > MaxWindStacks) return;
            currentWindStacks++;
            UpdateWindStats();
        }

        public void LoseWindStacks()
        {
            if (CurrentWindLevel <= 0) return;
            currentWindStacks = 0;
            UpdateWindStats();
        }

        private void UpdateWindStats()
        {
            UpdateCurrentAttackSpeed();
            UpdateCurrentMoveSpeed();

            if (CurrentWindLevel <= 2) return;
            UpdateCurrentPower();
            
            if (CurrentWindLevel <= 3) return;
            UpdateCurrentCriticalRate();
        }
        /***/
    }
}