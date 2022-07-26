using System;
using Game;
using Player;
using UnityEngine;

namespace Enemy.Attacks
{
    public class PetrifyingAttack : BaseAttack
    {
        [SerializeField] private FieldOfView AttackFoV;
        [SerializeField] [Range(0, 360)] private float attackAoE;
        public float AttackAoE => attackAoE;
        
        [SerializeField] [Range(0, 5)] private float crowdControlDuration;
        public float CrowdControlDuration => crowdControlDuration;
        
        protected void Start()
        {
            AttackFoV.SetViewDistance(AttackRange);
            AttackFoV.SetFieldOfView(AttackAoE);
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
            
            var current = AttackPreparationTime - _attackPreparationCurrentTime;
            Debug.Log(current);
            // make sound once (?)
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            if(hasAttackAnimation)
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            AudioManager.instance.Play("Hit enemy");
            if (AttackFoV.PlayerIsOnFieldOfView && IsPlayerLookingToTheEye(AttackFoV.Player.GetComponent<PlayerController>().PlayerFaceDir, EnemyAnimationController.CurrentFaceDirection))
                AttackFoV.Player.GetComponent<PlayerStateMachine>()
                    .PetrifyPlayer(CrowdControlDuration);
            EnemyCombatManager.IsAttacking = true;
            if(hasAttackAnimation)
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
            isOnHalfOfAttackAnimation = false;
            AttackFoV.gameObject.SetActive(false);
        }
        
        public void SetIsOnHalfOfAttackAnimation() // called by animator (?)
        {
            isOnHalfOfAttackAnimation = true;
        }

    }
}