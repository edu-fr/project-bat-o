using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using UnityEngine;
using UnityEngine.UIElements;
using Useful;
using Random = UnityEngine.Random;

namespace Player
{
    public class PowerUpEffects : MonoBehaviour
    {
        private PlayerStatsController _playerStatsController;
        private PlayerAttackManager _playerAttackManager;
        
        /* DEBUG */ 
        List<Vector2> rayDirections;

        private Vector2 lastHitPosition;
        /***/
        
        private void Awake()
        {
            _playerStatsController = GetComponent<PlayerStatsController>();
            _playerAttackManager = GetComponent<PlayerAttackManager>();
            
            /* DEBUG */
            lastHitPosition = Vector2.zero;
            rayDirections = new List<Vector2>();
            /***/
        }
        
        // [Header("Water blessing")]
        //
        // [Header("Wind blessing")]
        //
        // [Header("Lightning blessing")]
        
        public void ApplyBlessings(EnemyCombatManager enemy)
        {
            var fireLevel = _playerStatsController.CurrentFireLevel;
            var waterLevel = _playerStatsController.CurrentWaterLevel;
            var windLevel = _playerStatsController.CurrentWindLevel;
            var lightningLevel = _playerStatsController.CurrentLightningLevel;

            if (fireLevel > 0)
                FireBlessing(fireLevel, enemy);
            if (waterLevel > 0)
                WaterBlessing(waterLevel, enemy);
            if (windLevel > 0)
                WindBlessing(windLevel, enemy);
            if (lightningLevel > 0)
                LightningBlessing(lightningLevel, enemy);
        }

        public void ApplyBlessingsExcept(EnemyCombatManager enemy, PlayerStatsController.ElementalBlessing blessing)
        {
            var fireLevel = _playerStatsController.CurrentFireLevel;
            var waterLevel = _playerStatsController.CurrentWaterLevel;
            var windLevel = _playerStatsController.CurrentWindLevel;
            var lightningLevel = _playerStatsController.CurrentLightningLevel;

            if (fireLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Fire)
                FireBlessing(fireLevel, enemy);
            if (waterLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Water)
                WaterBlessing(waterLevel, enemy);
            if (windLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Wind)
                WindBlessing(windLevel, enemy);
            if (lightningLevel > 0 && blessing != PlayerStatsController.ElementalBlessing.Lightning)
                LightningBlessing(lightningLevel, enemy);
        }
        
        #region FireBlessing

        private void FireBlessing(int fireLevel, EnemyCombatManager enemy)
        {   
            // Stop on going burning events
            StopAllCoroutines();
            
            var interval = _playerStatsController.IntervalBetweenTicks[fireLevel - 1];
            var ticks = _playerStatsController.NumberOfFireTicks[fireLevel - 1];
            var totalDamage = _playerStatsController.TotalFireDamage[fireLevel - 1];
            var criticalRate = fireLevel > 3 ? _playerStatsController.CurrentCriticalRate
                : 0;
            var criticalDamage = fireLevel > 3 ? _playerStatsController.CurrentCriticalDamage
                : 0;
            StartCoroutine(ApplyBurnOnEnemy(enemy, totalDamage, ticks, interval, criticalRate, criticalDamage));
        }

        private IEnumerator ApplyBurnOnEnemy(EnemyCombatManager enemy, float totalDamage, float ticks, float interval, float criticalRate, float criticalDamage)
        {
            for (var i = 0; i < ticks; i++)
            {
                if (enemy == null) yield break;
                TickBurn(enemy, totalDamage/ticks, criticalRate, criticalDamage);
                yield return new WaitForSeconds(interval);
            }
        }

        private void TickBurn(EnemyCombatManager enemy, float damage, float criticalChance, float criticalDamage)
        {
            var criticalTest = Random.Range(0, 100);
            var burnId = Random.Range(1, 10000);
            print("Ticking! Damage: " + damage + " Chance: " + criticalChance + " Damage: " + criticalDamage);
            enemy?.TakeDamage(burnId, damage * (criticalTest < criticalChance ? criticalDamage / 100 : 1), Vector3.zero,
                true, criticalTest < criticalChance, true, null);
        }

        #endregion


        private void WaterBlessing(int waterLevel, EnemyCombatManager enemy)
        { 
            var damagePercentage = _playerStatsController.CurrentWaterCleaveDamagePercentage[waterLevel - 1];
            var cleaveRange = _playerStatsController.WaterCleaveRange[waterLevel - 1];
            var cleaveAngle = _playerStatsController.WaterCleaveAngle;
            GetEnemiesInAngle(enemy, cleaveAngle, cleaveRange, damagePercentage, waterLevel);
        }
        
        private void GetEnemiesInAngle(EnemyCombatManager enemy, float angle, float range, float damagePercentage, int waterLevel)
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

            StartCoroutine(SplashOnEnemies(enemiesHit, damagePercentage, waterLevel));
        }

        private IEnumerator SplashOnEnemies(List<Transform> enemies, float damagePercentage, int waterLevel)
        {
            var playerPosition = transform.position;
            yield return new WaitForSeconds(_playerStatsController.splashDelay);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                var enemyPosition = enemy.transform.position;
                var attackDirection = (enemyPosition - playerPosition).normalized;
                var currentAttackID = _playerAttackManager.CurrentAttack;
                var enemyCombat = enemy.GetComponent<EnemyCombatManager>(); 
                if (enemyCombat == null) continue;
                
                // If water level >= 4, critical hits splash too
                enemyCombat?.TakeDamage(_playerAttackManager.CurrentAttack.AttackID,
                    _playerStatsController.CurrentPower *
                    (waterLevel >= 4 && _playerAttackManager.CurrentAttack.CriticalHit
                        ? _playerStatsController.CurrentCriticalDamage / 100
                        : 1) * damagePercentage / 100, attackDirection, false,
                    waterLevel >= 4 && _playerAttackManager.CurrentAttack.CriticalHit, true, Color.blue);
                
                // If water level > 3, apply the other blessings to the enemies hit by the splash
                if (waterLevel >= 3)
                {
                    ApplyBlessingsExcept(enemyCombat, PlayerStatsController.ElementalBlessing.Water);
                }
            }
        }

        private void WindBlessing(int windLevel, EnemyCombatManager enemy)
        {
            
        }

        private void LightningBlessing(int lightningLevel, EnemyCombatManager enemy)
        {
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var ray in rayDirections)
            {
                Gizmos.DrawRay(lastHitPosition, ray * _playerStatsController.WaterCleaveRange[_playerStatsController.CurrentWaterLevel - 1] );
            }
        }
    }
}
