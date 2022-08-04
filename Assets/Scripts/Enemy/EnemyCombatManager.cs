using System.Collections;
using System.Collections.Generic;
using Enemy.Attacks;
using Game;
using Player;
using UI;
using UnityEngine;

namespace Enemy
{
    public class EnemyCombatManager : MonoBehaviour
    {
        private EnemyHealthManager _enemyHealthManager;
        private EnemyMovementHandler _enemyMovementHandler;
        private EnemyStatsManager _enemyStatsManager;
        [HideInInspector] public BoxCollider2D boxCollider2D; 
        [HideInInspector] public Rigidbody2D rigidbody2D;
        public List<int> attacksImmuneTo;  

        private void Awake()
        {
            _enemyHealthManager = GetComponent<EnemyHealthManager>();
            _enemyMovementHandler = GetComponent<EnemyMovementHandler>();
            _enemyStatsManager = GetComponent<EnemyStatsManager>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            attacksImmuneTo = new List<int>();
        }
        
        [Header("Damage popup")]
        public Transform prefabDamagePopup;

        public bool IsAttacking { get; set; }

        public void TakeDamage(int damageId, float damage, Vector3 attackDirection, bool isDot, bool isCriticalHit, bool showValue, Color? customColor)
        {
            print($"TAKE DAMAGE => Enemy: {name} | DamageID: {damageId} | Damage: {damage}");
            if (attacksImmuneTo.Contains(damageId))
            {
                print("The enemy " + name + " is immune to the attack " + damageId);
                return;
            }
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            var finalDamage = damage - _enemyStatsManager.CurrentResistance; // arbitrary value
            var knockBackForce = _enemyStatsManager.Lightness; // arbitrary value
            var knockBackDuration = _enemyStatsManager.Lightness / 8.5f; // arbitrary value
       
            rigidbody2D.AddForce(attackDirection * knockBackForce, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockBack(knockBackDuration));
            
            // CALCULATE DEFENSES, ETC
            if (showValue)
                DamagePopup.Create(transform.position, (int) damage, attackDirection, prefabDamagePopup, isCriticalHit, isDot, customColor);
            
            _enemyMovementHandler.aiPath.enabled = false;
            _enemyHealthManager.TakeDamage((int) damage);
            StartCoroutine(ImmunityFrames(damageId));
            if (_enemyHealthManager.currentHealth <= 0) StopAllCoroutines();
        }
        
        private void OnCollisionStay2D(Collision2D other) // I will maintain that? 
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)_enemyStatsManager.CurrentPower);
        }

        private IEnumerator TakeKnockBack(float knockBackTime)
        {
            if (_enemyStatsManager.AttackIsStoppedByPlayer)
                _enemyMovementHandler.enemyStateMachine.ChangeState(EnemyStateMachine.States.TakingKnockBack);
            
            yield return new WaitForSeconds(knockBackTime);
    
            if (_enemyStatsManager.AttackIsStoppedByPlayer)
                _enemyMovementHandler.enemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
        }

        private IEnumerator ImmunityFrames(int id)
        {
            attacksImmuneTo.Add(id);
            yield return new WaitForSeconds(_enemyStatsManager.immunitySeconds);
            attacksImmuneTo.Remove(id);
        }

        public void StartBurning(float totalDamage, float ticks, float interval, float criticalRate, float criticalDamage)
        {
            // Stop previous burnings
            StopAllCoroutines();
            StartCoroutine(BurningRoutine(totalDamage, ticks, interval, criticalRate, criticalDamage));;
        }
        
        private IEnumerator BurningRoutine(float totalDamage, float ticks, float interval, float criticalRate, float criticalDamage)
        {
            for (var i = 0; i < ticks; i++)
            {
                BurnTick(totalDamage/ticks, criticalRate, criticalDamage);
                yield return new WaitForSeconds(interval);
            }
        }

        private void BurnTick(float damage, float criticalChance, float criticalDamage)
        {
            var criticalTest = Random.Range(0, 100);
            var burnId = Random.Range(1, 10000);
            print("Ticking! Damage: " + damage + " Chance: " + criticalChance + " Damage: " + criticalDamage);
            TakeDamage(burnId, damage * (criticalTest < criticalChance ? criticalDamage / 100 : 1), Vector3.zero,
                true, criticalTest < criticalChance, true, null);
        }
    }
}
