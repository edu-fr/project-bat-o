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

        public class Attack
        {
            public int AttackID { get; private set; } 
            public bool CriticalHit { get; private set; }

            public Attack(float currentCriticalRate)
            {
                AttackID = Random.Range(1, 100000);
                CriticalHit = CriticalTest(currentCriticalRate);
            }
            
            private static bool CriticalTest(float currentCriticalRate)
            {
                Random.InitState((int) Time.realtimeSinceStartup);
                var random = Random.Range(0, 100);
                return random < currentCriticalRate;
            }
        }
        
        [SerializeField] private LayerMask enemyLayerMask;
        private Animator _animator;
        public PlayerStateMachine playerStateMachine;
        public Material standardMaterial;
        private Renderer _renderer;
        private Collider2D _collider2D;

        public PowerUpEffects powerUpEffects;
        private PlayerStatsController _playerStats;
        private PlayerController _playerController;

        [SerializeField] private float tryAttackingRange;
        [SerializeField] [Range(1f, 5f)] private float animationAttackSpeed;
        [SerializeField] private float defaultAttackCooldown;
        
        private Directions _direction;
        public float CurrentAttackCooldown { get; set; }
        public Attack CurrentAttack { get; private set; }

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _collider2D = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<Renderer>();
            playerStateMachine = GetComponent<PlayerStateMachine>();
            powerUpEffects = GetComponent<PowerUpEffects>();
            _playerStats = GetComponent<PlayerStatsController>();
        }

        public void TryToAttack()
        {
            if (CurrentAttackCooldown > 0)
            {
                CurrentAttackCooldown -= Time.deltaTime;
            }
            else
            {
                var closestEnemy = ThereIsEnemiesInRange();
                if (closestEnemy == null) return;
                LookToTheEnemy(closestEnemy);
                StartAttack();
                CurrentAttackCooldown = defaultAttackCooldown;
            }
        }
        
        private void StartAttack()
        {
            CurrentAttack = new Attack(_playerStats.CurrentCriticalRate);
            _direction = GetAnimationDirection();
            playerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
            AnimateAttack();
        }

        public void AnimateAttack()
        {
            // Set attack animation
            _animator.speed = _playerStats.CurrentAttackSpeed; 
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
                currentEnemyDistance = nearbyEnemies[i].Distance(_collider2D).distance;
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
            enemyCombatManager.TakeDamage(CurrentAttack.AttackID, _playerStats.CurrentPower * (CurrentAttack.CriticalHit ? _playerStats.CurrentCriticalDamage / 100 : 1), attackDirection,
                    false, CurrentAttack.CriticalHit, true, null); 
            powerUpEffects.ApplyBlessings(enemyCombatManager);
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
            _playerController.lastMoveX = faceDirection.x;
            _playerController.lastMoveY = faceDirection.y;
            /* I need to set the face dir variable? */
        }

        private void OnDrawGizmos()
        {
            // Gizmos.DrawWireSphere(transform.position, CurrentWeaponRange);
        }
        
        
    }
}