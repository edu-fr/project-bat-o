using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts.Enemy
{
    public class EnemyMovementHandler : MonoBehaviour
    {
        // Components
        public Rigidbody2D Rigidbody { get; private set; }
        public BoxCollider2D BoxCollider2D { get; private set; }
        public AIDestinationSetter AiDestinationSetter { get; private set; }
        public AIPath AiPath { get; private set; }
        public EnemyStateMachine EnemyStateMachine { get; private set; }
        public EnemyAnimationController EnemyAnimationController { get; private set; }
        public EnemyStatsManager EnemyStats { get; private set; }
        
        // Movement
        public float CurrentTimer = 0f;

        private float MaxTimer = 3f; // time to move to the next random spot
        public float WalkingAroundSpeed { get; private set; } = 2; // walk speed
        public float ChasingSpeed { get; private set; } = 3.5f; // chasing speed
        public float DyingBurnedSpeed { get; private set; }= 4.5f; // running on fire speed
        private Vector3 HomePosition; // original position on the level
        [SerializeField]
        private float WalkableRange = 5f; // Distance it can walk while isn't chasing the player 
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
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            BoxCollider2D = GetComponent<BoxCollider2D>(); 
            AiDestinationSetter = GetComponent<AIDestinationSetter>();
            AiPath = GetComponent<AIPath>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            EnemyAnimationController = GetComponent<EnemyAnimationController>();
            EnemyStats = GetComponent<EnemyStatsManager>();
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerStateMachine = Player.GetComponent<PlayerStateMachine>();
            Target = new GameObject("Target of " + gameObject.name);
            // Target.transform.parent = transform;
            
            // Set initial enemy position according to its initial position
            HomePosition = Rigidbody.position;

            // Set the first random target movement 
            Target.transform.position = GenerateNewTarget();
            AiDestinationSetter.target = Target.transform;

            // config field of view component
            FieldOfViewComponent.SetFieldOfView(EnemyStats.FieldOfViewValue);
            FieldOfViewComponent.SetViewDistance(EnemyStats.FieldOfViewDistance);
            FieldOfViewComponent.SetMyMovementHandler(this);
            FieldOfViewComponent.SetOrigin(transform.position);
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(EnemyStateMachine.IsWalkingAround)
                WalkAround();
        }

        private Vector3 GenerateNewTarget()
        {
            return new Vector3(HomePosition.x + (WalkableRange * Random.Range(-1, 2)), HomePosition.y + (WalkableRange * Random.Range(-1, 2)), transform.position.z);
        }

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
                if (CurrentTimer >= MaxTimer)
                {
                    CurrentTimer = 0;
                    Random.InitState((int) Time.realtimeSinceStartup); // Randomize enemy standing still time
                    MaxTimer = Random.Range(1.5f, 4f);
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
