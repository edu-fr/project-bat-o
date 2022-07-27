using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Player
{
    public class PowerUpEffects : MonoBehaviour
    {
        private PlayerStatsController _playerStatsController;

        private void Awake()
        {
            _playerStatsController = GetComponent<PlayerStatsController>();
        }
        
        // [Header("Water blessing")]
        //
        // [Header("Wind blessing")]
        //
        // [Header("Lightning blessing")]
        
        private void ApplyBlessings(EnemyHealthManager enemyHealth)
        {
            var fireLevel = _playerStatsController.CurrentFireLevel;
            var waterLevel = _playerStatsController.CurrentWaterLevel;
            var windLevel = _playerStatsController.CurrentWindLevel;
            var lightningLevel = _playerStatsController.CurrentLightningLevel;

            if (fireLevel > 0)
                FireBlessing(fireLevel, enemyHealth);
            if (waterLevel > 0)
                WaterBlessing(waterLevel, enemyHealth);
            if (windLevel > 0)
                WindBlessing(windLevel, enemyHealth);
            if (lightningLevel > 0)
                LightningBlessing(lightningLevel, enemyHealth);
        }

        #region FireBlessing
        [Header("Fire blessing")]
        
        private void FireBlessing(int fireLevel, EnemyHealthManager enemyHealth)
        {
            var interval = _playerStatsController.IntervalBetweenTicks;
            var ticks = _playerStatsController.CurrentFireNumberOfTicks;
            var totalDamage = _playerStatsController.CurrentTotalFireDamage;
            
            switch (fireLevel)
            {
                case 1:
                    ApplyBurnOnEnemy(enemyHealth, totalDamage, ticks, interval);
                    break;
                
                case 2:
                    break;
                
                case 3:
                    break;
                
                case 4:
                    break;
                
            }
        }

        private IEnumerator ApplyBurnOnEnemy(EnemyHealthManager enemyHealth, float totalDamage, float ticks, float interval)
        {
            // for (int i = 0; i < ticks;)
            yield return new WaitForSeconds(interval);
        }

        private IEnumerator TickBurn(int interval)
        {
        }

        #endregion


        private void WaterBlessing(int waterLevel, EnemyHealthManager enemyHealthManager)
        {
            
        }

        private void WindBlessing(int windLevel, EnemyHealthManager enemyHealthManager)
        {
            
        }

        private void LightningBlessing(int lightningLevel, EnemyHealthManager enemyHealthManager)
        {
            
        }
    }
}
