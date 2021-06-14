using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
public class PowerUpActivator : MonoBehaviour
{
    private PowerUpEffects PowerUpEffects;
    private PlayerAttackManager PlayerAttackManager;
    private PlayerHealthManager PlayerHealthManager;
    private PowerUpController PowerUpController;
    
    private void Awake()
    {
        PowerUpEffects = GetComponent<PowerUpEffects>();
        PlayerAttackManager = GetComponent<PlayerAttackManager>();
        PlayerHealthManager = GetComponent<PlayerHealthManager>();
        PowerUpController = GetComponent<PowerUpController>();
    }
    
   public void IncreasePlayerDamage()
    {
        PlayerAttackManager.CurrentDamage += PlayerAttackManager.CurrentDamage * PowerUpController.DamageUpMultiplier;
    }

    public void IncreasePlayerMaxHP()
    {
        int maxHPIncreaseValue = (int) (PlayerHealthManager.MaxHealth * PowerUpController.HpUpMultiplier);
        PlayerHealthManager.IncreaseMaxHP(maxHPIncreaseValue);

        // Heal according to the added value
        PlayerHealthManager.Heal(maxHPIncreaseValue);
    }

    public void HealPlayerHP()
    {
        PlayerHealthManager.Heal((int) (PlayerHealthManager.MaxHealth * PowerUpController.HealPercentage));
    }

    public PowerUpController.Effects GenerateEffect()
    {
        List<PowerUpController.Effects> activatedEffects = new List<PowerUpController.Effects>();

        if (PowerUpController.FireLevel == 1)
        {
            // FIRE LV 1
            if (Random.Range(1, 100) < PowerUpController.OddsFireLv1)
            {
                activatedEffects.Add(PowerUpController.Effects.Fire);
                // FIRE LV 2 needs FIRE LV1 to happen to take effect
            }
        }

        if (PowerUpController.IceLevel == 1)
        {
            // ICE LV 1
            if (Random.Range(1, 100) < PowerUpController.OddsIceLv1)
            {
                activatedEffects.Add(PowerUpController.Effects.Ice);
            }
        }

        if (PowerUpController.ElectricLevel == 1)
        {
            //  THUNDER LV 1
            if (Random.Range(1, 100) < PowerUpController.OddsThunderLv1)
            {
                activatedEffects.Add(PowerUpController.Effects.Thunder);

                // THUNDER LV 2 needs THUNDER LV 1 to happen to take effect
            }
        }

        int effectNumber = Random.Range(0, activatedEffects.Count);

        if (activatedEffects.Count > 0)
        {
            return activatedEffects[effectNumber];
        }

        return PowerUpController.Effects.None;
    }

    public void ApplyEffectsOnEnemies(GameObject enemy, PowerUpController.Effects activatedEffect)
    {
        EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

        // Even when the list is empty

        // ICE LV 2 doesn't needs ICE LV 1 to happen in the same attack
        if (PowerUpController.IceLevel >= 2 && enemyStateMachine.IsFrozen)
        {
            //Debug.Log("SHATTERING ENEMY");
            PowerUpEffects.ShatterEnemy(enemy, PowerUpController.ShatterDamage);
        }

        // Choosing between all the activated effects

        switch (activatedEffect)
        {
            case PowerUpController.Effects.Fire:
                //Debug.Log("FIRE CHOSEN");

                PowerUpEffects.BurnEnemy(enemy, (PowerUpController.FireLevel >= 2) ? PowerUpController.FireDamageLv2 : PowerUpController.FireDamageLv1);

                if (PowerUpController.FireLevel >= 2 && enemyStateMachine.IsOnFire && enemyStateMachine.WillDieBurned)
                {
                    // FIRE LV 2
                    //Debug.Log("BURNING ENEMY TO THE DEATH");
                    PowerUpEffects.BurnEnemyToDeath(enemy);
                }

                break;

            case PowerUpController.Effects.Ice:
                //Debug.Log("ICE CHOSEN");
                
                if (!enemyStateMachine.IsParalyzed)
                {
                    PowerUpEffects.FreezeEnemy(enemy, (PowerUpController.IceLevel >= 2) ? PowerUpController.DefrostTimeLv2 : PowerUpController.DefrostTimeLv1);
                }

                break;

            case PowerUpController.Effects.Thunder:
                //Debug.Log("THUNDER CHOSEN");
                PowerUpEffects.FindCloseEnemies(enemy, PowerUpController.ElectricRange,
                    (PowerUpController.ElectricLevel >= 2) ? PowerUpController.ElectricDamageLv2 : PowerUpController.ElectricDamageLv1); // AND DEALS DAMAGE TO THEM

                // THUNDER LV 2
                if (PowerUpController.ElectricLevel >= 2)
                {
                    if (enemyStateMachine.IsPrimaryTarget && Random.Range(1, 100) < PowerUpController.OddsThunderLv2)
                    {
                        if (!enemyStateMachine.IsFrozen)
                        {
                            PowerUpEffects.ParalyzeEnemy(enemy, PowerUpController.ParalyzeTime);
                            //Debug.Log("PARALIZADO");    
                        }
                    }
                }
                enemyStateMachine.IsPrimaryTarget = false;
                break;
        }
    }
}
