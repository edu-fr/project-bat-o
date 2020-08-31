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
        

        public bool IsFrozen = false;
        public float DefrostCurrentTimer;
        public float DefrostTime;
        public bool IsOnFire = false;
        public bool IsPrimaryTarget = false;
        public bool IsParalyzed = false;
        public float ParalyzeHealCurrentTimer;
        public float ParalyzeHealTime;
        public bool WillDieBurned = false;
        public bool IsAttacking;

        private float AttackPreparationTime = 0.95f;
        private float AttackPreparationCurrentTime = 0;
        private float PreparationDistance = 1f;

        private Vector3 PlayerDirection;

        public EnemyBehavior EnemyBehavior;
        public EnemyMeleeAttackManager EnemyMeleeAttackManager;
        public EnemyRangedAttackManager EnemyRangedAttackManager;
        
        private void Awake()
        {
            EnemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void Start()
        {
            EnemyType = EnemyMeleeAttackManager != null ? Type.Melee : Type.Ranged;
            State = States.Standard;
        }

        private void Update()
        {
            EnemyBehavior.UpdateMaterial();
            switch (State)
            {
                case States.Standard:
                    IsWalkingAround = true;
                    // Checking if player is close
                    EnemyBehavior.CheckSurroundings();

                    // Updating field of view
                    EnemyBehavior.FieldOfViewComponent.SetAimDirection(EnemyBehavior.FaceDirection);
                    EnemyBehavior.FieldOfViewComponent.SetOrigin(transform.position);
                    // Looking for the player
                    if (EnemyBehavior.TargetPlayer.gameObject != null)
                    {
                        ChangeState(States.Chasing);
                    }

                    break;

                case States.Chasing:
                    // Verify if there is close enemies chasing the player

                    if (EnemyBehavior.TargetPlayer != null) // Know where the player is
                    {
                        EnemyBehavior.Animate();
                        EnemyBehavior.SetCurrentFaceDirection();

                        // If enemy is in a certain distance to the player
                        if (Vector2.Distance(transform.position, EnemyBehavior.TargetPlayer.transform.position) < 1.3f)
                        {
                            ChangeState(States.PreparingAttack);
                            break;
                        }

                        // Return to Standard state
                        if (Vector2.Distance(transform.position, EnemyBehavior.TargetPlayer.transform.position) > 7f)
                        {
                            EnemyBehavior.TargetPlayer = null;
                            Debug.Log("ANULEI O PLAYER");
                        }
                    }
                    else // Lost sight of player
                    {
                        Debug.Log("PERDI VISAO DO PLAYER");
                        ChangeState(States.Standard);
                    }

                    break;

                case States.PreparingAttack:
                    
                    AttackPreparationCurrentTime += Time.deltaTime;
                    
                    if (AttackPreparationCurrentTime > AttackPreparationTime)
                    {
                        AttackPreparationCurrentTime = 0;
                        ChangeState(States.Attacking);
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
                    if (IsAttacking) return;
                    
                    IsAttacking = true; 
                    
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
                    }

                    break;

                case States.Paralyzed:
                    ParalyzeHealCurrentTimer += Time.deltaTime;
                    if (ParalyzeHealCurrentTimer >= ParalyzeHealTime)
                    {
                        ParalyzeHealCurrentTimer = 0;
                        IsParalyzed = false;
                        ChangeState(States.Chasing);
                    }

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
                    IsWalkingAround = false;
                    EnemyBehavior.Rigidbody.velocity = Vector2.zero;
                    EnemyBehavior.FieldOfViewComponent.gameObject.SetActive(false);
                    EnemyBehavior.AiPath.enabled = false;
                    EnemyBehavior.AiDestinationSetter.target = null;
                    PlayerDirection = (EnemyBehavior.TargetPlayer.transform.position - transform.position).normalized;
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
            }
            this.State = state;
        }
    }
}