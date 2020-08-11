using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class PowerUpEffects : MonoBehaviour
{
    public List<int> BurnTickTimers = new List<int>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
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
        
        if (enemyHealthManager.CurrentHealth < fireDamage)
        {
            enemyBehavior.WillDieBurned = true;
            Debug.Log("WILL DIE BURNED");
        }
        else
        {
            enemyBehavior.WillDieBurned = false;
            Debug.Log("WONT");
        }
        
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
    
   
        
    public void FreezeEnemy(GameObject enemy)
    {
            
    }
        
    public void ElectrifyEnemy(GameObject enemy)
    {
            
    }

    public void BurnEnemyToDeath(GameObject enemy)
    {
        enemy.GetComponent<EnemyBehavior>().ChangeState(EnemyBehavior.States.DyingBurned);
        Debug.Log("Mudei pra Dying burned");
    }

    public void ShatterEnemy(GameObject enemy)
    {
            
    }

    public void ParalyzeEnemy(GameObject enemy)
    {
            
    }
}
