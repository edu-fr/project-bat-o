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
        
        // Attack
        public float MeleeDamage { private set; get; } = 20;
        public float RangedDamage { private set; get; } = 200f;
        public bool IsAttacking;
        private bool Invincible = false;
        public float TimeInvincible = .9f;
        private void Awake()
        {
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(float damage, float knockback, Vector3 attackDirection, float knockBackDuration, float attackSpeed)
        {
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            
            FlashSprite();

            // Decrease health
            EnemyHealthManager.TakeDamage((int) damage);
           
            // Take knockback
            EnemyBehavior.AiPath.enabled = false;
            Rigidbody2D.AddForce(attackDirection * knockback, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockBack(knockBackDuration));

            // Work on it
            float timeFlashing = TimeInvincible / (attackSpeed / 6f);
            Invoke(nameof(EndFlash), timeFlashing / 3);
            Invoke(nameof(FlashSprite), timeFlashing / 2);
            Invoke(nameof(EndFlash), timeFlashing);
            Invoke(nameof(EndInvincibility), timeFlashing - 0.05f);
        }

        private void FlashSprite()
        {
            EnemyBehavior.Renderer.material = EnemyBehavior.FlashMaterial;
        }

        private void EndFlash()
        {
            EnemyBehavior.Renderer.material = EnemyBehavior.CurrentMaterial;
        }
        private void EndInvincibility ()
        {
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
