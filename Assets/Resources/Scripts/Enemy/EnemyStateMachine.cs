using System.Collections;
using Game;
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

        public enum Type
        {
            Ranged,
            Melee
        };

        public States State;
        public States PreviousState;
        public Type EnemyType;
        
        public bool IsWalkingAround = false;

        public bool IsDying = false;
        public bool IsTargeted = false;
        public bool IsFrozen = false;
        public float DefrostCurrentTimer;
        public float DefrostTime;
        public bool IsOnFire = false;
        public bool IsPrimaryTarget = false;
        public bool IsParalyzed = false;
        public float ParalyzeHealCurrentTimer;
        public float ParalyzeHealTime;
        public bool WillDieBurned = false;
        public bool IsAttackingNow;
        public bool IsBeenRushed;

        [SerializeField]
        private float AttackPreparationTime = 0.95f;
        private float AttackPreparationCurrentTime = 0;
        private float PreparationDistance = 1f;
        private float DistanceToAttack;
        private float DistanceToLosePlayerSight;
        
        private Vector3 PlayerDirection;

        private EnemyMovementHandler EnemyMovementHandler;
        public EnemyCombatManager EnemyCombatManager { get; private set; }
        public EnemyMeleeAttackManager EnemyMeleeAttackManager { get; private set; }
        public EnemyRangedAttackManager EnemyRangedAttackManager { get; private set; }
        public EnemyMaterialManager EnemyMaterialManager { get; private set; }
        // public EnemyStatsManager EnemyStatsManager { get; private set; }

        private LevelManager LevelManager;
        private Renderer Renderer;
        public GameObject Shadow;
        private void Awake()
        {
            EnemyMovementHandler = GetComponent<EnemyMovementHandler>();
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
            EnemyMeleeAttackManager = GetComponent<EnemyMeleeAttackManager>();
            EnemyRangedAttackManager = GetComponent<EnemyRangedAttackManager>();
            EnemyMaterialManager = GetComponent<EnemyMaterialManager>();
            // EnemyStatsManager = GetComponent<EnemyStatsManager>();
            Renderer = GetComponent<Renderer>();
            
            // Game Manager
            LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }

        private void Start()
        {
            EnemyType = EnemyMeleeAttackManager != null ? Type.Melee : Type.Ranged;
            switch (EnemyType)
            {
                case Type.Melee:
                    DistanceToAttack = 1.3f;
                    DistanceToLosePlayerSight = 9f;
                    break;
                
                case Type.Ranged:
                    DistanceToAttack = 6f;
                    DistanceToLosePlayerSight = 10f;
                    break;
                
            }
            State = States.Standard;
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
                    EnemyMovementHandler.FieldOfViewComponent.SetAimDirection(EnemyMovementHandler.FaceDirection);
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
                        EnemyMovementHandler.Animate();
                        EnemyMovementHandler.SetCurrentFaceDirection();

                        // If enemy is in a certain distance to the player
                        if (Vector2.Distance(transform.position, EnemyMovementHandler.TargetPlayer.transform.position) < DistanceToAttack)
                        {
                            ChangeState(States.PreparingAttack);
                            //Debug.Log("PreparingAttack");
                            break;
                        }

                        // Return to Standard state
                        if (Vector2.Distance(transform.position, EnemyMovementHandler.TargetPlayer.transform.position) > DistanceToLosePlayerSight)
                        {
                            EnemyMovementHandler.TargetPlayer = null;
                        }
                    }
                    else // Lost sight of player
                    {
                        ChangeState(States.Standard);
                        //Debug.Log("Lost player on attack");
                    }
                    break;

                case States.PreparingAttack:
                    AttackPreparationCurrentTime += Time.deltaTime;
                    
                    if (AttackPreparationCurrentTime > AttackPreparationTime)
                    {
                        AttackPreparationCurrentTime = 0;
                        ChangeState(States.Attacking);
                        //Debug.Log("FinishedReadingAttack");
                    }
                    else
                    {
                        if (EnemyType == Type.Melee)
                        {
                            // taking distance before dashing
                            EnemyMovementHandler.Rigidbody.AddForce(-PlayerDirection * PreparationDistance, ForceMode2D.Force);
                        }
                        else //ranged 
                        {
                            // make noise to warning about attack
                        }
                    }
                    break;
                
                case States.Attacking:
                    // Return if already start the attack
                    if (IsAttackingNow) return;
                    
                    IsAttackingNow = true; 
                    
                    // do only once
                    if (EnemyType == Type.Melee)
                    {
                        EnemyMeleeAttackManager.Attack(PlayerDirection);
                    }
                    else
                    {
                        EnemyRangedAttackManager.Attack(PlayerDirection);
                    }
                    break;

                case States.DyingBurned:
                    EnemyMovementHandler.Animate();
                    EnemyMovementHandler.SetCurrentFaceDirection();
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
                    EnemyMovementHandler.Animator.speed = 1;
                    EnemyMovementHandler.TargetPlayer = null;
                    EnemyMovementHandler.AiPath.maxSpeed = EnemyMovementHandler.WalkingAroundSpeed;
                    EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.Target.transform;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(true);
                    break;

                case (States.Chasing):
                    // Making sure that it isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    EnemyMovementHandler.Animator.speed = 1.2f;
                    IsWalkingAround = false;
                    EnemyMovementHandler.AiPath.maxSpeed = EnemyMovementHandler.ChasingSpeed;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    if(EnemyMovementHandler.Target) EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.TargetPlayer.transform;
                    AstarPath.active.Scan();
                    break;

                case (States.PreparingAttack):
                    EnemyMovementHandler.Animator.SetBool("IsMoving", false);
                    IsWalkingAround = false;
                    EnemyMovementHandler.Rigidbody.velocity = Vector2.zero;
                    EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyMovementHandler.AiPath.enabled = false;
                    EnemyMovementHandler.AiDestinationSetter.target = null;
                    PlayerDirection = ((Vector2) EnemyMovementHandler.TargetPlayer.transform.position + (Vector2) EnemyMovementHandler.TargetPlayer.GetComponent<BoxCollider2D>().offset - (Vector2) transform.position).normalized;
                    EnemyMovementHandler.SetCurrentFaceDirectionTo(PlayerDirection);
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
                    EnemyMovementHandler.Animator.speed = 1.5f;
                    EnemyMovementHandler.AiDestinationSetter.target = EnemyMovementHandler.Target.transform;
                    break;
                
                case (States.Frozen):
                    IsWalkingAround = false;
                    IsFrozen = true;
                    EnemyMovementHandler.Animator.speed = 0;
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                   
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    break;
                
                case (States.Paralyzed):
                    IsWalkingAround = false;
                    IsParalyzed = true;
                    EnemyMovementHandler.Animator.speed = 0;
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                    
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    break;
                
                case (States.BeenRushed):
                    IsWalkingAround = false;
                    IsBeenRushed = true;
                    EnemyCombatManager.Rigidbody2D.velocity = Vector2.zero;
                    EnemyMovementHandler.Animator.speed = 0;
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
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
                    EnemyMovementHandler.Animator.speed = 1;
                    EnemyMovementHandler.AiPath.maxSpeed = 0;
                    EnemyMovementHandler.BoxCollider2D.enabled = false;
                    if (EnemyMovementHandler.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyMovementHandler.FieldOfViewComponent.gameObject?.SetActive(false);
                    }
                    IsDying = true;
                    
                    AudioManager.instance.Play("Final blow in the enemy");
                    Shadow.SetActive(false);
                    Renderer.material = EnemyMaterialManager.DefaultMaterial;
                    // Animator.SetTrigger("Died");
                    DropLoot();
                    
                    break; 
            }
            this.State = state;
        }
        
        public IEnumerator ReturnEnemyToStateAfterSeconds(States state, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            ChangeState(state);
        }
        
        private void DropLoot()
        {
            var position = transform.position;
            for (var i = 0; i < 25 /* EnemyStatsManager.ExpDropQuantity */; i++)
            {
                LootScript.Create(position, 1);
            }
        }
        
        public void DestroyEnemyObjects()
        {
            if (!LevelManager) return;
            
            LevelManager.EnemiesRemaining -= 1;
            Destroy(EnemyMovementHandler.FieldOfViewComponent.gameObject);
            Destroy(EnemyMovementHandler.Target.gameObject);
            Destroy(gameObject);
            Destroy(Shadow);
        }
    }
}