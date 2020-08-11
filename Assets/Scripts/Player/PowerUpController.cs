using Enemy;
using UnityEngine;

namespace Player
{
    public class PowerUpController : MonoBehaviour
    {
        // Variables
        public bool HaveFireLv1 = false;
        public int OddsFireLv1 = 25;
        
        public bool HaveFireLv2 = false;
        
        public bool HaveIceLv1 = false;
        public int OddsIceLv1 = 15;
        
        public bool HaveIceLv2 = false;

        public bool HaveThunderLv1 = false;
        public int OddsThunderLv1 = 20;
        
        public bool HaveThunderLv2 = false;
        public int OddsThunderLv2 = 25;
        
        public bool DamageUp = false;
        public bool HpUp = false;
        public bool Heal = false;

        private PowerUpEffects PowerUpEffects;

        private void Awake()
        {
            PowerUpEffects = GetComponent<PowerUpEffects>();
        }

        private void Update()
        {
            
        }

        public void ApplyEffects(GameObject enemy)
        {
            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

            if (HaveFireLv1)
            {
                // FIRE LV 1
                if (Random.Range(1, 100) < 100 /* OddsFireLv1 */ )
                {
                    PowerUpEffects.BurnEnemy(enemy);
                }

                if (HaveFireLv2 && enemyBehavior.IsOnFire)
                {
                    // FIRE LV 2
                    PowerUpEffects.BurnEnemyToDeath(enemy);
                }
            }

            if (HaveIceLv1)
            {
                // ICE  LV 1
                if (Random.Range(1, 100) < OddsIceLv1)
                {
                    PowerUpEffects.FreezeEnemy(enemy);    
                }
                
                // ICE LV 2
                if (HaveIceLv2 && enemyBehavior.IsFrozen)
                {
                    PowerUpEffects.ShatterEnemy(enemy);
                }
            }

            if (HaveThunderLv1)
            {
                //  THUNDER LV 1
                if (Random.Range(1, 100) < OddsThunderLv1)
                {
                    PowerUpEffects.ElectrifyEnemy(enemy);
                    
                    // THUNDER LV 2
                    if (HaveThunderLv2 && enemyBehavior.IsPrimaryTarget && Random.Range(1, 100) < OddsThunderLv2)
                    {
                        PowerUpEffects.ParalyzeEnemy(enemy);
                    }
                }
                
            }
        }
    }
}
