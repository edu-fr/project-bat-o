using System;
using Resources.Scripts.Enemy.Attacks;
using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class ShootProjectile : BaseAttack
    {
        private float AttackCurrentRecoveryTime = 0;
        private float AttackRecoveryTime = 0.3f;
        
        // Projectile variables
        [SerializeField]
        private Transform ProjectilePrefab;
        
        private Vector3 PlayerDirection;
        private Vector2 ProjectileOrigin;
        [SerializeField]
        private float ProjectileSpeed; 
        
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
            Debug.Log("Projectile override!");
        }

        public override void PreparingAttack()
        {
            throw new NotImplementedException();
        }

        public override void Attack(Vector3 playerDirection)
        {    
            PlayerDirection = playerDirection;
            EnemyMovementHandler.Animator.SetFloat("AttackDirX", playerDirection.x);
            EnemyMovementHandler.Animator.SetFloat("AttackDirY", playerDirection.y);
            EnemyMovementHandler.Animator.SetTrigger("Attack");
            EnemyCombatManager.IsAttacking = true;
            
            // Correcting particle system position. (All arbitrary values that could change according to the enemy sprite) 
            if (Math.Abs(PlayerDirection.x) > Math.Abs(playerDirection.y))
            {
                if (playerDirection.y < 0)
                    ProjectileOrigin = transform.position + (new Vector3(playerDirection.x * 0.5f, (playerDirection.y * 0.8f) + 0.4f));
                else 
                    ProjectileOrigin = transform.position + (new Vector3(playerDirection.x * 0.5f, playerDirection.y * 0.8f));
            }
            else
                ProjectileOrigin = transform.position + (new Vector3(playerDirection.x * 0.5f, playerDirection.y * 0.3f));
        }

        public void ShootProjectileDuringAnimation() // Called by the animator
        {
            Debug.Log("Shoot during animation");
            OnShoot?.Invoke(this, new OnShootEventArgs()
            {
                ProjectileOrigin = ProjectileOrigin,
                ShootDirection = PlayerDirection
            });
        }

        private void CreateProjectileOnShoot(object sender, OnShootEventArgs e)
        {
            var newProjectile = Instantiate(ProjectilePrefab, ProjectileOrigin, Quaternion.identity, transform);
            var shootDirection = (e.ShootDirection).normalized;
            newProjectile.GetComponent<ProjectileScript>().Setup(shootDirection, ProjectileSpeed);
        }
    }
}
