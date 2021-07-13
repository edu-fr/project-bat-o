using System;
using Player;
using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class ProjectileScript : MonoBehaviour
    {
        [SerializeField] private float Damage;
        [SerializeField] private bool CanBeStuck;
        [SerializeField] private bool Rotating;
        [SerializeField] private bool HasDestructionAnimation; // Imply that the animation will destroy the object
        [SerializeField] [Range(0.01f, 10)] private float MoveSpeed;
        [SerializeField] [Range(0, 180)] private float RotationSpeed;
        private Vector3 ShootDirection;
        private bool IsStuck;
        public GameObject Sprite;
        private Animator SpriteAnimator;
        
        public void Setup(Vector3 shootDirection, float projectileSpeed)
        {
            SpriteAnimator = Sprite.GetComponent<Animator>();
            this.ShootDirection = shootDirection;
            this.MoveSpeed = projectileSpeed;
            transform.right = shootDirection;
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            if (CanBeStuck && IsStuck) return; 
            transform.position += ShootDirection * (MoveSpeed * Time.deltaTime);
            if(Rotating) Sprite.transform.Rotate(new Vector3(0, 0, RotationSpeed));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Hurt player
                other.gameObject.GetComponent<PlayerHealthManager>()?.TakeDamage(Damage);
                if(HasDestructionAnimation)
                    SpriteAnimator.SetTrigger("Extinguish");
                else 
                    Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                // Hurt enemy
                other.gameObject.GetComponent<EnemyCombatManager>()?.TakeDamage(Damage, ShootDirection, 25, false, false, true, Color.yellow); // arbitrary attack speed
                if(HasDestructionAnimation)
                    SpriteAnimator.SetTrigger("Extinguish");
                else 
                    Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Scene objects"))
            {
                if (CanBeStuck) IsStuck = true;
                else
                {
                    if(HasDestructionAnimation)
                        SpriteAnimator.SetTrigger("Extinguish");
                    else 
                        Destroy(gameObject);
                }
            }
        }

        public void ReflectProjectile()
        {
            ShootDirection = -ShootDirection;
            MoveSpeed = MoveSpeed * 1.5f;
            transform.right = ShootDirection;
        }
    }
}
