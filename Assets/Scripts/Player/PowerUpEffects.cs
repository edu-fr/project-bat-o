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
              BurnTickTimers.Add(4);
              StartCoroutine(ApplyBurn(enemy, fireDamage));
        }
        else
        {
            BurnTickTimers.Add(4);
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
        
    public void ElectrifyEnemy(GameObject enemy)
    {
            
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

    public void ParalyzeEnemy(GameObject enemy)
    {
            
    }
}
