using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttackManager : MonoBehaviour
    {
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        private EnemyBehavior EnemyBehavior;
        public BoxCollider2D AttackHitbox;
        
        public LayerMask PlayerLayer;

        [SerializeField]
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
            if (AttackEnded && EnemyStateMachine.State == EnemyStateMachine.States.Attacking)
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
                    // Debug.Log("MeleeAttackManager");
                }
            }
        }

        public void Attack(Vector3 playerDirection)
        {
            AttackHitbox.enabled = true;
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
            var currentPosition = transform.position;
            RaycastHit2D raycastHit2DRight =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x + EnemyBehavior.BoxCollider2D.size.x/2, currentPosition.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DLeft =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x - EnemyBehavior.BoxCollider2D.size.x/2, currentPosition.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DUp =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x, currentPosition.y + EnemyBehavior.BoxCollider2D.size.y/2),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DDown =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x, currentPosition.y - EnemyBehavior.BoxCollider2D.size.y/2),
                    playerDirection, 3.5f, PlayerLayer);

            
            Debug.DrawRay(new Vector2(transform.position.x + EnemyBehavior.BoxCollider2D.size.x/2, transform.position.y), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x - EnemyBehavior.BoxCollider2D.size.x/2, transform.position.y), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + EnemyBehavior.BoxCollider2D.size.y/2), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - EnemyBehavior.BoxCollider2D.size.y/2), playerDirection * AttackVelocity, Color.red, 2);
            

            return (raycastHit2DRight.rigidbody != null || raycastHit2DLeft.rigidbody != null ||
                    raycastHit2DUp.rigidbody != null || raycastHit2DDown.rigidbody != null);
        }

        public void AttackEnd()
        {
            // Called by animation end
            AttackEnded = true;
            EnemyBehavior.Animator.speed = 1f;
            IsOnHalfOfAttackAnimation = false;
            StartCoroutine(DeactivateAttackHitBox(0.4f));
        }

        public void SetIsOnHalfOfAttackAnimation()
        {
            IsOnHalfOfAttackAnimation = true;
        }

        private IEnumerator DeactivateAttackHitBox(float timeToDeactivate)
        {
            yield return new WaitForSeconds(timeToDeactivate);
            AttackHitbox.enabled = false;
        }
    }
}