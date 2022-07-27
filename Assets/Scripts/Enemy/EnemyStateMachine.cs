using System;
using System.Collections;
using Enemy.Attacks;
using Game;
using Objects;
using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public enum States
        {
            Standard,
            Chasing,
            TakingKnockBack,
            PreparingAttack,
            Attacking,
            Dying
        }
        
              

        /* State related variables */
        [Header("State variables")]
        private States _state;
        public States State => _state;
        [SerializeField] private States previousState;
        public States PreviousState => previousState;
        public bool IsWalkingAround { get; private set; }
        public bool IsDying { get; private set; }
        private bool _isTargeted;
        private bool _isOnFire;
        public bool IsOnFire => _isOnFire;
        private float _attackPreparationCurrentTime;
        [HideInInspector] public bool isAttackingNow;
        /***/

        /* Components */
        public Vector3 PlayerDirection { get; private set;}
        private EnemyMovementHandler _enemyMovementHandler;
        private Rigidbody2D _enemyRigidbody;
        private EnemyCombatManager _enemyCombatManager;
        private EnemyStatsManager _enemyStatsManager;
        private Dropper _dropper;
        private AIPath _aiPath;
        private BaseAttack _attack;
        private EnemyMaterialManager _enemyMaterialManager;
        private EnemyAnimationController _enemyAnimationController;
        private SpriteRenderer _renderer;
        /***/
        
        private LevelManager _levelManager;
        [SerializeField] private GameObject shadow;
        [SerializeField] private LayerMask obstaclesLayer;
        
        /* DEBUG */
        public bool showDistanceToAttack;
        public bool showLosePlayerDistance;
        public bool showFieldOfView;

        private void Awake()
        {
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            _enemyAnimationController = GetComponent<EnemyAnimationController>();
            _enemyMovementHandler = GetComponent<EnemyMovementHandler>();
            _enemyCombatManager = GetComponent<EnemyCombatManager>();
            _enemyStatsManager = GetComponent<EnemyStatsManager>();
            _enemyMaterialManager = GetComponent<EnemyMaterialManager>();
            _enemyRigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _attack = GetComponent<BaseAttack>();
            _dropper = GetComponent<Dropper>();
            _aiPath = GetComponent<AIPath>();
        }

        private void Update()
        {
            switch (State)
            {
                case States.Standard:
                    IsWalkingAround = true;
                    // Checking if player is close
                    _enemyMovementHandler.CheckSurroundings();

                    // Updating field of view
                    _enemyMovementHandler.fieldOfViewComponent.SetAimDirection(_enemyMovementHandler.enemyAnimationController.CurrentFaceDirection);
                    // Looking for the player
                    if (_enemyMovementHandler.TargetPlayer != null)
                    {
                        ChangeState(States.Chasing);
                    }
                    break;

                case States.Chasing:
                    // Verify if there is close enemies chasing the player
                    if (_enemyMovementHandler.TargetPlayer != null) // Know where the player is
                    {
                        var playerTransformPosition = _enemyMovementHandler.TargetPlayer.transform.position;
                        var enemyTransformPosition = transform.position;
                        _enemyAnimationController.AnimateMovement(playerTransformPosition.x - enemyTransformPosition.x, playerTransformPosition.y - enemyTransformPosition.y);

                        // If enemy is in a certain distance to the player
                        var distanceToThePlayer = Vector2.Distance(enemyTransformPosition, playerTransformPosition);
                        if (distanceToThePlayer < _attack.DistanceToAttack)
                        {
                            _enemyMovementHandler.aiPath.maxSpeed = _enemyStatsManager.ChasingSpeed / 5;
                            _enemyAnimationController.SetAnimationSpeedTo(0.3f);

                            if (!_attack.AttackOnCooldown)
                            {
                                // Back chasing speed to normal
                                _enemyMovementHandler.aiPath.maxSpeed = _enemyStatsManager.ChasingSpeed;

                                // Verify if there is nothing between the his body and the player 
                                var raycastHit2D = Physics2D.Raycast(enemyTransformPosition,  (playerTransformPosition - enemyTransformPosition).normalized, distanceToThePlayer, obstaclesLayer);
                                var distanceToAttack = _attack.DistanceToAttack;
                                if (raycastHit2D.collider && !raycastHit2D.collider.CompareTag("Player"))
                                {
                                    // print("Collider: " + raycastHit2D.collider.gameObject.name);
                                    distanceToAttack -= distanceToAttack/10; // Shrink distance to the player to keep chasing 
                                    if (distanceToAttack < _attack.DistanceToAttack / 2) // Reset if get to this point
                                        distanceToAttack = _attack.DistanceToAttack;
                                }
                                else
                                {
                                    distanceToAttack = _attack.DistanceToAttack;
                                    ChangeState(States.PreparingAttack);
                                    break;
                                }
                            }
                               
                            
                        }
                        else
                        {
                            _enemyMovementHandler.aiPath.maxSpeed = _enemyStatsManager.MoveSpeed * _enemyStatsManager.ChasingSpeed;
                        }
                        
                        // Return to Standard state
                        if (Vector2.Distance(transform.position, playerTransformPosition) > _enemyStatsManager.DistanceToLosePlayerSight)
                        {
                            _enemyMovementHandler.TargetPlayer = null;
                        }
                    }
                    else // Lost sight of player
                    {
                        ChangeState(States.Standard);
                    }
                    break;
                
                case States.TakingKnockBack:
                    
                    
                    break;

                case States.PreparingAttack:
                    _attackPreparationCurrentTime += Time.deltaTime;
                    if (_attackPreparationCurrentTime > _attack.AttackPreparationTime)
                    {
                        _attackPreparationCurrentTime = 0;
                        ChangeState(States.Attacking);
                    }
                    else
                    {
                        PlayerDirection =  ((Vector2) _enemyMovementHandler.TargetPlayer.transform.position - _attack.AttackOrigin);
                        _enemyAnimationController.AnimateStanding(PlayerDirection.x, PlayerDirection.y); // makes the enemy look to the player while preparing the attack
                        _attack.PreparingAttack(); // keeps preparing the attack every frame until it can attack!
                    }
                    break;
                
                case States.Attacking:
                    // Return if already start the attack
                    if (isAttackingNow) return;
                    isAttackingNow = true;
                    // only once
                    _attack.Attack(PlayerDirection);
                    PlayerDirection = Vector3.zero;
                    break;

                case States.Dying:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
    
        public void ChangeState(States state)
        {
            previousState = this.State;
            switch (state)
            {
                case (States.Standard):
                    IsWalkingAround = true;
                    _enemyAnimationController.SetAnimationSpeedToDefault();
                    _enemyMovementHandler.TargetPlayer = null;
                    _enemyMovementHandler.aiPath.maxSpeed = _enemyStatsManager.MoveSpeed;
                    _enemyMovementHandler.aiDestinationSetter.target = _enemyMovementHandler.target.transform;
                    _enemyMovementHandler.fieldOfViewComponent.gameObject.SetActive(true);
                    _enemyMaterialManager.SetToDefaultMaterial();
                    break;

                case (States.Chasing):
                    // Making sure that it isn't frozen or paralyzed
                    if(_enemyMovementHandler.TargetPlayer) 
                        _enemyMovementHandler.aiDestinationSetter.target = _enemyMovementHandler.TargetPlayer.transform;
                    else 
                        ChangeState(States.Standard);
                    
                    _enemyMovementHandler.aiPath.enabled = true;
                    _enemyAnimationController.SetAnimationSpeedTo(1.2f);
                    IsWalkingAround = false;
                    _enemyMovementHandler.aiPath.maxSpeed = _enemyStatsManager.ChasingSpeed;
                    _enemyMovementHandler.fieldOfViewComponent.gameObject.SetActive(false);
                    AstarPath.active.Scan();
                    _enemyMaterialManager.SetToDefaultMaterial();
                    break;
                
                case (States.TakingKnockBack):
                    _enemyAnimationController.StopMoving();
                    IsWalkingAround = false;
                    _enemyMovementHandler.aiPath.maxSpeed = 0f;
                    _enemyMovementHandler.aiPath.enabled = false;
                    break;

                case (States.PreparingAttack):
                    if (_enemyRigidbody.bodyType == RigidbodyType2D.Static) return; // if it's dying
                    _enemyAnimationController.StopMoving();
                    IsWalkingAround = false;
                    _enemyRigidbody.velocity = Vector2.zero;
                    _enemyMovementHandler.fieldOfViewComponent.gameObject.SetActive(false);
                    _enemyMovementHandler.aiPath.enabled = false;
                    _enemyMovementHandler.aiDestinationSetter.target = null;
                    _enemyAnimationController.SetFlipAndFaceDirection(PlayerDirection.x, PlayerDirection.y);
                    _enemyMaterialManager.SetMaterial(_enemyMaterialManager.PreparingAttackMaterial);
                    _attackPreparationCurrentTime = 0f;
                    break;
                
                case (States.Attacking):
                    if ( _enemyRigidbody.bodyType == RigidbodyType2D.Static) return; // if it's dying
                    IsWalkingAround = false;
                    _enemyRigidbody.velocity = Vector2.zero;
                    _enemyMovementHandler.fieldOfViewComponent.gameObject.SetActive(false);
                    _enemyMovementHandler.aiPath.enabled = false;
                    _enemyMovementHandler.aiDestinationSetter.target = null;
                    break;
                
                case (States.Dying):
                    _enemyCombatManager.StopAllCoroutines();
                    IsWalkingAround = false;
                    isAttackingNow = false;
                    _isOnFire = false;
                    _enemyCombatManager.rigidbody2D.velocity = Vector2.zero;
                    _enemyCombatManager.rigidbody2D.bodyType = RigidbodyType2D.Static;
                    _enemyMovementHandler.protectorCollider.enabled = false; 
                    _enemyMovementHandler.boxCollider2D.enabled = false; 
                    _enemyAnimationController.SetAnimationSpeedTo(1);
                    _enemyMovementHandler.aiPath.canMove = false;
                    _enemyMovementHandler.aiPath.maxSpeed = 0;
                    _enemyMovementHandler.boxCollider2D.enabled = false;
                    if (_enemyMovementHandler.fieldOfViewComponent.gameObject != null)
                    {
                        _enemyMovementHandler.fieldOfViewComponent.gameObject?.SetActive(false);
                    }
                    IsDying = true;
                    
                    AudioManager.instance.Play("Final blow in the enemy");
                    shadow.SetActive(false);
                    _enemyMaterialManager.SetToDefaultMaterial();
                    _enemyAnimationController.TriggerDieAnimation(); // Animation will trigger the death effect
                    break; 
            }
            _state = state;
        }
        
        public IEnumerator ReturnEnemyToStateAfterSeconds(States state, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            ChangeState(state);
        }
        
        private void DropLoot()
        {
            _dropper.DropExp();
            // Dropper.DropItem();
        }
        
        public void EnemyDeath()
        {
            DropLoot();
            if (!_levelManager) return;
            _levelManager.enemiesRemaining -= 1;
            Destroy(_enemyMovementHandler.fieldOfViewComponent.gameObject);
            Destroy(_enemyMovementHandler.target.gameObject);
            Destroy(gameObject);
            Destroy(shadow);
        }
        
        private void OnDrawGizmos()
        {
            if (showDistanceToAttack  && _attack != null)
            {
                Gizmos.color = new Color(100, 0, 0, 0.1f);
                Gizmos.DrawSphere(transform.position, _attack.DistanceToAttack);
            }

            if (showLosePlayerDistance && _enemyStatsManager != null)
            {
                Gizmos.color = new Color(50, 50, 0, 0.1f);
                Gizmos.DrawSphere(transform.position, _enemyStatsManager.DistanceToLosePlayerSight);
            }
        }
    }
}