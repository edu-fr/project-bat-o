using System;
using Resources.Project.Runtime.Scripts.Game;
using Resources.Project.Runtime.Scripts.Player;
using Resources.Scripts.Enemy;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Enemy.Attacks
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
            }
            
            if (AlreadyPredicted) return;
            
            var current = EnemyStatsManager.AttackPreparationTime - EnemyStateMachine.AttackPreparationCurrentTime;
            Debug.Log(current);
            
            if (current < EnemyStatsManager.TimeToPredictIfWillHitTheTarget) 
                WillHitTheTarget(Vector3.zero);

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
            ProbablyGonnaHit = WillHitTheTarget(playerDirection);
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
            AlreadyPredicted = false;
        }
        
        protected override bool WillHitTheTarget(Vector3 nullVector)
        {
            AlreadyPredicted = true;
            var willHit = AttackFoV.PlayerIsOnFieldOfView;
            if(willHit) Debug.Log("IA ACERTAR!");
            return willHit;
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