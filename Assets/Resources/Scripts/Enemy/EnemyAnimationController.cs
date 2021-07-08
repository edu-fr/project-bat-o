using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public enum SpriteType {
            FourDir,
            FourDirMirror,
        }

        private Animator Animator;
        private SpriteRenderer SpriteRenderer;
        private Vector3 CurrentDirection;
        private float CurAngle;
        private Vector3 FaceDirection;

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
            SpriteRenderer.flipX = (moveDirX > 0 && moveDirY < 0) || (moveDirX < 0 && moveDirY > 0);
            if (SpriteRenderer.flipX)
            {
                Debug.Log("X: " + moveDirX + " Y: " + moveDirY);
            }
            SetCurrentFaceDirectionTo(new Vector3(moveDirX, moveDirY));
        }

        public void AnimateAttack(float attackDirX, float attackDirY)
        {
            Animator.SetFloat("AttackDirX", attackDirX);
            Animator.SetFloat("AttackDirY", attackDirY);
            SpriteRenderer.flipX = (attackDirX > 0 && attackDirY < 0) || (attackDirX < 0 && attackDirY > 0);
            if (SpriteRenderer.flipX)
            {
                Debug.Log("X: " + attackDirX + " Y: " + attackDirY);
            }
            SetAnimationSpeedTo(3.5f); // TODO: change the animation speed
            Animator.SetTrigger("Attack");
        }

        public void StopMoving()
        {
            Animator.SetBool("IsMoving", false);
        }

        public void SetCurrentFaceDirectionTo(Vector3 desiredDirection)
        {
            CurAngle = UtilitiesClass.GetAngleFromVectorFloat(desiredDirection);
            FaceDirection = UtilitiesClass.GetDirectionFromAngle(CurAngle);
            Animator.SetFloat("MoveX", FaceDirection.x);
            Animator.SetFloat("MoveY", FaceDirection.y);
            SpriteRenderer.flipX = (FaceDirection.x > 0 && FaceDirection.y < 0) || (FaceDirection.x < 0 && FaceDirection.y > 0);

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
