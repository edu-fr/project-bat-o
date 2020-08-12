using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class PowerUpEffects : MonoBehaviour
{
    public List<int> BurnTickTimers = new List<int>();

    public void IncreasePlayerAttack()
    {
            
    }

    public void IncreasePlayerHp()
    {
            
    }

    public void BurnEnemy(GameObject enemy, int fireDamage)
    {
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();

        enemyBehavior.WillDieBurned = enemyHealthManager.CurrentHealth < fireDamage ? true : false;

        if (BurnTickTimers.Count <= 0)
        {
              BurnTickTimers.Add(10);
              StartCoroutine(ApplyBurn(enemy, fireDamage));
        }
        else
        {
            BurnTickTimers.Add(10);
        }
    }
    
    public IEnumerator ApplyBurn(GameObject enemy, int fireDamage)
    {
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        
        
        while (BurnTickTimers.Count > 0)
        {
            enemyBehavior.IsOnFire = true;
            for (int i = 0; i < BurnTickTimers.Count; i++)
            {
                BurnTickTimers[i]--;
            }
            enemyHealthManager.TakeDamage(fireDamage/4);
            BurnTickTimers.RemoveAll(i => i == 0);
            yield return new WaitForSeconds(0.5f);
        }
        enemyBehavior.IsOnFire = false;
    }
    
   
        
    public void FreezeEnemy(GameObject enemy, float defrostTime)
    {
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>(); 
        enemyBehavior.DefrostCurrentTimer = 0f;
        enemyBehavior.DefrostTime = defrostTime;
        enemyBehavior.ChangeState(EnemyBehavior.States.Frozen);
    }
        
    public void FindCloseEnemies(GameObject enemy, float electricRange, int electricDamage)
    {
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        enemyBehavior.IsPrimaryTarget = true;
        float closestEnemyDistance = 1000f;
        Collider2D closestEnemy = null; 
        
        // Search all enemies in the electric attack range
        Collider2D[] enemiesNearby = Physics2D.OverlapCircleAll(transform.position, electricRange, LayerMask.GetMask("Enemies"));
        foreach (Collider2D enemyNearby in enemiesNearby)
        {
            if (enemyNearby.gameObject.CompareTag("Enemy") && (enemy.GetInstanceID() != enemyNearby.gameObject.GetInstanceID()))
            {
                if(enemyBehavior.CircleCollider.Distance(enemyNearby).distance < closestEnemyDistance)
                {
                    closestEnemyDistance = enemyBehavior.CircleCollider.Distance(enemyNearby).distance;
                    closestEnemy = enemyNearby;
                }
            }
        }

        if (closestEnemy != null) // found an close enemy
        {
            // Deals electric damage to closest enemy
            DealsElectricDamage(closestEnemy.gameObject, electricDamage);
            
            // Search the closest enemy (except the first enemy hit) and deals damage it too
            Collider2D closestEnemyToTheClosestEnemy = null;
            float closestEnemyDistanceToTheClosestEnemy = 1000f;
            Collider2D[] enemiesNearbyToTheClosestEnemy = Physics2D.OverlapCircleAll(closestEnemy.gameObject.transform.position, electricRange, LayerMask.GetMask("Enemies"));
            foreach (Collider2D enemyNearbyFromTheClosestEnemy in enemiesNearbyToTheClosestEnemy)
            {
                if (enemyNearbyFromTheClosestEnemy.gameObject.CompareTag("Enemy") && (enemyNearbyFromTheClosestEnemy.gameObject.GetInstanceID() != enemy.GetInstanceID()) && (enemyNearbyFromTheClosestEnemy.gameObject.GetInstanceID() != closestEnemy.gameObject.GetInstanceID()))
                {
                    if(closestEnemy.gameObject.GetComponent<CircleCollider2D>().Distance(enemyNearbyFromTheClosestEnemy).distance < closestEnemyDistanceToTheClosestEnemy)
                    {
                        closestEnemyDistanceToTheClosestEnemy = enemyNearbyFromTheClosestEnemy.GetComponent<CircleCollider2D>().Distance(closestEnemy.GetComponent<CircleCollider2D>()).distance;
                        closestEnemyToTheClosestEnemy = enemyNearbyFromTheClosestEnemy;
                    }
                }
            }

            if (closestEnemyToTheClosestEnemy != null)
            {
                DealsElectricDamage(closestEnemyToTheClosestEnemy.gameObject, electricDamage);
            }
        }
        
        // deals electric damage to the first enemy hit
        DealsElectricDamage(enemy, electricDamage);
    }

    public void DealsElectricDamage(GameObject enemy, int electricDamage)
    {
        enemy.GetComponent<EnemyHealthManager>().TakeDamage(electricDamage);
    }
    
    public void BurnEnemyToDeath(GameObject enemy)
    {
        enemy.GetComponent<EnemyBehavior>().ChangeState(EnemyBehavior.States.DyingBurned);
    }

    public void ShatterEnemy(GameObject enemy, int shatterDamage)
    {
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        enemyBehavior.DefrostCurrentTimer = enemyBehavior.DefrostTime;
        enemyHealthManager.TakeDamage(shatterDamage);
    }

    public void ParalyzeEnemy(GameObject enemy, float paralyzeTime)
    {
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        enemyBehavior.ParalyzeHealCurrentTimer = 0;
        enemyBehavior.ParalyzeHealTime = paralyzeTime;
        enemyBehavior.ChangeState(EnemyBehavior.States.Paralyzed);
    }
}
