using System;
using Pathfinding.Util;
using UnityEditor;
using UnityEngine;

namespace Resources.Scripts.Enemy.Attacks
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
        [SerializeField] [Range(2, 10)] private float ProjectileSpeed;

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

        protected override void Update()
        {
            base.Update();
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
            if (TriggeredDuringAnimation) 
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            else 
                DoShootProjectile();
        }

        protected override bool WillHitTheTarget(Vector3 playerPositionOrDirection)
        {
            throw new NotImplementedException();
        }

        public void DoShootProjectile() // Called by the animator
        {
            OnShoot?.Invoke(this, new OnShootEventArgs()
            {
                ProjectileOrigin = AttackOrigin,
                ShootDirection = PlayerDirection
            });
            if (!TriggeredDuringAnimation)
                AttackEnd();
        }

        private void CreateProjectileOnShoot(object sender, OnShootEventArgs e)
        {
            var newProjectile = Instantiate(ProjectilePrefab, AttackOrigin, Quaternion.identity, null);
            var shootDirection = (e.ShootDirection).normalized;
            var enemyPhysicalDamage = EnemyStatsManager.PhysicalDamage; 
            var enemyMagicalDamage = EnemyStatsManager.MagicalDamage; 
            newProjectile.GetComponent<ProjectileScript>().Setup(shootDirection, ProjectileSpeed, enemyPhysicalDamage, enemyMagicalDamage);
        }

    }
}
