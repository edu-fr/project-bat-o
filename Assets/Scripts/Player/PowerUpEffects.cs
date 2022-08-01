using System;
using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            _playerStatsController = GetComponent<PlayerStatsController>();
            
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
            GetEnemiesInAngle(enemy, cleaveAngle, cleaveRange, damagePercentage);
        }
        
        /* DEBUG */ 
        List<Vector2> rayDirections;

        private Vector2 lastHitPosition;
        /***/

        private void GetEnemiesInAngle(EnemyCombatManager enemy, float angle, float range, float damagePercentage)
        {
            var playerTransform = transform;
            var enemyTransform = enemy.transform;
            var enemyCurrentPosition = enemyTransform.position;
            lastHitPosition = enemyCurrentPosition;
            var attackDirection = (enemyCurrentPosition - playerTransform.position).normalized;
            var rayCount = 30;
            var angleIncrease = angle / rayCount;
            var currentAngle = UtilitiesClass.SubtractAngle(UtilitiesClass.GetAngleFromVectorFloat(attackDirection),angle / 2);
            var enemiesHit = new List<GameObject>();
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
                foreach (var hit in resultingHits)
                {
                    if (hit.collider == null)
                        continue;
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        print(hit.collider.gameObject.name);
                        if (!enemiesHit.Contains(hit.collider.gameObject))
                        {
                            enemiesHit.Add(hit.collider.gameObject);
                        }
                    }
                }
                currentAngle = UtilitiesClass.AddAngle(currentAngle, angleIncrease);
            }

            StartCoroutine(SplashOnEnemies(enemiesHit, damagePercentage));
            // hit.collider.gameObject.GetComponent<EnemyCombatManager>().TakeDamage(_playerAttackManager.CurrentAttackID,1, (hit.collider.transform.position - playerTransform.position).normalized, false, false, true, Color.cyan);
        }

        private IEnumerator SplashOnEnemies(List<GameObject> enemies, float damagePercentage)
        {
            var playerPosition = transform.position;
            yield return new WaitForSeconds(_playerStatsController.splashDelay);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                var enemyPosition = enemy.transform.position;
                var attackDirection = (enemyPosition - playerPosition).normalized;
                var enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();
                if (enemyCombatManager == null) continue;
                enemyCombatManager.TakeDamage(_playerAttackManager.CurrentAttackID, 
                    _playerStatsController.CurrentPower * damagePercentage / 100,
                    attackDirection, false, false, true, Color.blue);
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
