using System.Collections;
using System.Collections.Generic;
using Resources.Scripts.Enemy;
using UnityEngine;

public class PowerUpEffects : MonoBehaviour
{
    public List<int> BurnTickTimers = new List<int>();

    public void BurnEnemy(GameObject enemy, float fireDamage)
    {
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();

        enemyStateMachine.WillDieBurned = enemyHealthManager.CurrentHealth < fireDamage;

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
    
    public IEnumerator ApplyBurn(GameObject enemy, float fireDamage)
    {
        EnemyCombatManager enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();
        EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();

        while (BurnTickTimers.Count > 0 && enemyHealthManager.CurrentHealth > 0)
        {
            enemyStateMachine.IsOnFire = true;
            for (int i = 0; i < BurnTickTimers.Count; i++)
            {
                BurnTickTimers[i]--;
            }
            enemyCombatManager.TakeDamage(fireDamage/4, Vector3.zero, 40, true, false, true, Color.magenta);
            BurnTickTimers.RemoveAll(i => i == 0);
            yield return new WaitForSeconds(0.5f);
        }
        if(enemyStateMachine) enemyStateMachine.IsOnFire = false;
        BurnTickTimers.Clear();
    }
    
   
        
    public void FreezeEnemy(GameObject enemy, float defrostTime)
    {
        var enemyStateMachine = enemy.GetComponent<EnemyStateMachine>(); 
        
        enemyStateMachine.DefrostCurrentTimer = 0f;
        enemyStateMachine.DefrostTime = defrostTime;
        enemyStateMachine.ChangeState(EnemyStateMachine.States.Frozen);
        // Debug.Log("PowerUp Frozen");
    }
        
    public void FindCloseEnemies(GameObject enemy, float electricRange, float electricDamage)
    {
        var enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        enemyStateMachine.IsPrimaryTarget = true;
        var enemyBehavior = enemy.GetComponent<EnemyMovementHandler>();

        var closestEnemyDistance = 1000f;
        Collider2D closestEnemy = null; 
        
        // Search all enemies in the electric attack range
        var  enemiesNearby = Physics2D.OverlapCircleAll(transform.position, electricRange, LayerMask.GetMask("Enemies"));
        foreach (var enemyNearby in enemiesNearby)
        {
            if (enemyNearby.gameObject.CompareTag("Enemy") && (enemy.GetInstanceID() != enemyNearby.gameObject.GetInstanceID()))
            {
                if(enemyBehavior.BoxCollider2D.Distance(enemyNearby).distance < closestEnemyDistance)
                {
                    closestEnemyDistance = enemyBehavior.BoxCollider2D.Distance(enemyNearby).distance;
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
            var closestEnemyDistanceToTheClosestEnemy = 1000f;
            var enemiesNearbyToTheClosestEnemy = Physics2D.OverlapCircleAll(closestEnemy.gameObject.transform.position, electricRange, LayerMask.GetMask("Enemies"));
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

    public void DealsElectricDamage(GameObject enemy, float electricDamage)
    {
        enemy.GetComponent<EnemyCombatManager>().TakeDamage(electricDamage, Vector3.zero, 40, false, false, true, Color.yellow);
    }
    
    public void BurnEnemyToDeath(GameObject enemy)
    {
        if(enemy)
            enemy.GetComponent<EnemyStateMachine>().ChangeState(EnemyStateMachine.States.DyingBurned);
    }

    public void ShatterEnemy(GameObject enemy, float shatterDamage)
    {
        var enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        var enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();
        enemyStateMachine.DefrostCurrentTimer = enemyStateMachine.DefrostTime;
        enemyCombatManager.TakeDamage(shatterDamage, Vector3.down, 40, false, true, true, Color.blue);
    }

    public void ParalyzeEnemy(GameObject enemy, float paralyzeTime)
    {
        var enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        enemyStateMachine.ParalyzeHealCurrentTimer = 0;
        enemyStateMachine.ParalyzeHealTime = paralyzeTime;
        enemyStateMachine.ChangeState(EnemyStateMachine.States.Paralyzed);
        // Debug.Log("PowerUp Paralyzed");
    }
}
