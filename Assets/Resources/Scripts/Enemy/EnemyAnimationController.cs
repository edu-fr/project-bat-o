using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public enum SpriteType {
            FourDir,
            FourDirMirror,
        }

        public enum FaceDirection
        {
            FrontRight, 
            FrontLeft,
            BackRight,
            BackLeft
        }

        private Animator Animator;
        private SpriteRenderer SpriteRenderer;
        private Vector3 CurrentDirection;
        private float CurAngle;
        public FaceDirection CurrentFaceDirection { get; private set; }
        public float LastX;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AnimateMovement(float moveDirX, float moveDirY)
        {
            if (moveDirX != 0)
            {
                LastX = moveDirX;
            }
            Animator.SetBool("IsMoving", true);
            Animator.SetFloat("MoveX", moveDirX);
            Animator.SetFloat("MoveY", moveDirY);
            if (moveDirY > 0) // Going up (sprite back)
            {
                SpriteRenderer.flipX = moveDirX < 0f;
                CurrentFaceDirection = SpriteRenderer.flipX ? FaceDirection.BackLeft : FaceDirection.BackRight; 
            }
            else // Going down
            {
                SpriteRenderer.flipX = moveDirX > 0f;
                CurrentFaceDirection = SpriteRenderer.flipX ? FaceDirection.FrontRight : FaceDirection.FrontLeft; 
            }
        }

        public void AnimateAttack(float attackDirX, float attackDirY)
        {
            Animator.SetFloat("AttackDirX", attackDirX);
            Animator.SetFloat("AttackDirY", attackDirY);
            SpriteRenderer.flipX = (attackDirX > 0 && attackDirY < 0) || (attackDirX < 0 && attackDirY > 0);
            SetAnimationSpeedTo(3.5f); // TODO: change the animation speed
            Animator.SetTrigger("Attack");
        }

        public void StopMoving()
        {
            Animator.SetBool("IsMoving", false);
        }
        
        public void SetAnimationSpeedToDefault()
        {
            Animator.speed = 1; 
        }

        public void SetAnimationSpeedTo(float speed)
        {
            Animator.speed = speed;
        }
    }
}
