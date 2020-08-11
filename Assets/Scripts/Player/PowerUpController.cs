using Enemy;
using UnityEngine;

namespace Player
{
    public class PowerUpController : MonoBehaviour
    {
        // Variables
        private bool HaveFireLv1 = false;
        private int OddsFireLv1 = 25;
        
        private bool HaveFireLv2 = false;
        
        private bool HaveIceLv1 = false;
        private int OddsIceLv1 = 15;
        
        private bool HaveIceLv2 = false;

        private bool HaveThunderLv1 = false;
        private int OddsThunderLv1 = 20;
        
        private bool HaveThunderLv2 = false;
        private int OddsThunderLv2 = 25;
        
        private bool DamageUp = false;
        private bool HpUp = false;
        private bool Heal = false;

        private PowerUpEffects PowerUpEffects;

        private void Awake()
        {
            PowerUpEffects = GetComponent<PowerUpEffects>();
        }

        private void Update()
        {
            
        }

        private void ApplyEffects(GameObject enemy)
        {
            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

            if (HaveFireLv1)
            {
                // FIRE LV 1
                if (Random.Range(1, 100) < OddsFireLv1)
                {
                    PowerUpEffects.BurnEnemy();
                }

                if (HaveFireLv2 && enemyBehavior.IsOnFire)
                {
                    // FIRE LV 2
                    PowerUpEffects.BurnEnemyToDeath();
                }
            }

            if (HaveIceLv1)
            {
                // ICE  LV 1
                if (Random.Range(1, 100) < OddsIceLv1)
                {
                    PowerUpEffects.FreezeEnemy();    
                }
                
                // ICE LV 2
                if (HaveIceLv2 && enemyBehavior.IsFrozen)
                {
                    PowerUpEffects.ShatterEnemy();
                }
            }

            if (HaveThunderLv1)
            {
                //  THUNDER LV 1
                if (Random.Range(1, 100) < OddsThunderLv1)
                {
                    PowerUpEffects.ElectrifyEnemy();
                    
                    // THUNDER LV 2
                    if (HaveThunderLv2 && enemyBehavior.IsPrimaryTarget && Random.Range(1, 100) < OddsThunderLv2)
                    {
                        PowerUpEffects.ParalyzeEnemy();
                    }
                }
                
            }
        }
    }
}
