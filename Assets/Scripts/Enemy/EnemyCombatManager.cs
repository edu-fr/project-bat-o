using System.Collections;
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

        private void Awake()
        {
            _enemyHealthManager = GetComponent<EnemyHealthManager>();
            _enemyMovementHandler = GetComponent<EnemyMovementHandler>();
            _enemyStatsManager = GetComponent<EnemyStatsManager>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        [Header("Damage popup")]
        public Transform prefabDamagePopup;

        public bool IsAttacking { get; set; }

        public float TakeDamage(float damage, Vector3 attackDirection, float attackSpeed, bool isDot, bool isCriticalHit, bool showValue, Color? customColor)
        {
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
            
            return damage;
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
    }
}
