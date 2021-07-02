using System.Collections;
using Game;
using Pathfinding;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyCombatManager : MonoBehaviour
    {
        private EnemyHealthManager EnemyHealthManager;
        private AudioManager AudioManager;
        private EnemyMovementHandler EnemyMovementHandler;
        public Rigidbody2D Rigidbody2D;
        
        public Transform PrefabDamagePopup;
        
        // Attack
        public float MeleeDamage { private set; get; } = 20;
        public float RangedDamage { private set; get; } = 15f;
        public bool IsAttacking;

        private void Awake()
        {
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemyMovementHandler = GetComponent<EnemyMovementHandler>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public float TakeDamage(float damage, Vector3 attackDirection, float attackSpeed, bool isDot, bool isCriticalHit, bool showValue, Color? customColor)
        {
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            var knockBack = damage / 20; // arbitrary value
            var knockBackDuration = knockBack / 17; // arbitrary value
            
            // Knockback
            Rigidbody2D.AddForce(attackDirection * knockBack, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockBack(knockBackDuration));

            // CALCULATE DEFENSES, ETC
            if (showValue)
                DamagePopup.Create(transform.position, (int) damage, attackDirection, PrefabDamagePopup, isCriticalHit, isDot, customColor);
            
            EnemyMovementHandler.AiPath.enabled = false;

            EnemyHealthManager.TakeDamage((int) damage);
            
            return damage;
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            // Saves the last time enemy hit the player with an attack
            if (EnemyMovementHandler.EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee)
            {
                other.gameObject.GetComponent<PlayerController>().DodgeFailed = true;
            }
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)MeleeDamage);
        }

        private IEnumerator TakeKnockBack(float knockBackTime)
        {
            yield return new WaitForSeconds(knockBackTime);
            EnemyMovementHandler.AiPath.enabled = true;
        }
    }
}
