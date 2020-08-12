using Enemy;
using UnityEngine;
using System.Collections.Generic;  

namespace Player
{
    public class PowerUpController : MonoBehaviour
    {
        private enum Effects
        {
            Fire,
            Ice,
            Thunder
        };

        // Variables
        public bool HaveFireLv1 = false;
        public int OddsFireLv1 = 25;

        public int FireDamageLv1 = 25;
        public int FireDamageLv2 = 40;
        public bool HaveFireLv2 = false;
        
        public bool HaveIceLv1 = false;
        public float DefrostTimeLv1 = 1f;
        public float DefrostTimeLv2 = 1.5f;
        public int OddsIceLv1 = 15;
        public int ShatterDamage = 50;
        
        public bool HaveIceLv2 = false;

        public bool HaveThunderLv1 = false;
        public int OddsThunderLv1 = 20;
        public float ElectricRange = 1.7f;
        public int ElectricDamageLv1 = 10;
        public int ElectricDamageLv2 = 15;
        
        public bool HaveThunderLv2 = false;
        public float ParalyzeTime = 1.5f;
        public int OddsThunderLv2 = 50;

        private bool FireActivate;
        private bool IceActivate;
        private bool ElectricActivate;
        
        public bool DamageUp = false;
        public bool HpUp = false;
        public bool Heal = false;

        private PowerUpEffects PowerUpEffects;

        private void Awake()
        {
            PowerUpEffects = GetComponent<PowerUpEffects>();
        }

        public void ApplyEffects(GameObject enemy)
        {
            EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
            EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

            List<Effects> activatedEffects = new List<Effects>();

            if (HaveFireLv1)
            {
                // FIRE LV 1
                if (Random.Range(1, 100) < OddsFireLv1)
                {
                    activatedEffects.Add(Effects.Fire);
                    // FIRE LV 2 needs FIRE LV1 to happen to take effect
                }
            }

            if (HaveIceLv1)
            {
                // ICE LV 2 doesn't needs ICE LV 1 to happen in the same attack
                if (HaveIceLv2 && enemyBehavior.IsFrozen)
                {
                    //Debug.Log("SHATTERING ENEMY");
                    PowerUpEffects.ShatterEnemy(enemy, ShatterDamage);
                }
                
                // ICE LV 1
                if (Random.Range(1, 100) < OddsIceLv1)
                {
                    activatedEffects.Add(Effects.Ice);
                }
            }

            if (HaveThunderLv1)
            {
                //  THUNDER LV 1
                if (Random.Range(1, 100) < OddsThunderLv1)
                {
                    activatedEffects.Add(Effects.Thunder);
                    
                    // THUNDER LV 2 needs THUNDER LV 1 to happen to take effect
                }
            }

            // Choosing between all the activated effects
            
            int effectNumber = Random.Range(0, activatedEffects.Count);
            if (activatedEffects.Count > 0)
            {
                Effects chosenEffect = activatedEffects[effectNumber];
                switch (chosenEffect)
                {
                    case Effects.Fire:
                        //Debug.Log("FIRE CHOSEN");
                        
                        PowerUpEffects.BurnEnemy(enemy, HaveFireLv2 ? FireDamageLv2 : FireDamageLv1);
                        
                        if (HaveFireLv2 && enemyBehavior.IsOnFire && enemyBehavior.WillDieBurned)
                        {
                            // FIRE LV 2
                            //Debug.Log("BURNING ENEMY TO THE DEATH");
                            PowerUpEffects.BurnEnemyToDeath(enemy);
                        }
                        break;
                    
                    case Effects.Ice:
                        //Debug.Log("ICE CHOSEN");
                        if (!enemyBehavior.IsParalyzed)
                        {
                            PowerUpEffects.FreezeEnemy(enemy, HaveIceLv2 ? DefrostTimeLv2 : DefrostTimeLv1);   
                        }
                        else
                        {
                           //Debug.Log("YOU CAN'T FREEZE SOMEONE WHO IS PARALYZED");
                        }
                        break;
                    
                    case Effects.Thunder:
                        //Debug.Log("THUNDER CHOSEN");
                        PowerUpEffects.FindCloseEnemies(enemy, ElectricRange, HaveThunderLv2 ? ElectricDamageLv2 : ElectricDamageLv1); // AND DEALS DAMAGE TO THEM
                        
                        // THUNDER LV 2
                        if (HaveThunderLv2)
                        {
                            if(enemyBehavior.IsPrimaryTarget && Random.Range(1, 100) < OddsThunderLv2)
                            {
                                if (!enemyBehavior.IsFrozen)
                                {
                                    PowerUpEffects.ParalyzeEnemy(enemy, ParalyzeTime);
                                    //Debug.Log("PARALIZADO");    
                                }
                                else
                                {
                                    //Debug.Log("YOU CAN'T PARALYZE SOMEONE WHO IS FROZEN");
                                }
                            }
                            else
                            {
                                //Debug.Log("Tentou paralizar, mas não conseguiu");
                            }  
                        }
                        enemyBehavior.IsPrimaryTarget = false;
                        break;
                }
            }
        }
    }
}
