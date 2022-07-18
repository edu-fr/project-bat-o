using Pathfinding;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyMovementHandler : MonoBehaviour
    {
        // Components
        public Rigidbody2D Rigidbody;
        public BoxCollider2D BoxCollider2D;
        public BoxCollider2D ProtectorCollider;
        public AIDestinationSetter AiDestinationSetter;
        public AIPath AiPath;
        public EnemyStateMachine EnemyStateMachine;
        public EnemyAnimationController EnemyAnimationController;
        public EnemyStatsManager EnemyStats;

        // Movement
        public float CurrentTimer = 0f;

        [SerializeField] [Range(0f, 5f)] private float averageTimeWaitingToMove = 3f; // time to move to the next random spot
        private float _timeWaitingToMove; // time to move to the next random spot
        public float WalkingAroundSpeed { get; private set; }// walk speed
        public float ChasingSpeed { get; set; }
        public float DyingBurnedSpeed { get; private set; }= 4.5f; // running on fire speed
        private Vector3 HomePosition; // original position on the level
        [SerializeField] private float WalkableRange = 5f; // Distance it can walk while isn't chasing the player 
        public GameObject Target;

        // Searching for player
        public FieldOfView FieldOfViewComponent;
        public GameObject TargetPlayer { get; set; }
        private GameObject Player;
        private PlayerStateMachine PlayerStateMachine;
        public float SurroundingDistance = 2f;
        
        private Vector3 CurrentDirection;
        private float CurAngle;

        // Ally search
        // private float SearchForAlliesRange = 5f;
        
        [SerializeField]
        private LayerMask EnemiesLayer;
        
        
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerStateMachine = Player.GetComponent<PlayerStateMachine>();
           
            Target = new GameObject("Target of " + gameObject.name)
            {
                transform =
                {
                    parent = GameObject.FindGameObjectWithTag("EnemiesTargetsParent").transform,
                }
            };

            // Set the first random target movement 
            Target.transform.position = GenerateNewTarget();
            AiDestinationSetter.target = Target.transform;

            // config field of view component
            FieldOfViewComponent.SetFieldOfView(EnemyStats.FieldOfViewValue);
            FieldOfViewComponent.SetViewDistance(EnemyStats.FieldOfViewDistance);
            FieldOfViewComponent.SetMyMovementHandler(this);
            FieldOfViewComponent.SetOrigin(transform.position);
            WalkingAroundSpeed = EnemyStats.MoveSpeed;
            ChasingSpeed = EnemyStats.MoveSpeed * EnemyStats.ChasingSpeedMultiplier;
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(EnemyStateMachine.IsWalkingAround)
                WalkAround();
        }

        private Vector3 GenerateNewTarget()
        {
            _timeWaitingToMove = Random.Range(averageTimeWaitingToMove - 1.5f, averageTimeWaitingToMove + 1.5f);
            var position = transform.position;
            return new Vector3(position.x + (WalkableRange * Random.Range(0.3f, 1f) * Random.Range(-1, 2)), position.y + (WalkableRange * Random.Range(0.3f, 1f) * Random.Range(-1, 2)), position.z);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void RunFromThePlayer()
        {
            var oldTargetPosition =
                TargetPlayer != null ? TargetPlayer.transform.position : Target.transform.position;
            
            var newTarget = new Vector3(0, 0, 0);

            newTarget.x = transform.position.x > oldTargetPosition.x
                ? transform.position.x + 20
                : transform.position.x - 20;
            
            newTarget.y = transform.position.y > oldTargetPosition.y
                ? transform.position.y + 20
                : transform.position.y - 20;

            // Set A* target
            Target.transform.position = newTarget;
            
            // Active A*
            AstarPath.active.Scan();
        }
    
        public void WalkAround()
        {
            // Generate new target every maxTimer (three seconds)

            if (AiPath.reachedEndOfPath) //if reached desired location, wait some seconds and move to another
            {
                // Animate standing still
                EnemyAnimationController.StopMoving();
                
                CurrentTimer += Time.deltaTime;
                if (CurrentTimer >= _timeWaitingToMove)
                {
                    CurrentTimer = 0;
                    Random.InitState((int) Time.realtimeSinceStartup); // Randomize enemy standing still time
                    Target.transform.position = GenerateNewTarget();
                    AstarPath.active.Scan();
                }
            }
            else 
            {
                // Animate movement
                EnemyAnimationController.AnimateMovement(AiPath.desiredVelocity.x, AiPath.desiredVelocity.y);
            }
        }

        public void SetTargetPlayer(GameObject player)
        {
            this.TargetPlayer = player;
        }
   
       
        public void CheckSurroundings()
        {
            if (Player)
            {
                if(Vector2.Distance(Player.transform.position, this.transform.position) < SurroundingDistance)
                {
                    TargetPlayer = Player;
                }
            }
        }

        // public void SearchForAlliesActivityNearby() // if there is an ally nearby chasing the player, the object starts to follow the player too
        // {
        //     Collider2D[] alliesNearby = Physics2D.OverlapCircleAll(transform.position, SearchForAlliesRange, EnemiesLayer);
        //     foreach (Collider2D ally in alliesNearby)
        //     {
        //         
        //     if (myCircleCollider.Distance(ally.GetComponent<CircleCollider2D>()) < searchForAlliesRange)
        //     {
        //
        //     }
        //     
        //     }
        // }
    }
}
