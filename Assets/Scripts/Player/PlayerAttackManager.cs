using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Useful;
using Random = UnityEngine.Random;


namespace Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        private enum Directions
        {
            UpLeft,
            UpRight,
            DownLeft,
            DownRight,
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField] private float tryAttackingRange;
        [SerializeField] [Range(1f, 5f)] private float animationAttackSpeed;
        [SerializeField] private float defaultAttackCooldown;
        public float CurrentAttackCooldown { get; set; }

        public int CurrentAttackID { get; private set; } 
        [SerializeField]
        private LayerMask enemyLayerMask;
        
        private Animator _animator;
        public PlayerHealthManager playerHealthManager;
        public PlayerStateMachine playerStateMachine;
        public Material standardMaterial;
        private Renderer _renderer;

        public PowerUpEffects powerUpEffects;
        private PlayerStatsController _playerStatsController;

        [SerializeField] private LayerMask enemyLayers;
        private Directions _direction;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<Renderer>();
            playerHealthManager = GetComponent<PlayerHealthManager>();
            playerStateMachine = GetComponent<PlayerStateMachine>();
            powerUpEffects = GetComponent<PowerUpEffects>();
            _playerStatsController = GetComponent<PlayerStatsController>();
        }

        public void TryToAttack()
        {
            if (CurrentAttackCooldown / _playerStatsController.CurrentAttackSpeed > 0)
            {
                CurrentAttackCooldown -= Time.deltaTime;
            }
            else
            {
                var closestEnemy = ThereIsEnemiesInRange();
                if (closestEnemy == null) return;
                LookToTheEnemy(closestEnemy);
                Attack();
                CurrentAttackCooldown = defaultAttackCooldown;
            }
        }
        
        private void Attack()
        {
            CurrentAttackID = Random.Range(1, 100000);
            _direction = GetAnimationDirection();
            playerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
            AnimateAttack();
        }

        public void AnimateAttack()
        {
            // Set attack animation
            _animator.speed = animationAttackSpeed; // CurrentAttackSpeed * 0.2f;
            _animator.SetTrigger("Attack");
            _animator.SetBool("IsAttacking", true);
        }

        public GameObject ThereIsEnemiesInRange()
        {
            var nearbyEnemies = new List<Collider2D>();
            var filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayerMask);
            Physics2D.OverlapCircle(transform.position, tryAttackingRange, filter, nearbyEnemies);
            var shorterDistanceEnemyIndex = -1;
            var shorterDistanceEnemy = 100f;
            var currentEnemyDistance = -1f;
            for (var i = 0; i < nearbyEnemies.Count; i++)
            {
                if (nearbyEnemies[i] == null) continue;
                currentEnemyDistance = nearbyEnemies[i].Distance(playerStateMachine.PlayerController.PlayerCollider).distance;
                if (currentEnemyDistance < shorterDistanceEnemy)
                {
                    shorterDistanceEnemy = currentEnemyDistance;
                    shorterDistanceEnemyIndex = i;
                }
            }

            if (shorterDistanceEnemyIndex == -1) return null;
            return nearbyEnemies[shorterDistanceEnemyIndex].gameObject;

        }

        public void AttackEnd()
        {
            _animator.speed = 1f;
            _animator.SetBool("IsAttacking", false);
            playerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
            _renderer.material = standardMaterial;
        }

        public void VerifyAttackCollision(GameObject enemy)
        {
            var enemyCombatManager = enemy.GetComponent<EnemyCombatManager>();
    
            Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
            
            powerUpEffects.ApplyBlessings(enemyCombatManager);

            var criticalHit = CriticalTest();
            enemyCombatManager.TakeDamage(CurrentAttackID, _playerStatsController.CurrentPower * (criticalHit ? _playerStatsController.CurrentCriticalDamage / 100 : 1), attackDirection,
                    false, criticalHit, true, null); 
        }

        private bool CriticalTest()
        {
            Random.InitState((int) Time.realtimeSinceStartup);
            var random = Random.Range(0, 100);
            return random < _playerStatsController.CurrentCriticalRate;
        }

        private Directions GetAnimationDirection()
        {
            float lastMoveX = _animator.GetFloat("LastMoveX");
            float lastMoveY = _animator.GetFloat("LastMoveY");

            switch (lastMoveX)
            {
                case 1: //Up Right, Right and Down Right
                    switch (lastMoveY)
                    {
                        case 1:
                            return Directions.UpRight;

                        case 0:
                            return Directions.Right;

                        case -1:
                            return Directions.DownRight;
                    }

                    break;

                case 0: // Down and Up
                    return lastMoveY == 1 ? Directions.Up : Directions.Down;

                case -1: // Up Left, Left and Down Left
                    switch (lastMoveY)
                    {
                        case 1:
                            return Directions.UpLeft;

                        case 0:
                            return Directions.Left;

                        case -1:
                            return Directions.DownLeft;
                    }

                    break;
            }

            return Directions.Down;
        }


        private void LookToTheEnemy(GameObject enemy)
        {
            var enemyPosition = enemy.transform.position;
            var playerPosition = transform.position;
            var faceDirection = UtilitiesClass.Get8DirectionFromAngle(UtilitiesClass.GetAngleFromVectorFloat(new Vector3(enemyPosition.x - playerPosition.x, enemyPosition.y - playerPosition.y)));
            _animator.SetFloat("MoveX", faceDirection.x);
            _animator.SetFloat("MoveY", faceDirection.y);
            
            _animator.SetFloat("LastMoveX", faceDirection.x);
            _animator.SetFloat("LastMoveY", faceDirection.y);
            
            /* Necessary? */
            playerStateMachine.PlayerController.lastMoveX = faceDirection.x;
            playerStateMachine.PlayerController.lastMoveY = faceDirection.y;
            
            /* I need to set the face dir variable? */
        }

        private void OnDrawGizmos()
        {
            // Gizmos.DrawWireSphere(transform.position, CurrentWeaponRange);
        }
        
        
    }
}