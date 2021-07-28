using System;
using System.Collections;
using Game;
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
            AttackFoV.SetAimDirection(EnemyAnimationController.CurrentFaceDirection);
        }

        public override void PreparingAttack()
        {
            if (!AttackFoV.gameObject.activeSelf)
            {
                AttackFoV.gameObject.SetActive(true); 
                Debug.Log("Liguei");
            }
            // make sound once (?)
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            if(HasAttackAnimation)
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            AudioManager.instance.Play("Hit enemy");
            if (AttackFoV.PlayerIsOnFieldOfView && IsPlayerLookingToTheEye(AttackFoV.Player.GetComponent<PlayerController>().PlayerFaceDir, EnemyAnimationController.CurrentFaceDirection))
                AttackFoV.Player.GetComponent<PlayerStateMachine>()
                    .PetrifyPlayer(EnemyStatsManager.CrowdControlDuration);
            else
                Debug.Log("Player nao encontrado");
            ProbablyGonnaHit = PredictAccuracy(playerDirection);
            EnemyCombatManager.IsAttacking = true;
            if(!HasAttackAnimation)
                AttackEnd();
        }

        private bool IsPlayerLookingToTheEye(PlayerController.PlayerFaceDirection playerFaceDir, EnemyAnimationController.FaceDirection enemyFaceDirection)
        {
            return enemyFaceDirection switch
            {
                EnemyAnimationController.FaceDirection.BackLeft => playerFaceDir == PlayerController.PlayerFaceDirection.Down ||
                                                                   playerFaceDir == PlayerController.PlayerFaceDirection.DownRight ||
                                                                   playerFaceDir == PlayerController.PlayerFaceDirection.Right,
                EnemyAnimationController.FaceDirection.FrontRight => playerFaceDir == PlayerController.PlayerFaceDirection.Up ||
                                                                     playerFaceDir == PlayerController.PlayerFaceDirection.UpLeft ||
                                                                     playerFaceDir == PlayerController.PlayerFaceDirection.Left,
                EnemyAnimationController.FaceDirection.FrontLeft => playerFaceDir == PlayerController.PlayerFaceDirection.Up ||
                                                                    playerFaceDir == PlayerController.PlayerFaceDirection.UpRight ||
                                                                    playerFaceDir == PlayerController.PlayerFaceDirection.Right,
                EnemyAnimationController.FaceDirection.BackRight => playerFaceDir == PlayerController.PlayerFaceDirection.Down ||
                                                                    playerFaceDir == PlayerController.PlayerFaceDirection.DownLeft ||
                                                                    playerFaceDir == PlayerController.PlayerFaceDirection.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(enemyFaceDirection), enemyFaceDirection, null)
            };
        }
        
        public override void AttackEnd()
        {
            base.AttackEnd();
            IsOnHalfOfAttackAnimation = false;
            AttackFoV.gameObject.SetActive(false);
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