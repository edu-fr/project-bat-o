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
        private EnemyBehavior EnemyBehavior;
        public Rigidbody2D Rigidbody2D;
        
        public Transform PrefabDamagePopup;
        
        // Attack
        public float MeleeDamage { private set; get; } = 20;
        public float RangedDamage { private set; get; } = 15f;
        public bool IsAttacking;
        public bool Invincible { get; private set; } = false;
        public float TimeInvincible = 2f;
        private void Awake()
        {
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public float TakeDamage(float damage, Vector3 attackDirection, float attackSpeed, bool isDot, bool isCriticalHit, bool showValue, Color? customColor)
        {
            if (Invincible)
            {
                return 0;
            }

            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            Invincible = true;
            var knockBack = damage / 20; // arbitrary value
            var knockBackDuration = knockBack / 17; // arbitrary value
            
            FlashSprite();

            // CALCULATE DEFENSES, ETC
            EnemyHealthManager.TakeDamage((int) damage);
           
            // Take knockback
            EnemyBehavior.AiPath.enabled = false;
            Rigidbody2D.AddForce(attackDirection * knockBack, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockBack(knockBackDuration));

            // Work on it
            float timeFlashing = TimeInvincible / (attackSpeed / 6f);
            Invoke(nameof(EndFlash), timeFlashing / 3);
            Invoke(nameof(FlashSprite), timeFlashing / 2);
            Invoke(nameof(EndFlash), timeFlashing);
            Invoke(nameof(EndInvincibility), timeFlashing - 0.05f);

            if (showValue)
            {
                DamagePopup.Create(transform.position, (int) damage, attackDirection, PrefabDamagePopup, isCriticalHit, isDot, customColor);
            }
            
            return damage;
        }

        private void FlashSprite()
        {
            if(EnemyBehavior)
                EnemyBehavior.Renderer.material = EnemyBehavior.FlashMaterial;
        }

        private void EndFlash()
        {
            if(EnemyBehavior)
                EnemyBehavior.Renderer.material = EnemyBehavior.CurrentMaterial;
        }
        private void EndInvincibility ()
        {
            if(EnemyBehavior)
                Invincible = false;
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            // Saves the last time enemy hit the player with an attack
            if (EnemyBehavior.EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee)
            {
                other.gameObject.GetComponent<PlayerController>().DodgeFailed = true;
            }
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)MeleeDamage);
        }

        private IEnumerator TakeKnockBack(float knockBackTime)
        {
            yield return new WaitForSeconds(knockBackTime);
            EnemyBehavior.AiPath.enabled = true;
        }
    }
}
