using Enemy.Attacks;
using Player;
using UnityEngine;

namespace Objects
{
    public class ProjectileScript : MonoBehaviour
    {
        [SerializeField] private bool canBeStuck;
        [SerializeField] private bool rotating;
        [SerializeField] private bool hasDestructionAnimation; // Imply that the animation will destroy the object
        [SerializeField] [Range(0, 180)] private float rotationSpeed;
        [SerializeField] private BaseAttack.DamageType damageType;
        private Vector3 _shootDirection;
        private float _damage;
        private float _moveSpeed;
        private bool _isStuck;
        private bool _stoppedMoving; 
        public GameObject sprite;
        private Animator _spriteAnimator;
        
        public void Setup(Vector3 shootDirection, float projectileSpeed, float enemyPhysicalDamage, float enemyMagicalDamage)
        {
            _spriteAnimator = sprite.GetComponent<Animator>();
            this._shootDirection = shootDirection;
            this._moveSpeed = projectileSpeed;
            this._damage = damageType switch
            {
                BaseAttack.DamageType.Physical => enemyPhysicalDamage,
                BaseAttack.DamageType.Magical => enemyMagicalDamage,
                _ => 0
            };
            transform.right = shootDirection;
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            if ((canBeStuck && _isStuck) || _stoppedMoving) return;
            transform.position += _shootDirection * (_moveSpeed * Time.deltaTime);
            if(rotating) sprite.transform.Rotate(new Vector3(0, 0, rotationSpeed));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Hurt player
                other.gameObject.GetComponent<PlayerHealthManager>()?.TakeDamage(_damage, damageType);
                if (hasDestructionAnimation)
                {
                    _spriteAnimator.SetTrigger("Extinguish");
                    _stoppedMoving = true;
                }
                else 
                    Destroy(gameObject);
            }

            // if (other.gameObject.CompareTag("Enemy"))
            // {
            //     // Hurt enemy
            //     other.gameObject.GetComponent<EnemyCombatManager>()?.TakeDamage(Damage, ShootDirection, 25, false, false, true, Color.yellow); // arbitrary attack speed
            //     if(HasDestructionAnimation)
            //         SpriteAnimator.SetTrigger("Extinguish");
            //     else 
            //         Destroy(gameObject);
            // }

            if (other.gameObject.CompareTag("Scene objects"))
            {
                if (canBeStuck) _isStuck = true;
                else
                {
                    if (hasDestructionAnimation)
                    {
                        _spriteAnimator.SetTrigger("Extinguish");
                        _stoppedMoving = true; 
                    }
                    else 
                        Destroy(gameObject);
                }
            }
        }

        public void ReflectProjectile()
        {
            _shootDirection = -_shootDirection;
            _moveSpeed = _moveSpeed * 1.5f;
            transform.right = _shootDirection;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}
