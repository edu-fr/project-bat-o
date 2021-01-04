using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttackManager : MonoBehaviour
    {
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        private EnemyBehavior EnemyBehavior;

        [SerializeField]
        private LayerMask PlayerLayer;

        private float AttackVelocity = 12f;
        private bool AttackEnded = false;
        public bool IsOnHalfOfAttackAnimation = false;
        private float AttackCurrentRecoveryTime = 0;
        private float AttackRecoveryTime = 1.5f;
        public bool ProbablyGonnaHit;
        

        private void Awake()
        {
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void Update()
        {
            if (AttackEnded)
            {
                EnemyCombatManager.IsAttacking = false;
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    AttackEnded = false;
                    EnemyStateMachine.IsAttackingNow = false;
                    ProbablyGonnaHit = false;
                    EnemyBehavior.AiPath.enabled = true;
                    EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
                }
            }
        }

        public void Attack(Vector3 playerDirection)
        {
            EnemyBehavior.Animator.SetFloat("AttackDirX", playerDirection.x);
            EnemyBehavior.Animator.SetFloat("AttackDirY", playerDirection.y);
            EnemyBehavior.Animator.speed = 3.5f;
            EnemyBehavior.Animator.SetTrigger("Attack");
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * AttackVelocity, ForceMode2D.Impulse);
            ProbablyGonnaHit = PredictAccuracy(playerDirection);
            EnemyCombatManager.IsAttacking = true;
        }

        private bool PredictAccuracy(Vector3 playerDirection)
        {
            RaycastHit2D raycastHit2DRight =
                Physics2D.Raycast(
                    new Vector2(transform.position.x + EnemyBehavior.CircleCollider.radius, transform.position.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DLeft =
                Physics2D.Raycast(
                    new Vector2(transform.position.x - EnemyBehavior.CircleCollider.radius, transform.position.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DUp =
                Physics2D.Raycast(
                    new Vector2(transform.position.x, transform.position.y + EnemyBehavior.CircleCollider.radius),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DDown =
                Physics2D.Raycast(
                    new Vector2(transform.position.x, transform.position.y - EnemyBehavior.CircleCollider.radius),
                    playerDirection, 3.5f, PlayerLayer);

            /* DEBUG RAYS
            Debug.DrawRay(new Vector2(transform.position.x + EnemyBehavior.CircleCollider.radius, transform.position.y), playerDirection * AttackVelocity, Color.red, 4);
            Debug.DrawRay(new Vector2(transform.position.x - EnemyBehavior.CircleCollider.radius, transform.position.y), playerDirection * AttackVelocity, Color.red, 4);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + EnemyBehavior.CircleCollider.radius), playerDirection * AttackVelocity, Color.red, 4);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - EnemyBehavior.CircleCollider.radius), playerDirection * AttackVelocity, Color.red, 4);
            */

            return (raycastHit2DRight.rigidbody != null || raycastHit2DLeft.rigidbody != null ||
                    raycastHit2DUp.rigidbody != null || raycastHit2DDown.rigidbody != null);
        }

        public void AttackEnd()
        {
            // Called by animation end
            AttackEnded = true;
            EnemyBehavior.Animator.speed = 1f;
            IsOnHalfOfAttackAnimation = false;
        }

        public void SetIsOnHalfOfAttackAnimation()
        {
            IsOnHalfOfAttackAnimation = true;
        }
    }
}