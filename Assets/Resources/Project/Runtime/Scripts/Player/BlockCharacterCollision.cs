using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Player
{
    public class BlockCharacterCollision : MonoBehaviour
    {
        [SerializeField] 
        private Collider2D ParentCollider;
        [SerializeField] 
        private Collider2D ProtectorCollider;
        void Awake()
        {
            Physics2D.IgnoreCollision(ParentCollider, ProtectorCollider, true);
        }
    }
}
