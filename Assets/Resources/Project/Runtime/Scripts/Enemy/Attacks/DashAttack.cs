using System.Collections;
using Resources.Project.Runtime.Scripts.Player;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Enemy.Attacks
{
    public class DashAttack : BaseAttack
    {
        public BoxCollider2D AttackHitbox;
        [SerializeField] private bool HasAttackAnimation;
        public LayerMask PlayerLayer;
        public bool IsOnHalfOfAttackAnimation = false;

        protected override void Update()
        {
            base.Update();  
            AttackOrigin = transform.position;
        }

        public override void PreparingAttack()
        {
            // make sound once (?)
            EnemyMovementHandler.Rigidbody.AddForce(-EnemyStateMachine.PlayerDirection * EnemyStatsManager.PreparationWalkDistance, ForceMode2D.Force);
         
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            AttackHitbox.enabled = true;
            if(HasAttackAnimation)
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * EnemyStatsManager.AttackSpeed, ForceMode2D.Impulse);
            ProbablyGonnaHit = WillHitTheTarget(playerDirection);
            EnemyCombatManager.IsAttacking = true;
            if(!HasAttackAnimation)
                AttackEnd();
        }
        
        public override void AttackEnd()
        {
            base.AttackEnd();
            IsOnHalfOfAttackAnimation = false;
            StartCoroutine(DeactivateAttackHitBox(0.4f));
        }
        
        protected override bool WillHitTheTarget(Vector3 playerDirection)
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

            
            // Debug.DrawRay(new Vector2(transform.position.x + EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            // Debug.DrawRay(new Vector2(transform.position.x - EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            // Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
         
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

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.gameObject.GetComponent<PlayerController>().DodgeFailed = true;
            Debug.Log("Colidi e triggei o dodge failed!");
        }
    }
}