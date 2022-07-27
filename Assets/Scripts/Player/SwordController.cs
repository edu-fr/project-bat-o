using Objects;
using UnityEngine;

namespace Player
{
    public class SwordController : MonoBehaviour
    {
        private PlayerAttackManager PlayerAttackManager;
        private BoxCollider2D BoxCollider2D;
    
        void Awake()
        {
            BoxCollider2D = GetComponent<BoxCollider2D>();
            PlayerAttackManager = GetComponentInParent<PlayerAttackManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                PlayerAttackManager.VerifyAttackCollision(other.gameObject);
            }
        }
    }
}
