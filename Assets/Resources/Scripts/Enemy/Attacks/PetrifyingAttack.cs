using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Resources.Scripts.Enemy.Attacks
{
    public class PetrifyingAttack : BaseAttack
    {
        [SerializeField] private bool HasAttackAnimation;
        [SerializeField] private FieldOfView AttackFoV;
        public LayerMask PlayerLayer;
        public bool IsOnHalfOfAttackAnimation = false;

        protected override void Start()
        {
            base.Start();
            AttackFoV.SetViewDistance(EnemyStatsManager.AttackRange);
            AttackFoV.SetFieldOfView(EnemyStatsManager.AreaOfEffect);
        }
        
        protected override void Update()
        {
            base.Update();  
            AttackOrigin = transform.position;
        }

        public override void PreparingAttack()
        {
            if (!AttackFoV.enabled)
            {
                AttackFoV.enabled = true; 
                Debug.Log("Liguei");
            }
            // make sound once (?)
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            if(HasAttackAnimation)
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            Debug.Log(AttackFoV.PlayerIsOnFieldOfView ? "DALE" : "Player nao encontrado");
            ProbablyGonnaHit = PredictAccuracy(playerDirection);
            EnemyCombatManager.IsAttacking = true;
            if(!HasAttackAnimation)
                AttackEnd();
        }
        
        public override void AttackEnd()
        {
            base.AttackEnd();
            IsOnHalfOfAttackAnimation = false;
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

            
            Debug.DrawRay(new Vector2(transform.position.x + EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x - EnemyMovementHandler.BoxCollider2D.size.x/2, transform.position.y), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - EnemyMovementHandler.BoxCollider2D.size.y/2), playerDirection * EnemyStatsManager.AttackSpeed, Color.red, 2);
            
            return (raycastHit2DRight.rigidbody != null || raycastHit2DLeft.rigidbody != null ||
                    raycastHit2DUp.rigidbody != null || raycastHit2DDown.rigidbody != null);
        }
        
        public void SetIsOnHalfOfAttackAnimation() // called by animator (?)
        {
            IsOnHalfOfAttackAnimation = true;
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.gameObject.GetComponent<PlayerController>().DodgeFailed = true;
            Debug.Log("Colidi e triggei o dodge failed!");
        }
    }
}