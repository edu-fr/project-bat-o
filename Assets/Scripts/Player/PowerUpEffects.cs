using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Game;
using UnityEngine;
using UnityEngine.UIElements;
using Useful;
using Random = UnityEngine.Random;

namespace Player
{
    public class PowerUpEffects : MonoBehaviour
    {
        private PlayerStatsController _playerStats;
        private PlayerAttackManager _playerAttackManager;

        /* DEBUG */ 
        List<Vector2> rayDirections;
        private Vector2 lastHitPosition;
        private float rayLenght;
        /***/
        
        private void Awake()
        {
            _playerStats = GetComponent<PlayerStatsController>();
            _playerAttackManager = GetComponent<PlayerAttackManager>();
            
            /* DEBUG */
            lastHitPosition = Vector2.zero;
            rayDirections = new List<Vector2>();
            /***/
        }

        private void Update()
        {
            /* Wind blessing */
            if (_playerStats.CurrentWindLevel <= 0) return;
            _timeSinceLastAttack += Time.deltaTime;
            if (_timeSinceLastAttack > _playerStats.TimeLimitToLoseStacks)
            {
                _playerStats.LoseWindStacks();
                _timeSinceLastAttack = 0;
            }
            /***/
            
            /* Tsunami Rampage */
            if (!_tsunamiReady)
            {
                if (_tsunamiCurrentCooldown > _playerStats.TsunamiCooldown)
                {
                    AudioManager.instance.Play("Wave 5");
                    _tsunamiReady = true;
                }
                else   
                    _tsunamiCurrentCooldown += Time.deltaTime;
            } 
            /***/
        }

        #region Blessings

        public void ApplyBlessings(EnemyCombatManager enemy)
        {
            var fireLevel = _playerStats.CurrentFireLevel;
            var waterLevel = _playerStats.CurrentWaterLevel;
            var windLevel = _playerStats.CurrentWindLevel;
            var lightningLevel = _playerStats.CurrentLightningLevel;

            if (fireLevel > 0)
                FireBlessing(fireLevel, enemy);
            if (waterLevel > 0)
                WaterBlessing(waterLevel, enemy);
            if (windLevel > 0)
                WindBlessing();
            if (lightningLevel > 0)
                LightningBlessing(lightningLevel, enemy);
        }

        public void ApplyBlessingsExcept(EnemyCombatManager enemy, PlayerStatsController.ElementalBlessing blessing)
        {
            var fireLevel = _playerStats.CurrentFireLevel;
            var waterLevel = _playerStats.CurrentWaterLevel;
            var windLevel = _playerStats.CurrentWindLevel;
            var lightningLevel = _playerStats.CurrentLightningLevel;

            if (fireLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Fire)
                FireBlessing(fireLevel, enemy);
            if (waterLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Water)
                WaterBlessing(waterLevel, enemy);
            if (windLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Wind)
                WindBlessing();
            if (lightningLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Lightning)
                LightningBlessing(lightningLevel, enemy);
        }

        #region FireBlessing

        private void FireBlessing(int fireLevel, EnemyCombatManager enemy)
        {
            var interval = _playerStats.IntervalBetweenTicks[fireLevel - 1];
            var ticks = _playerStats.NumberOfFireTicks[fireLevel - 1];
            var totalDamage = _playerStats.TotalFireDamage[fireLevel - 1];
            var criticalRate = fireLevel > 3 ? _playerStats.CurrentCriticalRate
                : 0;
            var criticalDamage = fireLevel > 3 ? _playerStats.CurrentCriticalDamage
                : 0;
            
            enemy.StartBurning(totalDamage,  ticks, interval, criticalRate, criticalDamage);
        }

        #endregion

        
        #region WaterBlessing

        private void WaterBlessing(int waterLevel, EnemyCombatManager enemy)
        {
            /* Tsunami rampage*/
            if (_playerStats.currentElementalRampage == PlayerStatsController.ElementalRampage.Tsunami && _tsunamiReady) 
                return;
            /***/
            
            var attackID = _playerAttackManager.CurrentAttack.AttackID;
            var damagePercentage = _playerStats.CurrentWaterCleaveDamagePercentage[waterLevel - 1];
            var cleaveRange = _playerStats.WaterCleaveRange[waterLevel - 1];
            var cleaveAngle = _playerStats.WaterCleaveAngle;
            WaterBlessingsActivation(attackID, enemy, cleaveAngle, cleaveRange, damagePercentage, waterLevel);
        }

        private void WaterBlessingsActivation(int attackID, EnemyCombatManager enemy, float cleaveAngle, float cleaveRange, float damagePercentage, int waterLevel)
        {
            var enemiesHitByTheSplash = GetEnemiesInAngle(attackID, enemy, cleaveAngle, cleaveRange);
            StartCoroutine(SplashOnEnemies(attackID, enemiesHitByTheSplash, damagePercentage, waterLevel));
        }
        
        private List<Transform> GetEnemiesInAngle(int attackID, EnemyCombatManager enemy, float angle, float range)
        {
            var playerTransform = transform;
            var enemyTransform = enemy.transform;
            var enemyCurrentPosition = enemyTransform.position;
            lastHitPosition = enemyCurrentPosition;
            var attackDirection = (enemyCurrentPosition - playerTransform.position).normalized;
            var rayCount = 30;
            var angleIncrease = angle / rayCount;
            var currentAngle = UtilitiesClass.SubtractAngle(UtilitiesClass.GetAngleFromVectorFloat(attackDirection),angle / 2);
            var enemiesHit = new List<Transform>();
            
            /* DEBUG */ 
            rayDirections = new List<Vector2>();
            rayLenght = range;
            /***/

            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask( LayerMask.GetMask("Enemy"));
            List<RaycastHit2D> resultingHits = new List<RaycastHit2D>();
            for (int i = 0; i < rayCount; i++)
            {
                Physics2D.Raycast(enemyCurrentPosition,
                    UtilitiesClass.GetVectorFromAngle(currentAngle), contactFilter, resultingHits, range);
                /* Debug */
                rayDirections.Add(UtilitiesClass.GetVectorFromAngle(currentAngle));
                /***/
                // Add to the list of enemies hit
                foreach (var hit in from hit in resultingHits where hit.transform != null where hit.transform.CompareTag("Enemy") where !enemiesHit.Contains(hit.transform) select hit)
                    enemiesHit.Add(hit.transform);
                currentAngle = UtilitiesClass.AddAngle(currentAngle, angleIncrease);
            }
            return enemiesHit;
        }

        private IEnumerator SplashOnEnemies(int attackID, List<Transform> enemies, float damagePercentage, int waterLevel)
        {
            var playerPosition = transform.position;
            yield return new WaitForSeconds(_playerStats.splashDelay);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                var enemyPosition = enemy.transform.position;
                var attackDirection = (enemyPosition - playerPosition).normalized;
                var enemyCombat = enemy.GetComponent<EnemyCombatManager>(); 
                if (enemyCombat == null) continue;
                
                // If water level >= 4, critical hits splash too
                enemyCombat?.TakeDamage(_playerAttackManager.CurrentAttack.AttackID,
                    _playerStats.CurrentPower *
                    (waterLevel >= 4 && _playerAttackManager.CurrentAttack.CriticalHit
                        ? _playerStats.CurrentCriticalDamage / 100
                        : 1) * damagePercentage / 100, attackDirection, false,
                    waterLevel >= 4 && _playerAttackManager.CurrentAttack.CriticalHit, true, Color.blue);
                
                // If water level > 3, apply the other blessings to the enemies hit by the splash
                if (waterLevel >= 3)
                {
                    ApplyBlessingsExcept(enemyCombat, PlayerStatsController.ElementalBlessing.Water);
                }
            }
        }
        
        #endregion

        
        #region WindBlessing

        private float _timeSinceLastAttack;
        
        private void WindBlessing()
        {
            _timeSinceLastAttack = 0; 
            _playerStats.IncreaseWindStacks();
        }

        #endregion
        
        #region LightningBlessing
        
        private void LightningBlessing(int lightningLevel, EnemyCombatManager enemy)
        {
            var chance = Random.Range(0, 100);
            if (chance < _playerStats.CurrentLightningRate[lightningLevel])
            {
                var area = _playerStats.CurrentLightningAoE[lightningLevel];
                var damage = _playerStats.CurrentLightningDamage[lightningLevel];
                var lightningDelay = _playerStats.lightningDelay + Random.Range(-_playerStats.lightningDelayVariation, _playerStats.lightningDelayVariation); 
                StartCoroutine(InvokeLightning(enemy, damage, area, lightningDelay));    
            }
        }

        private IEnumerator InvokeLightning(EnemyCombatManager enemy, float damage, float area, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // TODO animation
            var enemiesAffected = Physics2D.OverlapCircleAll(enemy.transform.position, area, LayerMask.GetMask("Enemy"));
            var lightningID = Random.Range(0, 100000);
            // Cast on the original enemy hit
            enemy.GetComponent<EnemyCombatManager>().TakeDamage(lightningID, damage, Vector3.zero, false, false, true, Color.yellow);
            foreach (var enemyAffected in enemiesAffected)
            {
                enemyAffected.GetComponent<EnemyCombatManager>().TakeDamage(lightningID, damage, Vector3.zero, false, false, true, Color.yellow);
            }
            
        }
        
        #endregion

        #endregion Blessings
        
        
        #region Rampages
        
        public void ApplyRampage(EnemyCombatManager enemy)
        {
            switch (_playerStats.currentElementalRampage)
            {
                case PlayerStatsController.ElementalRampage.BoilingWave:
                    
                    break;
                
                case PlayerStatsController.ElementalRampage.Tsunami:
                    if (_tsunamiReady)
                    {
                        var attackID = _playerAttackManager.CurrentAttack.AttackID;
                        var waterLevel = _playerStats.CurrentWaterLevel;
                        var damagePercentage = _playerStats.CurrentWaterCleaveDamagePercentage[waterLevel - 1] + _playerStats.TsunamiExtraDamagePercentage;
                        var cleaveRange = _playerStats.WaterCleaveRange[waterLevel - 1] * _playerStats.TsunamiRangeMultiplier;
                        var cleaveAngle = _playerStats.WaterCleaveAngle * _playerStats.TsunamiCleaveAngleMultiplier;
                        AudioManager.instance.Play("Wave 3");
                        WaterBlessingsActivation(attackID, enemy, cleaveAngle, cleaveRange, damagePercentage, waterLevel);
                        _tsunamiCurrentCooldown = 0;
                        _tsunamiReady = false;
                    }
                    break;
                
                case PlayerStatsController.ElementalRampage.HeatCloak:
                    
                    break;
                
                case PlayerStatsController.ElementalRampage.StormBringer:
                    
                    break;
                
                case PlayerStatsController.ElementalRampage.FireShock:
                    
                    break;
                
                case PlayerStatsController.ElementalRampage.GoddessOfTheHunt:
                    
                    break;
                
                case PlayerStatsController.ElementalRampage.None:
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #region Boiling Wave

        #endregion
        
        
        #region Tsunami
        
        private bool _tsunamiReady;
        private float _tsunamiCurrentCooldown;
        
        
        
        #endregion 
        
        #endregion Rampages

        private void OnDrawGizmos()
        {
            if (_playerStats == null) return; 
            if (_playerStats.CurrentWaterLevel <= 0) return;
            if (rayDirections == null) return;
            if (rayDirections.Count == 0) return; 
            Gizmos.color = Color.red;
            foreach (var ray in rayDirections)
            {
                Gizmos.DrawRay(lastHitPosition, ray * rayLenght );
            }
        }
    }
}
