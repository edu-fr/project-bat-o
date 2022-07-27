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

        [SerializeField] private float currentWeaponRange;
        [SerializeField] private float defaultAttackCooldown;
        [SerializeField] [Range(1f, 5f)] private float currentAttackSpeed;
        public float CurrentAttackCooldown { get; set; }
        
        private List<GameObject> _enemiesHit;
        [SerializeField]
        private LayerMask enemyLayerMask;
        
        private Animator _animator;
        public PlayerHealthManager playerHealthManager;
        public PlayerStateMachine playerStateMachine;
        public Material standardMaterial;
        public Material fireMaterial;
        public Material iceMaterial;
        public Material thunderMaterial;
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

        private void Start()
        {
            _enemiesHit = new List<GameObject>();
            // SetWeaponStats();
            CurrentAttackCooldown = 0; 
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
            _direction = GetAnimationDirection();
            playerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
            AnimateAttack();
        }

        public void AnimateAttack()
        {
            // Set attack animation
            _animator.speed = currentAttackSpeed; // CurrentAttackSpeed * 0.2f;
            _animator.SetTrigger("Attack");
            _animator.SetBool("IsAttacking", true);
        }

        public GameObject ThereIsEnemiesInRange()
        {
            var nearbyEnemies = new List<Collider2D>();
            var filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayerMask);
            Physics2D.OverlapCircle(transform.position, currentWeaponRange * 5, filter, nearbyEnemies);
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
            
        // public void FlurryAttack()
        // {
        //     PlayerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
        //     Animator.speed = CurrentAttackSpeed * 0.2f;
        //     Animator.SetTrigger("Attack");
        //     Animator.SetBool("IsAttacking", true);
        // }

        public void AttackEnd()
        {
            _animator.speed = 1f;
            _animator.SetBool("IsAttacking", false);
            playerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
            _renderer.material = standardMaterial;
            ClearEnemiesHitList();
        }

        public void VerifyAttackCollision(GameObject enemy)
        {
            // avoids the enemy been hit twice in the same attack
            if (!_enemiesHit.Contains(enemy))
            {
                _enemiesHit.Add(enemy);
        
                Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
                
                powerUpEffects.
                if (CriticalTest()) // critical hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(_playerStatsController.CurrentPower * _playerStatsController.CurrentCriticalDamage, attackDirection,
                        _playerStatsController.CurrentAttackSpeed ,false, true, true, null);
                else                // normal hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(_playerStatsController.CurrentPower, attackDirection,
                        _playerStatsController.CurrentAttackSpeed,false, false, true, null);
                // PowerUpActivator.ApplyEffectsOnEnemies(enemy, CurrentEffect);
            }
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

        private void ClearEnemiesHitList()
        {
            if (_enemiesHit.Count > 0)
            {
                _enemiesHit.Clear();
            }
        }
        
        private void OnDrawGizmos()
        {
            // Gizmos.DrawWireSphere(transform.position, CurrentWeaponRange);
        }
        
        
    }
}