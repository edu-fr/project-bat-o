using System.Collections;
using System.Linq.Expressions;
using Player;
using Unity.Collections;
using UnityEngine;

namespace Enemy
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

        private float AttackPreparationTime = 0.95f;
        private float AttackPreparationCurrentTime = 0;
        private float PreparationDistance = 1f;
        private float DistanceToAttack;
        private float DistanceToLosePlayerSight;
        
        private Vector3 PlayerDirection;

        public EnemyBehavior EnemyBehavior;
        public EnemyCombatManager EnemyCombatManager;
        public EnemyMeleeAttackManager EnemyMeleeAttackManager;
        public EnemyRangedAttackManager EnemyRangedAttackManager;
        
        private void Awake()
        {
            EnemyBehavior = GetComponent<EnemyBehavior>();
            EnemyCombatManager = GetComponent<EnemyCombatManager>();
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
            EnemyBehavior.UpdateMaterial();
            if (EnemyType == Type.Ranged)
            {
              //  Debug.Log("STATE: " + State);
            }
            switch (State)
            {
                case States.Standard:
                    IsWalkingAround = true;
                    // Checking if player is close
                    EnemyBehavior.CheckSurroundings();

                    // Updating field of view
                    EnemyBehavior.FieldOfViewComponent.SetAimDirection(EnemyBehavior.FaceDirection);
                    // Looking for the player
                    if (EnemyBehavior.TargetPlayer != null)
                    {
                        ChangeState(States.Chasing);
                        //Debug.Log("LostTargetPlayer");
                    }

                    break;

                case States.Chasing:
                    // Verify if there is close enemies chasing the player

                    if (EnemyBehavior.TargetPlayer != null) // Know where the player is
                    {
                        EnemyBehavior.Animate();
                        EnemyBehavior.SetCurrentFaceDirection();

                        // If enemy is in a certain distance to the player
                        if (Vector2.Distance(transform.position, EnemyBehavior.TargetPlayer.transform.position) < DistanceToAttack)
                        {
                            ChangeState(States.PreparingAttack);
                            //Debug.Log("PreparingAttack");
                            break;
                        }

                        // Return to Standard state
                        if (Vector2.Distance(transform.position, EnemyBehavior.TargetPlayer.transform.position) > DistanceToLosePlayerSight)
                        {
                            EnemyBehavior.TargetPlayer = null;
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
                            EnemyBehavior.Rigidbody.AddForce(-PlayerDirection * PreparationDistance, ForceMode2D.Force);
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
                    EnemyBehavior.Animate();
                    EnemyBehavior.SetCurrentFaceDirection();
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
                    EnemyBehavior.Animator.speed = 1;
                    EnemyBehavior.TargetPlayer = null;
                    EnemyBehavior.AiPath.maxSpeed = EnemyBehavior.WalkingAroundSpeed;
                    EnemyBehavior.AiDestinationSetter.target = EnemyBehavior.Target.transform;
                    EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(true);
                    break;

                case (States.Chasing):
                    // Making sure that it isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    EnemyBehavior.Animator.speed = 1.2f;
                    IsWalkingAround = false;
                    EnemyBehavior.AiPath.maxSpeed = EnemyBehavior.ChasingSpeed;
                    EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyBehavior.AiDestinationSetter.target = EnemyBehavior.TargetPlayer.transform;
                    AstarPath.active.Scan();
                    break;

                case (States.PreparingAttack):
                    EnemyBehavior.Animator.SetBool("IsMoving", false);
                    IsWalkingAround = false;
                    EnemyBehavior.Rigidbody.velocity = Vector2.zero;
                    EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyBehavior.AiPath.enabled = false;
                    EnemyBehavior.AiDestinationSetter.target = null;
                    PlayerDirection = ((Vector2) EnemyBehavior.TargetPlayer.transform.position + (Vector2) EnemyBehavior.TargetPlayer.GetComponent<BoxCollider2D>().offset - (Vector2) transform.position).normalized;
                    EnemyBehavior.SetCurrentFaceDirectionTo(PlayerDirection);
                    break;
                
                case (States.Attacking):
                    IsWalkingAround = false;
                    EnemyBehavior.Rigidbody.velocity = Vector2.zero;
                    EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyBehavior.AiPath.enabled = false;
                    EnemyBehavior.AiDestinationSetter.target = null;
                    break;
                
                case (States.DyingBurned):
                    // Making sure that isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    IsWalkingAround = false;
                    EnemyBehavior.AiPath.maxSpeed = EnemyBehavior.DyingBurnedSpeed;
                    
                    if (EnemyBehavior.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                   
                    EnemyBehavior.RunFromThePlayer();
                    EnemyBehavior.Animator.speed = 1.5f;
                    EnemyBehavior.AiDestinationSetter.target = EnemyBehavior.Target.transform;
                    break;
                
                case (States.Frozen):
                    IsWalkingAround = false;
                    IsFrozen = true;
                    EnemyBehavior.Animator.speed = 0;
                    EnemyBehavior.AiPath.maxSpeed = 0;
                   
                    if (EnemyBehavior.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    break;
                
                case (States.Paralyzed):
                    IsWalkingAround = false;
                    IsParalyzed = true;
                    EnemyBehavior.Animator.speed = 0;
                    EnemyBehavior.AiPath.maxSpeed = 0;
                    
                    if (EnemyBehavior.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    break;
                
                case (States.BeenRushed):
                    IsWalkingAround = false;
                    IsBeenRushed = true;
                    EnemyCombatManager.Rigidbody2D.velocity = Vector2.zero;
                    EnemyBehavior.Animator.speed = 0;
                    EnemyBehavior.AiPath.maxSpeed = 0;
                    if (EnemyBehavior.FieldOfViewComponent.gameObject != null)
                    {
                        EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    
                    break;
            }
            this.State = state;
        }

        public IEnumerator ReturnEnemyToStateAfterSeconds(States state, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            ChangeState(state);
        }
    }
}