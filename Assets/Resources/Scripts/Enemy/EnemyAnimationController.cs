using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public enum SpriteType
        {
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

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AnimateMovement(float moveDirX, float moveDirY)
        {
            Animator.SetBool("IsMoving", true);
            Animator.SetFloat("MoveX", moveDirX);
            Animator.SetFloat("MoveY", moveDirY);
            SetFlipAndFaceDirection(moveDirX, moveDirY);
        }

        public void SetFlipAndFaceDirection(float x, float y)
        {
            if (y > 0) // Going up (sprite back)
            {
                SpriteRenderer.flipX = x < 0f;
                CurrentFaceDirection = SpriteRenderer.flipX ? FaceDirection.BackLeft : FaceDirection.BackRight;
            }
            else // Going down
            {
                SpriteRenderer.flipX = x > 0f;
                CurrentFaceDirection = SpriteRenderer.flipX ? FaceDirection.FrontRight : FaceDirection.FrontLeft;
            }
        }

        public void AnimateAttack(float attackDirX, float attackDirY)
        {
            Animator.SetFloat("AttackDirX", attackDirX);
            Animator.SetFloat("AttackDirY", attackDirY);
            SetFlipAndFaceDirection(attackDirX, attackDirY);
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

        public void TriggerDieAnimation()
        {
            Animator.SetTrigger("Die");
        }
    }
}
