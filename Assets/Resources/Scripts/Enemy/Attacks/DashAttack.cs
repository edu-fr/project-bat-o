using System.Collections;
using UnityEngine;

namespace Resources.Scripts.Enemy.Attacks
{
    public class DashAttack : BaseAttack
    {
        public BoxCollider2D AttackHitbox;
        
        public LayerMask PlayerLayer;

        [SerializeField]
        private float AttackVelocity = 12f;

        public bool IsOnHalfOfAttackAnimation = false;

        protected override void Update()
        {
            base.Update();
        }

        public override void PreparingAttack()
        {
            // make sound once (?)
            EnemyMovementHandler.Rigidbody.AddForce(-EnemyStateMachine.PlayerDirection * EnemyStatsManager.PreparationWalkDistance, ForceMode2D.Force);
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            AttackHitbox.enabled = true;
            EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * AttackVelocity, ForceMode2D.Impulse);
            ProbablyGonnaHit = PredictAccuracy(playerDirection);
            EnemyCombatManager.IsAttacking = true;
        }
        
        public override void AttackEnd()
        {
            base.AttackEnd();
            IsOnHalfOfAttackAnimation = false;
            StartCoroutine(DeactivateAttackHitBox(0.4f));
        }
        
        private bool PredictAccuracy(Vector3 playerDirection)
        {
            var currentPosition = transform.position;
            RaycastHit2D raycastHit2DRight =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x + EnemyMovementHandler.BoxCollider2D.size.x/2, currentPosition.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DLeft =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x - EnemyMovementHandler.BoxCollider2D.size.x/2, currentPosition.y),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DUp =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x, currentPosition.y + EnemyMovementHandler.BoxCollider2D.size.y/2),
                    playerDirection, 3.5f, PlayerLayer);
            RaycastHit2D raycastHit2DDown =
                Physics2D.Raycast(
                    new Vector2(currentPosition.x, currentPosition.y - EnemyMovementHandler.BoxCollider2D.size.y/2),
                    playerDirection, 3.5f, PlayerLayer);

            
            Debug.DrawRay(new Vector2(transform.position.x + EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x - EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * AttackVelocity, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * AttackVelocity, Color.red, 2);
            
            return (raycastHit2DRight.rigidbody != null || raycastHit2DLeft.rigidbody != null ||
                    raycastHit2DUp.rigidbody != null || raycastHit2DDown.rigidbody != null);
        }
        
        public void SetIsOnHalfOfAttackAnimation() // called by animator (?)
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