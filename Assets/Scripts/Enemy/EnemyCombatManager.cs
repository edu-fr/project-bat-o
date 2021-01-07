using System.Collections;
using Game;
using Pathfinding;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Enemy
{
    public class EnemyCombatManager : MonoBehaviour
    {
        private EnemyHealthManager EnemyHealthManager;
        private AudioManager AudioManager;
        private EnemyBehavior EnemyBehavior;
        public Rigidbody2D Rigidbody2D;
        public float LastTimeHitPlayerDuringAttack;
        
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
            StartCoroutine(TakeKnockBack(knockBackDuration));

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
            
            // Saves the last time enemy hit the player with an attack
            if (EnemyBehavior.EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee)
            {
                LastTimeHitPlayerDuringAttack = Time.time;
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
