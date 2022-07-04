using System.Collections;
using Game;
using Pathfinding;
using Resources.Project.Runtime.Scripts.Enemy;
using Resources.Project.Runtime.Scripts.Game;
using Resources.Scripts.Enemy.Attacks;
using Resources.Scripts.Objects;
using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public enum States
        {
            Standard,
            Chasing,
            PreparingAttack,
            Attacking,
            DyingBurned,
            Frozen,
            Paralyzed,
            BeenRushed,
            Dying
        };

        public States State;
        public States PreviousState;
        
        public bool IsWalkingAround { get; private set; } = false;

        public bool IsDying { get; private set; } = false;
        public bool IsTargeted { get; set; } = false;
        public bool IsFrozen { get; private set; } = false;
        public float DefrostCurrentTimer { get; set; }
        public float DefrostTime { get; set; } 
        public bool IsOnFire { get; set; } = false;
        public bool IsPrimaryTarget { get; set; } = false;
        public bool IsParalyzed { get; private set; } = false;
        public float ParalyzeHealCurrentTimer { get; set; } 
        public float ParalyzeHealTime { get; set; } 
        public bool WillDieBurned { get; set; } = false;
        public bool IsAttackingNow { get; set; } 
        public bool IsBeenRushed { get; private set; }
        
        public float AttackPreparationCurrentTime { get; private set; }
        private float DistanceToAttack; // Reference to stats manager
        private float DistanceToLosePlayerSight; // Reference to stats manager
        
        public Vector3 PlayerDirection { get; private set;}

        private EnemyMovementHandler EnemyMovementHandler;
        public EnemyCombatManager EnemyCombatManager { get; private set; }
        public EnemyStatsManager EnemyStatsManager { get; private set; }
        
        public AIPath AiPath { get; private set; }
        public BaseAttack BaseAttack { get; private set; }
        public EnemyMaterialManager EnemyMaterialManager { get; private set; }
        
        public EnemyAnimationController EnemyAnimationController { get; private set; }
        
        private LevelManager LevelManager;
        private Renderer Renderer;
        public GameObject Shadow;
        public LayerMask ObstaclesLayer;
        private void Awake()
        {
            EnemyMovementHandler = GetComponent<EnemyMovementHandler>();
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            BaseAttack = GetComponent<BaseAttack>();
            EnemyMaterialManager = GetComponent<EnemyMaterialManager>();
            EnemyStatsManager = GetComponent<EnemyStatsManager>();
            Renderer = GetComponent<Renderer>();
            EnemyAnimationController = GetComponent<EnemyAnimationController>();
            AiPath = GetComponent<AIPath>();
            // Game Manager
            LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }

        private void Start()
        {
            DistanceToAttack = EnemyStatsManager.DistanceToAttack;
            DistanceToLosePlayerSight = EnemyStatsManager.DistanceToLosePlayerSight;
        }

        private void Update()
        {
            switch (State)
            {
                case States.Standard:
                    IsWalkingAround = true;
                    // Checking if player is close
                    EnemyMovementHandler.CheckSurroundings();

                    // Updating field of view
                    EnemyMovementHandler.FieldOfViewComponent.SetAimDirection(EnemyMovementHandler.EnemyAnimationController.CurrentFaceDirection);
                    // Looking for the player
                    if (EnemyMovementHandler.TargetPlayer != null)
                    {
                        ChangeState(States.Chasing);
                        //Debug.Log("LostTargetPlayer");
                    }

                    break;

                case States.Chasing:
                    // Verify if there is close enemies chasing the player
                    if (EnemyMovementHandler.TargetPlayer != null) // Know where the player is
                    {
                        var playerTransformPosition = EnemyMovementHandler.TargetPlayer.transform.position;
                        var enemyTransformPosition = transform.position;
                        EnemyAnimationController.AnimateMovement(playerTransformPosition.x - enemyTransformPosition.x, playerTransformPosition.y - enemyTransformPosition.y);

                        // If enemy is in a certain distance to the player
                        if (Vector2.Distance(transform.position, playerTransformPosition) < DistanceToAttack)
                        {
                            EnemyMovementHandler.AiPath.maxSpeed = EnemyStatsManager.MoveSpeed / 3;

                            if (!BaseAttack.AttackOnCooldown)
                            {
                                // Back chasing speed to normal
                                EnemyMovementHandler.AiPath.maxSpeed = EnemyStatsManager.MoveSpeed * EnemyStatsManager.ChasingSpeedMultiplier;

                                // Verify if there is nothing between the his body and the player
                                var raycastHit2D = Physics2D.Raycast(enemyTransformPosition,  (playerTransformPosition - enemyTransformPosition).normalized, EnemyStatsManager.AttackSpeed, ObstaclesLayer);
                                if (raycastHit2D.collider)
                                {
                                    DistanceToAttack -= DistanceToAttack/10;
                                    if (DistanceToAttack < AiPath.slowdownDistance)
                                        DistanceToAttack = EnemyStatsManager.DistanceToAttack;
                                }
                                else
                                {
                                    DistanceToAttack = EnemyStatsManager.DistanceToAttack;
                                    ChangeState(States.PreparingAttack);
                                    break;
                                }
                            }
                               
                            
                        }
                        else
                        {
                            EnemyMovementHandler.AiPath.maxSpeed = EnemyStatsManager.MoveSpeed * EnemyStatsManager.ChasingSpeedMultiplier;
                        }
                        
                        // Return to Standard state
                        if (Vector2.Distance(transform.position, playerTransformPosition) > DistanceToLosePlayerSight)
                        {
                            EnemyMovementHandler.TargetPlayer = null;
                        }
                    }
                    else // Lost sight of player
                    {
                        ChangeState(States.Standard);
                    }
                    break;

                case States.PreparingAttack:
                    AttackPreparationCurrentTime += Time.deltaTime;
                    if (AttackPreparationCurrentTime > EnemyStatsManager.AttackPreparationTime)
                    {
                        AttackPreparationCurrentTime = 0;
                        ChangeState(States.Attacking);
                    }
                    else
                    {
                        PlayerDirection =  ((Vector2) EnemyMovementHandler.TargetPlayer.transform.position - BaseAttack.AttackOrigin);
                        EnemyAnimationController.AnimateStanding(PlayerDirection.x, PlayerDirection.y); // makes the enemy look to the player while preparing the attack
                        BaseAttack.PreparingAttack(); // keeps preparing the attack every frame until it can attack!
                    }
                    break;
                
                case States.Attacking:
                    // Return if already start the attack
                    if (IsAttackingNow) return;
                    IsAttackingNow = true;
                    // only once
                    BaseAttack.Attack(PlayerDirection);
                    PlayerDirection = Vector3.zero;
                    break;
                    
                case States.DyingBurned:
                    EnemyAnimationController.AnimateMovement(EnemyMovementHandler.AiPath.desiredVelocity.x, EnemyMovementHandler.AiPath.desiredVelocity.y);
                    break;

                case States.Frozen:
                    DefrostCurrentTimer += Time.deltaTime;

                    if (DefrostCurrentTimer >= DefrostTime)
                    {
                        DefrostCurrentTimer = 0;
                        IsFrozen = false;
                        ChangeState(States.Chasing);
                        //Debug.Log("Frozen Ends");
                    }
                    break;

                case States.Paralyzed:
                    ParalyzeHealCurrentTimer += Time.deltaTime;
                    if (ParalyzeHealCurrentTimer >= ParalyzeHealTime)
                    {
                        ParalyzeHealCurrentTimer = 0;
                        IsParalyzed = false;
                        ChangeState(States.Chasing);
                        //Debug.Log("Paralyze ends");
                    }
                    break;
                
                case States.BeenRushed:
                    /* StartCoroutine(ReturnEnemyToStandardStateAfterSeconds(3f)); */
                    break; 
                
                case States.Dying:
                    
                    break; 
            }
        }
    
    
        public void ChangeState(States state)
        {
            PreviousState = this.State;
            switch (state)
            {
                case (States.Standard):
                    // Making sure that is not frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    IsWalkingAround = true;
                    EnemyAnimationController.SetAnimationSpeedToDefault();
                    EnemyMovementHandler.TargetPlayer = null;
                    EnemyMovementHandler.AiPath.maxSpeed = EnemyMovementHandler.WalkingAroundSpeed;
                    EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.Target.transform;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(true);
                    EnemyMovementHandler.ChasingSpeed = EnemyStatsManager.MoveSpeed * EnemyStatsManager.ChasingSpeedMultiplier;
                    EnemyMaterialManager.SetToDefaultMaterial();
                    break;

                case (States.Chasing):
                    // Making sure that it isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    EnemyAnimationController.SetAnimationSpeedTo(1.2f);
                    IsWalkingAround = false;
                    EnemyMovementHandler.AiPath.maxSpeed = EnemyMovementHandler.ChasingSpeed;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    if(EnemyMovementHandler.TargetPlayer) EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.TargetPlayer.transform;
                    AstarPath.active.Scan();
                    EnemyMaterialManager.SetToDefaultMaterial();
                    break;

                case (States.PreparingAttack):
                    EnemyAnimationController.StopMoving();
                    IsWalkingAround = false;
                    EnemyMovementHandler.Rigidbody.velocity = Vector2.zero;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyMovementHandler.AiPath.enabled = false;
                    EnemyMovementHandler.AiDestinationSetter.target = null;
                    EnemyAnimationController.SetFlipAndFaceDirection(PlayerDirection.x, PlayerDirection.y);
                    EnemyMaterialManager.SetMaterial(EnemyMaterialManager.PreparingAttackMaterial);
                    break;
                
                case (States.Attacking):
                    IsWalkingAround = false;
                    EnemyMovementHandler.Rigidbody.velocity = Vector2.zero;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyMovementHandler.AiPath.enabled = false;
                    EnemyMovementHandler.AiDestinationSetter.target = null;
                    break;
                
                case (States.DyingBurned):
                    // Making sure that isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    IsWalkingAround = false;
                    EnemyMovementHandler.AiPath.maxSpeed = EnemyMovementHandler.DyingBurnedSpeed;
                    
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    EnemyMovementHandler.RunFromThePlayer();
                    EnemyAnimationController.SetAnimationSpeedTo(1.5f);
                    EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.Target.transform;
                    
                    EnemyMaterialManager.SetMaterial(EnemyMaterialManager.BurnedMaterial);
                    break;
                
                case (States.Frozen):
                    IsWalkingAround = false;
                    IsFrozen = true;
                    EnemyAnimationController.SetAnimationSpeedTo(0);
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                   
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    EnemyMaterialManager.SetMaterial(EnemyMaterialManager.FrozenMaterial);
                    break;
                
                case (States.Paralyzed):
                    IsWalkingAround = false;
                    IsParalyzed = true;
                    EnemyAnimationController.SetAnimationSpeedTo(0);
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                    
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    
                    EnemyMaterialManager.SetMaterial(EnemyMaterialManager.ParalyzedMaterial);
                    break;

                case (States.Dying):
                    IsWalkingAround = false;
                    IsBeenRushed = false;
                    IsFrozen = false;
                    IsParalyzed = false;
                    IsTargeted = false;
                    IsAttackingNow = false;
                    IsBeenRushed = false;
                    IsOnFire = false;
                    IsPrimaryTarget = false;
                    EnemyCombatManager.Rigidbody2D.velocity = Vector2.zero;
                    EnemyAnimationController.SetAnimationSpeedTo(1);
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                    EnemyMovementHandler.BoxCollider2D.enabled = false;
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject?.SetActive(false);
                    }
                    IsDying = true;
                    
                    AudioManager.instance.Play("Final blow in the enemy");
                    Shadow.SetActive(false);
                    EnemyMaterialManager.SetToDefaultMaterial();
                    EnemyAnimationController.TriggerDieAnimation(); // Animation will trigger the death effect
                    break; 
            }
            this.State = state;
        }
        
        public IEnumerator ReturnEnemyToStateAfterSeconds(States state, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            ChangeState(state);
        }
        
        public void DropLoot()
        {
            var position = transform.position;
            for (var i = 0; i < EnemyStatsManager.ExpDropQuantity; i++)
            {
                LootScript.Create(position, 1);
            }
        }
        
        public void EnemyDeath()
        {
            DropLoot();
            if (!LevelManager) return;
            LevelManager.EnemiesRemaining -= 1;
            Destroy(EnemyMovementHandler.FieldOfViewComponent.gameObject);
            Destroy(EnemyMovementHandler.Target.gameObject);
            Destroy(gameObject);
            Destroy(Shadow);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(100, 0, 0, 0.1f);
            Gizmos.DrawSphere(transform.position, DistanceToAttack);
            // Gizmos.color = new Color(50, 50, 0, 0.1f);
            // Gizmos.DrawSphere(transform.position, DistanceToLosePlayerSight);
            
        }
    }
}