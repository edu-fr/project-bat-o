using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpActivator : MonoBehaviour
{
    private PowerUpEffects PowerUpEffects;
    private PlayerStatsController PlayerStatsController;
    private PlayerAttackManager PlayerAttackManager;
    private PlayerHealthManager PlayerHealthManager;
    private PowerUpController PowerUpController;
    
    private void Awake()
    {
        PowerUpEffects = GetComponent<PowerUpEffects>();
        PlayerStatsController = GetComponent<PlayerStatsController>();
        PlayerAttackManager = GetComponent<PlayerAttackManager>();
        PlayerHealthManager = GetComponent<PlayerHealthManager>();
        PowerUpController = GetComponent<PowerUpController>();
    }

    public PowerUpController.Effects GenerateEffect()
    {
        List<PowerUpController.Effects> activatedEffects = new List<PowerUpController.Effects>();

        if (PowerUpController.FireLevel > 0)
        {
            // FIRE LV 1
            Random.InitState((int) DateTime.Now.Ticks);
            if (Random.Range(0, 100) < PlayerStatsController.FireAttackRate)
            {
                activatedEffects.Add(PowerUpController.Effects.Fire);
                // FIRE LV 2 needs FIRE LV1 to happen to take effect
            }
        }

        if (PowerUpController.IceLevel > 0)
        {
            // ICE LV 1
            Random.InitState((int) DateTime.Now.Ticks);
            if (Random.Range(0, 100) < PlayerStatsController.IceAttackRate)
            {
                activatedEffects.Add(PowerUpController.Effects.Ice);
            }
        }

        if (PowerUpController.ElectricLevel > 0)
        {
            //  THUNDER LV 1
            Random.InitState((int) DateTime.Now.Ticks);
            if (Random.Range(0, 100) < PlayerStatsController.ElectricAttackRate)
            {
                activatedEffects.Add(PowerUpController.Effects.Thunder);
                // THUNDER LV 2 needs THUNDER LV 1 to happen to take effect
            }
        }

        Random.InitState((int) DateTime.Now.Ticks);
        var effectNumber = Random.Range(0, activatedEffects.Count);

        return activatedEffects.Count > 0 ? activatedEffects[effectNumber] : PowerUpController.Effects.None;
    }

    public void ApplyEffectsOnEnemies(GameObject enemy, PowerUpController.Effects activatedEffect)
    {
        EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

        // Even when the list is empty

        // ICE LV 2 doesn't needs ICE LV 1 to happen in the same attack
        if (PowerUpController.IceLevel > 1 && enemyStateMachine.IsFrozen)
        {
            Debug.Log("SHATTERING ENEMY");
            PowerUpEffects.ShatterEnemy(enemy, PlayerStatsController.ShatterDamage);
        }

        // Choosing between all the activated effects

        switch (activatedEffect)
        {
            case PowerUpController.Effects.Fire:
                //Debug.Log("FIRE CHOSEN");

                PowerUpEffects.BurnEnemy(enemy, PlayerStatsController.FireDamage);

                if (PowerUpController.FireLevel > 1 && enemyStateMachine.IsOnFire && enemyStateMachine.WillDieBurned)
                {
                    // FIRE LV 2
                    // Debug.Log("BURNING ENEMY TO THE DEATH");
                    PowerUpEffects.BurnEnemyToDeath(enemy);
                }

                break;

            case PowerUpController.Effects.Ice:
                if (!enemyStateMachine.IsParalyzed)
                {
                    PowerUpEffects.FreezeEnemy(enemy, PlayerStatsController.FrostDuration);
                }
                break;

            case PowerUpController.Effects.Thunder:
                //Debug.Log("THUNDER CHOSEN");
                PowerUpEffects.FindCloseEnemies(enemy, PlayerStatsController.ElectricRange, PlayerStatsController.ElectricalDamage); // AND DEALS DAMAGE TO THEM

                // THUNDER LV 2
                if (PowerUpController.ElectricLevel >= 2)
                {
                    if (enemyStateMachine.IsPrimaryTarget && Random.Range(1, 100) < PlayerStatsController.ElectricAttackRate)
                    {
                        if (!enemyStateMachine.IsFrozen)
                        {
                            PowerUpEffects.ParalyzeEnemy(enemy, PlayerStatsController.ParalyzeDuration);
                            //Debug.Log("PARALIZADO");    
                        }
                    }
                }
                enemyStateMachine.IsPrimaryTarget = false;
                break;
        }
    }
}
