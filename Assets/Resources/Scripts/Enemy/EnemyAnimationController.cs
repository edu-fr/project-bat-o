using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public enum SpriteType {
            FourDir,
            FourDirMirror,
        }
    
        public Animator Animator { get; private set; }
        private Vector3 CurrentDirection;
        private float CurAngle;
        public Vector3 FaceDirection;
    
    
    }
}
