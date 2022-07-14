using System.Collections;
using Resources.Project.Runtime.Scripts.Enemy.Attacks;
using Resources.Project.Runtime.Scripts.Game;
using Resources.Project.Runtime.Scripts.Player;
using Resources.Project.Runtime.Scripts.UI;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Enemy
{
    public class EnemyCombatManager : MonoBehaviour
    {
        [SerializeField] private EnemyHealthManager EnemyHealthManager;
        [SerializeField] private EnemyMovementHandler EnemyMovementHandler;
        [SerializeField] private EnemyStatsManager EnemyStatsManager;
        public BoxCollider2D BoxCollider2D;
        public Rigidbody2D Rigidbody2D; 
        
        public Transform PrefabDamagePopup;

        public bool IsAttacking;

        public float TakeDamage(float damage, Vector3 attackDirection, float attackSpeed, bool isDot, bool isCriticalHit, bool showValue, Color? customColor)
        {
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            var finalDamage = damage - EnemyStatsManager.PhysicalDefense; // arbitrary value
            var knockBack = 100 / EnemyStatsManager.Weight; // arbitrary value
            var knockBackDuration = 0.7f; // arbitrary value
            // Knockback
            if (knockBack > 5)
            {
                Rigidbody2D.AddForce(attackDirection * knockBack, ForceMode2D.Impulse);
                StartCoroutine(TakeKnockBack(knockBackDuration));
            }
            // CALCULATE DEFENSES, ETC
            if (showValue)
                DamagePopup.Create(transform.position, (int) damage, attackDirection, PrefabDamagePopup, isCriticalHit, isDot, customColor);
            
            EnemyMovementHandler.AiPath.enabled = false;
            EnemyHealthManager.TakeDamage((int) damage);
            
            return damage;
        }
        
        private void OnCollisionStay2D(Collision2D other) // I will maintain that? 
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)EnemyStatsManager.PhysicalDamage, BaseAttack.DamageType.Physical);
        }

        private IEnumerator TakeKnockBack(float knockBackTime)
        {
            EnemyMovementHandler.EnemyStateMachine.ChangeState(EnemyStateMachine.States.TakingKnockBack);
            yield return new WaitForSeconds(knockBackTime);
            EnemyMovementHandler.EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
        }
    }
}
