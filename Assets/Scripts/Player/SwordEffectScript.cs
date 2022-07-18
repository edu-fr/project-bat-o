using UnityEngine;

namespace Player
{
    public class SwordEffectScript : MonoBehaviour
    {
        private BoxCollider2D SwordHitbox;

        void Awake()
        {
            SwordHitbox = GetComponentInParent<BoxCollider2D>();
        }

        void Update()
        {
            var position = transform.position;
            var hitboxOffset = SwordHitbox.offset;
            gameObject.transform.position = new Vector2(SwordHitbox.transform.position.x + hitboxOffset.x  /* + SwordHitbox.size.x/2 */ , SwordHitbox.transform.position.y + hitboxOffset.y /* + SwordHitbox.size.y/2 */ );
            // position = new Vector2( );
        }

        private void OnDrawGizmos()
        {
            // Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}
