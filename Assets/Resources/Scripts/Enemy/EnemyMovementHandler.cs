using System;
using Game;
using Pathfinding;
using Resources.Scripts.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyMovementHandler : MonoBehaviour
    {
        // Components
        public Rigidbody2D Rigidbody;
        public BoxCollider2D BoxCollider2D;
        public AIDestinationSetter AiDestinationSetter;
        public AIPath AiPath;
        public EnemyStateMachine EnemyStateMachine;
        public Animator Animator; 

        // Movement
        public Path Path;
        public bool ReachedEndOfPath;
        public float CurrentTimer = 0f;

        private float MaxTimer = 3f; // time to move to the next random spot
        public float WalkingAroundSpeed = 2; // walk speed
        public float ChasingSpeed = 3.5f; // chasing speed
        public float DyingBurnedSpeed = 4.5f; // running on fire speed
        private Vector3 HomePosition; // original position on the level
        private float WalkableRange = 1f; // Distance it can walk while isnt chasing the player 
        public GameObject Target;
        public GameObject Shadow;

        // Searching for player
        public FieldOfView FieldOfViewComponent;
        public float FieldOfViewValue;
        public float ViewDistance;
        public GameObject TargetPlayer { get; set; }
        private GameObject Player;
        private PlayerStateMachine PlayerStateMachine;
        public float SurroundingDistance = 2f;

        
        public Material DefaultMaterial;
        public Material CurrentMaterial;
        public Material FlashMaterial;
        public Material BurnedMaterial;
        public Material FrozenMaterial;
        public Material ParalyzedMaterial;
        public Material TargetedMaterial;
        public Renderer Renderer;

        // Animation
        private Vector3 CurrentDirection;
        private float CurAngle;
        public Vector3 FaceDirection;
        
        // Health
        public EnemyHealthManager EnemyHealthManager;
        
        // Ally search
        // private float SearchForAlliesRange = 5f;
        private LayerMask EnemiesLayer;

        // Game Manager
        public LevelManager LevelManager;
        
        // Drop
        public Transform PrefabExperienceLoot;

        // Start is called before the first frame update
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerStateMachine = Player.GetComponent<PlayerStateMachine>();
            Target = new GameObject("target " + gameObject.name);
            
            // Set initial enemy position according to its initial position
            HomePosition = Rigidbody.position;

            // Set the first random target movement 
            Target.transform.position = GenerateNewTarget();
            AiDestinationSetter.target = Target.transform;

            // config field of view component
            FieldOfViewComponent.SetFieldOfView(FieldOfViewValue);
            FieldOfViewComponent.SetViewDistance(EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee ? ViewDistance : ViewDistance * 2); // Ranged enemies has twice the view distance than melee enemies
            FieldOfViewComponent.SetMyEnemyBehavior(this);
            FieldOfViewComponent.SetOrigin(transform.position);

            // Game Manager
            LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

            // Current sprite material
            DefaultMaterial = Renderer.material;
            CurrentMaterial = DefaultMaterial;

        }

        // Update is called once per frame
        private void Update()
        {
          
            UpdateMaterial();

           
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(EnemyStateMachine.IsWalkingAround)
                WalkAround();
        }

        private void DropLoot()
        {
            var position = transform.position;
            for (int i = 0; i < 25; i++)
            {
                LootScript.Create(position, 1);
            }
           
        }

        public void UpdateMaterial()
        {
            if (EnemyStateMachine.IsDying)
            {
                CurrentMaterial = DefaultMaterial;
            }
            else if (EnemyStateMachine.IsTargeted)
            {
                CurrentMaterial = TargetedMaterial;
            } 
            else if (EnemyStateMachine.IsOnFire)
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
            }
            Renderer.material = CurrentMaterial;
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


        public void DestroyObject()
        {
            if (!LevelManager) return;
            
            LevelManager.EnemiesRemaining -= 1;
            Destroy(FieldOfViewComponent.gameObject);
            Destroy(Target.gameObject);
            Destroy(gameObject);
            Destroy(Shadow);
        }
    }
}
