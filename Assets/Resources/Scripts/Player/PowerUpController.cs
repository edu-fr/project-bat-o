using Enemy;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Player
{
    public class PowerUpController : MonoBehaviour
    {
        public enum Effects
        {
            Fire,
            Ice,
            Thunder,
            None
        };

        public int FireLevel = 0;
        public int IceLevel = 0;
        public int ElectricLevel = 0;
        
        // Variables
        public int OddsFireLv1 = 25; // 25% chance of burning the enemy 

        public int FireDamageLv1 = 25; // Deals 25 damage in 4 ticks
        public int FireDamageLv2 = 40; // Deals 40 damage in 4 ticks
       
        public float DefrostTimeLv1 = 1f; // Freeze the enemy for 1 second
        public float DefrostTimeLv2 = 1.5f; // Freeze the enemy for 2 seconds
        public int OddsIceLv1 = 15; // 15% chance of freezing the enemy
        public int ShatterDamage = 50; // Deals 50 damage to a frozen enemy 
        
        public int OddsThunderLv1 = 20; // 20% chance of deal electric damage to an enemy and up to two enemies nearby
        public float ElectricRange = 1.7f; // Up to two enemies on this range will receive electric damage
        public int ElectricDamageLv1 = 10; // Deals 10 damage to an enemy and up to two enemies nearby
        public int ElectricDamageLv2 = 15; // Deals 15 damage to an enemy and up to two enemies nearby

         public float ParalyzeTime = 1.5f; // Paralyze the enemy  for 1.5 seconds
        public int OddsThunderLv2 = 50; // 50% chance of paralyze the primary target hit by and electric attack

        private bool FireActivate;
        private bool IceActivate;
        private bool ElectricActivate;

        public float DamageUpMultiplier = 0.15f; // Increase the player attack by 15%
        public float HPUpMultiplier = 0.10f; // Increase the player max health by 10%
        public float HealPercentage = 0.4f; // Recover 40% of the player's max hp

        private PowerUpEffects PowerUpEffects;
        private PlayerAttackManager PlayerAttackManager;
        private PlayerHealthManager PlayerHealthManager;

        private void Awake()
        {
            PowerUpEffects = GetComponent<PowerUpEffects>();
            PlayerAttackManager = GetComponent<PlayerAttackManager>();
            PlayerHealthManager = GetComponent<PlayerHealthManager>();
        }

        public void IncreasePlayerDamage()
        {
            PlayerAttackManager.CurrentDamage += PlayerAttackManager.CurrentDamage * DamageUpMultiplier;
        }

        public void IncreasePlayerMaxHP()
        {
            int maxHPIncreaseValue = (int) (PlayerHealthManager.MaxHealth * HPUpMultiplier);
            PlayerHealthManager.IncreaseMaxHP(maxHPIncreaseValue);

            // Heal according to the added value
            PlayerHealthManager.Heal(maxHPIncreaseValue);
        }

        public void HealPlayerHP()
        {
            PlayerHealthManager.Heal((int) (PlayerHealthManager.MaxHealth * HealPercentage));
        }

        public Effects GenerateEffect()
        {
            List<Effects> activatedEffects = new List<Effects>();

            if (FireLevel == 1)
            {
                // FIRE LV 1
                if (Random.Range(1, 100) < OddsFireLv1)
                {
                    activatedEffects.Add(Effects.Fire);
                    // FIRE LV 2 needs FIRE LV1 to happen to take effect
                }
            }

            if (IceLevel == 1)
            {
                // ICE LV 1
                if (Random.Range(1, 100) < OddsIceLv1)
                {
                    activatedEffects.Add(Effects.Ice);
                }
            }

            if (ElectricLevel == 1)
            {
                //  THUNDER LV 1
                if (Random.Range(1, 100) < OddsThunderLv1)
                {
                    activatedEffects.Add(Effects.Thunder);

                    // THUNDER LV 2 needs THUNDER LV 1 to happen to take effect
                }
            }

            int effectNumber = Random.Range(0, activatedEffects.Count);

            if (activatedEffects.Count > 0)
            {
                return activatedEffects[effectNumber];
            }

            return Effects.None;
        }

        public void ApplyEffectsOnEnemies(GameObject enemy, Effects activatedEffect)
        {
            EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
            EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

            // Even when the list is empty

            // ICE LV 2 doesn't needs ICE LV 1 to happen in the same attack
            if (IceLevel >= 2 && enemyStateMachine.IsFrozen)
            {
                //Debug.Log("SHATTERING ENEMY");
                PowerUpEffects.ShatterEnemy(enemy, ShatterDamage);
            }

            // Choosing between all the activated effects

            switch (activatedEffect)
            {
                case Effects.Fire:
                    //Debug.Log("FIRE CHOSEN");

                    PowerUpEffects.BurnEnemy(enemy, (FireLevel >= 2) ? FireDamageLv2 : FireDamageLv1);

                    if (FireLevel >= 2 && enemyStateMachine.IsOnFire && enemyStateMachine.WillDieBurned)
                    {
                        // FIRE LV 2
                        //Debug.Log("BURNING ENEMY TO THE DEATH");
                        PowerUpEffects.BurnEnemyToDeath(enemy);
                    }

                    break;

                case Effects.Ice:
                    //Debug.Log("ICE CHOSEN");
                    
                    if (!enemyStateMachine.IsParalyzed)
                    {
                        PowerUpEffects.FreezeEnemy(enemy, (IceLevel >= 2) ? DefrostTimeLv2 : DefrostTimeLv1);
                    }

                    break;

                case Effects.Thunder:
                    //Debug.Log("THUNDER CHOSEN");
                    PowerUpEffects.FindCloseEnemies(enemy, ElectricRange,
                        (ElectricLevel >= 2) ? ElectricDamageLv2 : ElectricDamageLv1); // AND DEALS DAMAGE TO THEM

                    // THUNDER LV 2
                    if (ElectricLevel >= 2)
                    {
                        if (enemyStateMachine.IsPrimaryTarget && Random.Range(1, 100) < OddsThunderLv2)
                        {
                            if (!enemyStateMachine.IsFrozen)
                            {
                                PowerUpEffects.ParalyzeEnemy(enemy, ParalyzeTime);
                                //Debug.Log("PARALIZADO");    
                            }
                        }
                    }
                    enemyStateMachine.IsPrimaryTarget = false;
                    break;
            }
        }
    }
}