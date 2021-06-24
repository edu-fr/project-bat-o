using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyRangedAttackManager : MonoBehaviour
    { 
        private EnemyCombatManager EnemyCombatManager;
        private EnemyStateMachine EnemyStateMachine;
        private EnemyBehavior EnemyBehavior;
        private bool AttackEnded = false;
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

        private void Awake()
        {
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyBehavior = GetComponent<EnemyBehavior>();

            OnShoot += CreateProjectileOnShoot; // Subscribing a new event to the event handler
        }

        private void Update()
        {
            if (AttackEnded)
            {
                EnemyCombatManager.IsAttacking = false;
                AttackCurrentRecoveryTime += Time.deltaTime;
                if (AttackCurrentRecoveryTime > AttackRecoveryTime)
                {
                    AttackCurrentRecoveryTime = 0;
                    EnemyBehavior.AiPath.enabled = true;
                    EnemyStateMachine.ChangeState(EnemyStateMachine.States.Chasing);
                    EnemyStateMachine.IsAttackingNow = false;
                }
            }
        }
        
        
        public void Attack(Vector3 playerDirection)
        {    
            PlayerDirection = playerDirection;
            EnemyBehavior.Animator.SetFloat("AttackDirX", playerDirection.x);
            EnemyBehavior.Animator.SetFloat("AttackDirY", playerDirection.y);
            EnemyBehavior.Animator.SetTrigger("Attack");
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

        public void ShootArrowDuringAnimation()
        {
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
        

        public void AttackEndRanged()
        {
            // Called by animation end
            AttackEnded = true;
            EnemyBehavior.Animator.speed = 1f;
        }
    }
}
