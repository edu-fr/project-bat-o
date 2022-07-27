using System;
using Objects;
using UnityEngine;

namespace Enemy.Attacks
{
    public class ShootProjectile : BaseAttack
    {
        // Projectile variables
        [SerializeField] private Transform ProjectilePrefab;
        
        [SerializeField] private Vector2 ProjectileOriginFrontLeft;
        [SerializeField] private Vector2 ProjectileOriginFrontRight;
        [SerializeField] private Vector2 ProjectileOriginBackRight;
        [SerializeField] private Vector2 ProjectileOriginBackLeft;
        
        private Vector3 PlayerDirection;

        public event EventHandler<OnShootEventArgs> OnShoot; // Creation of the event handler

        public class OnShootEventArgs : EventArgs
        {
            public Vector3 ProjectileOrigin;
            public Vector3 ShootDirection;
        }

        protected override void Awake()
        {
            base.Awake();
            OnShoot += CreateProjectileOnShoot; // Subscribing a new event to the event handler
        }
        
        public override void PreparingAttack()
        {
            SetProjectileOrigin(EnemyAnimationController.CurrentFaceDirection);
        }

        private void SetProjectileOrigin(EnemyAnimationController.FaceDirection currentFaceDirection)
        {
            AttackOrigin = currentFaceDirection switch
            {
                EnemyAnimationController.FaceDirection.FrontRight => ProjectileOriginFrontRight,
                EnemyAnimationController.FaceDirection.FrontLeft => ProjectileOriginFrontLeft,
                EnemyAnimationController.FaceDirection.BackRight => ProjectileOriginBackRight,
                EnemyAnimationController.FaceDirection.BackLeft => ProjectileOriginBackLeft,
                _ => Vector2.zero
            };
            var playerTransformPosition = transform.position;
            AttackOrigin = new Vector2(playerTransformPosition.x + AttackOrigin.x,
                playerTransformPosition.y + AttackOrigin.y);
        }

        public override void Attack(Vector3 playerDirection)
        {    
            PlayerDirection = playerDirection;
            EnemyCombatManager.IsAttacking = true;
            if (triggeredDuringAnimation) 
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            else 
                DoShootProjectile();
        }
        
        public void DoShootProjectile() // Called by the animator
        {
            OnShoot?.Invoke(this, new OnShootEventArgs()
            {
                ProjectileOrigin = AttackOrigin,
                ShootDirection = PlayerDirection
            });
            if (!triggeredDuringAnimation)
                AttackEnd();
        }

        private void CreateProjectileOnShoot(object sender, OnShootEventArgs e)
        {
            var newProjectile = Instantiate(ProjectilePrefab, AttackOrigin, Quaternion.identity, null);
            var shootDirection = (e.ShootDirection).normalized;
            newProjectile.GetComponent<ProjectileScript>().Setup(shootDirection, EnemyStatsManager.CurrentAttackSpeed * AttackSpeedModifier, EnemyStatsManager.CurrentPower);
        }

    }
}
