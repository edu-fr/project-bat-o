using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Player
{
    public class PowerUpEffects : MonoBehaviour
    {
        // public List<int> BurnTickTimers = new List<int>();
        //
        // public void BurnEnemy(GameObject enemy, float fireDamage)
        // {
        //     EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        //     EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        //     
        //     if (BurnTickTimers.Count <= 0)
        //     {
        //         BurnTickTimers.Add(5);
        //         StartCoroutine(ApplyBurn(enemy, fireDamage));
        //     }
        //     else
        //     {
        //         BurnTickTimers.Add(5);
        //     }
        // }
        //
        // public IEnumerator ApplyBurn(GameObject enemy, float fireDamage)
        // {
        //     EnemyCombatManager enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();
        //     EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
        //     EnemyHealthManager enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        //
        //     while (BurnTickTimers.Count > 0 && enemyHealthManager.currentHealth > 0)
        //     {
        //         enemyStateMachine.IsOnFire = true;
        //         for (int i = 0; i < BurnTickTimers.Count; i++)
        //         {
        //             BurnTickTimers[i]--;
        //         }
        //         enemyCombatManager.TakeDamage(fireDamage/4, Vector3.zero, 40, true, false, true, Color.magenta);
        //         BurnTickTimers.RemoveAll(i => i == 0);
        //         yield return new WaitForSeconds(0.5f);
        //     }
        //     if(enemyStateMachine) enemyStateMachine._isOnFire = false;
        //     BurnTickTimers.Clear();
        // }
    }
}
