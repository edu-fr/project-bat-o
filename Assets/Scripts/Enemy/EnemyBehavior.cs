using System;
using System.Collections;
using Game;
using Objects;
using Pathfinding;
using Player;
using UnityEditor.U2D;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyBehavior : MonoBehaviour
    {
        
        // Components
        public Rigidbody2D Rigidbody;
        public CircleCollider2D CircleCollider;
        public Animator Animator;
        public AIDestinationSetter AiDestinationSetter;
        public AIPath AiPath;

        public EnemyStateMachine EnemyStateMachine;
        
        // Movement
        public Path Path;
        public bool ReachedEndOfPath;
        public float CurrentTimer = 0f;

        private float MaxTimer = 3f;            // time to move to the next random spot
        public float WalkingAroundSpeed = 2;    // walk speed
        public float ChasingSpeed = 3.5f;       // chasing speed
        public float DyingBurnedSpeed = 4.5f;   // running on fire speed
        private Vector3 HomePosition;           // original position on the level
        private float WalkableRange = 1f;       // Distance it can walk while isnt chasing the player 
        public GameObject Target;

        // Searching for player
        public Transform PreFabFieldOfView = null;
        public FieldOfView FieldOfViewComponent = null;
        public float FieldOfViewValue = 0;
        public float ViewDistance = 0;
        public GameObject TargetPlayer = null;
        public GameObject Player;
        public float SurroundingDistance = 2f;

        // Animation
        private Vector3 CurrentDirection;
        private float CurAngle;
        public Vector3 FaceDirection;
        
        public Material DefaultMaterial;
        public Material CurrentMaterial;
        public Material FlashMaterial;
        public Material BurnedMaterial;
        public Material FrozenMaterial;
        public Material ParalyzedMaterial;
        public Renderer Renderer;

        // Health
        private EnemyHealthManager EnemyHealthManager;
        
        // Ally search
        private float SearchForAlliesRange = 5f;
        private LayerMask EnemiesLayer;

        // Game Manager
        private GameManager GameManager;
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            CircleCollider = GetComponent<CircleCollider2D>();
            AiDestinationSetter = GetComponent<AIDestinationSetter>();
            AiPath = GetComponent<AIPath>();
            GetComponent<Seeker>();
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemiesLayer = LayerMask.GetMask("Enemies");
            Renderer = GetComponent<Renderer>();
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            Target = new GameObject("target " + gameObject.name);
            
            // Set initial enemy position according to its initial position
            HomePosition = Rigidbody.position;

            // Set the first random target movement 
            Target.transform.position = GenerateNewTarget();
            AiDestinationSetter.target = Target.transform;

            // Instantiate prefab field of view
            FieldOfViewComponent = Instantiate(PreFabFieldOfView, null).GetComponent<FieldOfView>();
            FieldOfViewComponent.gameObject.name = "Field of view" + gameObject.name;
            FieldOfViewComponent.SetFieldOfView(FieldOfViewValue);
            FieldOfViewComponent.SetViewDistance(EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee ? ViewDistance : ViewDistance * 2); // Ranged enemies has twice the view distance than melee enemies
            FieldOfViewComponent.SetMyEnemyBehavior(this);

            // Game Manager
            GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

            // Current sprite material
            DefaultMaterial = Renderer.material;
            CurrentMaterial = DefaultMaterial;

        }

        // Update is called once per frame
        private void Update()
        {
            // Verify if its alive
            if(EnemyHealthManager.GetCurrentHp() <= 0)
            {
                AudioManager.instance.Play("Final blow in the enemy");;
                GameManager.EnemiesRemaining -= 1;
                Destroy(gameObject);
                Destroy(FieldOfViewComponent.gameObject);
                Destroy(Target.gameObject);
            }
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(EnemyStateMachine.IsWalkingAround)
                WalkAround();
        }

        public void UpdateMaterial()
        {
            if (EnemyStateMachine.IsOnFire)
            {
                CurrentMaterial = BurnedMaterial;
            }
            else if (EnemyStateMachine.IsFrozen)
            {
                CurrentMaterial = FrozenMaterial;
            }
            else if (EnemyStateMachine.IsParalyzed)
            {
                CurrentMaterial = ParalyzedMaterial;
            }
            else
            {
                CurrentMaterial = DefaultMaterial;
                Renderer.material = CurrentMaterial;
            }
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

            if (AiPath.reachedEndOfPath) //if reached desired location, wait three seconds and move to another
            {
                // Animate standing still
                Animator.SetBool("IsMoving", false);
                CurrentTimer += Time.deltaTime;

                if (CurrentTimer >= MaxTimer)
                {
                    CurrentTimer = 0;
                    Target.transform.position = GenerateNewTarget();
                    AstarPath.active.Scan();
                }
            }
            else 
            {
                // Animate movement
                Animate();
            }
            SetCurrentFaceDirection();
        }

        public void SetTargetPlayer(GameObject player)
        {
            this.TargetPlayer = player;
        }
   
       
        public void CheckSurroundings()
        {
            if(Vector2.Distance(Player.transform.position, this.transform.position) < SurroundingDistance)
            {
                TargetPlayer = Player;
            }
        }

        public void SearchForAlliesActivityNearby() // if there is an ally nearby chasing the player, the object starts to follow the player too
        {
            Collider2D[] alliesNearby = Physics2D.OverlapCircleAll(transform.position, SearchForAlliesRange, EnemiesLayer);
            foreach (Collider2D ally in alliesNearby)
            {
                /*
            if (myCircleCollider.Distance(ally.GetComponent<CircleCollider2D>()) < searchForAlliesRange)
            {

            }
            */
            }
        }

        public void SetCurrentFaceDirection()
        {
            if (Animator.GetBool("IsMoving")) // just want to change the facing direction if the object is walking
            {
                CurrentDirection = AiPath.desiredVelocity;
            }
            CurAngle = UtilitiesClass.GetAngleFromVectorFloat(CurrentDirection);
            // Actual set of face direction
            FaceDirection = UtilitiesClass.GetDirectionFromAngle(CurAngle);
        }
        
        public void SetCurrentFaceDirectionTo(Vector3 desiredDirection)
        {
            CurAngle = UtilitiesClass.GetAngleFromVectorFloat(desiredDirection);
            // Actual set of face direction
            FaceDirection = UtilitiesClass.GetDirectionFromAngle(CurAngle);
            Animator.SetFloat("MoveX", FaceDirection.x);
            Animator.SetFloat("MoveY", FaceDirection.y);
        }

        public void Animate()
        {
            Animator.SetBool("IsMoving", true);
            Animator.SetFloat("MoveX", AiPath.velocity.x);
            Animator.SetFloat("MoveY", AiPath.velocity.y);
        }
    }
}
