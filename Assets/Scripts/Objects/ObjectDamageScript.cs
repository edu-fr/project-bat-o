using Enemy;
using Enemy.Attacks;
using Player;
using UnityEngine;

namespace Objects
{
    public class ObjectDamageScript : MonoBehaviour
    {
        public Collider2D objectCollider2D;
        public Transform prefabDamagePopup;
        [SerializeField]
        private float objectDamage;

        [SerializeField] private float objectAttackSpeed; // the bigger, the less time the attacker is invulnerable
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var damageTakenByEnemy = other.gameObject.GetComponent<EnemyCombatManager>().TakeDamage(objectDamage,
                    other.transform.position - transform.position, 25, false, false, true, Color.green);
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage(objectDamage);
            }
        }
    }
}
