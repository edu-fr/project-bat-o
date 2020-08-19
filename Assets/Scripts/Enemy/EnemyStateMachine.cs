using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public enum States
        {
            Standard,
            Chasing,
            Attacking,
            DyingBurned,
            Frozen,
            Paralyzed
        };
        
        public States State;
        public States PreviousState;
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
        
        public EnemyBehavior EnemyBehavior;
        
        private void Awake()
        {
            EnemyBehavior = GetComponent<EnemyBehavior>();
        }

        private void Start()
        {
            State = States.Standard;
        }

        private void Update()
        {
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

                        // Return to Standard state
                        if (Vector2.Distance(transform.position, EnemyBehavior.TargetPlayer.transform.position) > 7f)
                        {
                            EnemyBehavior.TargetPlayer = null;
                        }
                    }
                    else // Lost sight of player
                    {
                        ChangeState(States.Standard);
                    }

                    break;

                case States.Attacking:

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
                        if (PreviousState == States.Frozen || PreviousState == States.Paralyzed)
                        {
                            ChangeState(States.Chasing);
                        }
                        else
                        {
                            ChangeState(PreviousState);
                        }

                        IsFrozen = false;
                    }

                    break;

                case States.Paralyzed:
                    ParalyzeHealCurrentTimer += Time.deltaTime;
                    if (ParalyzeHealCurrentTimer >= ParalyzeHealTime)
                    {
                        ParalyzeHealCurrentTimer = 0;
                        if (PreviousState == States.Frozen || PreviousState == States.Paralyzed)
                        {
                            ChangeState(States.Chasing);
                        }
                        else
                        {
                            ChangeState(PreviousState);
                        }

                        IsParalyzed = false;
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

                case (States.Attacking):
                    
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