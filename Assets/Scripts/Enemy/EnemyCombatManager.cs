using System.Collections;
using Game;
using Pathfinding;
using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyCombatManager : MonoBehaviour
    {
        private EnemyHealthManager EnemyHealthManager;
        private AudioManager AudioManager;
        private EnemyBehavior EnemyBehavior;
        public Rigidbody2D Rigidbody2D;
        
        
        // Attack
        public float Damage = 20;
        private bool Invincible = false;
        public float TimeInvincible = .9f;
        private void Awake()
        {
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(float weaponDamage, float weaponKnockback, Vector3 attackDirection, float knockBackDuration, float weaponAttackSpeed)
        {
            if (Invincible) return; // if takes damage recently, dont take damage;

            Invincible = true;
            
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            
            FlashSprite();

            // Decrease health
            EnemyHealthManager.TakeDamage((int) weaponDamage);
           
            // Take knockback
            EnemyBehavior.AiPath.enabled = false;
            Rigidbody2D.AddForce(attackDirection * weaponKnockback, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockback(knockBackDuration));

            // Work on it
            float timeFlashing = TimeInvincible / (weaponAttackSpeed / 6f);
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
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)Damage);

        }

        private IEnumerator TakeKnockback(float knockbackTime)
        {
            yield return new WaitForSeconds(knockbackTime);
            EnemyBehavior.AiPath.enabled = true;
        }
    }
}
