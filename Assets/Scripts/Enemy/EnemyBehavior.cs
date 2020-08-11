using System;
using System.Collections;
using Game;
using Objects;
using Pathfinding;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyBehavior : MonoBehaviour
    {
        #region Enums

        private enum States {Standard, Chasing, Attacking};

        #endregion

        #region Variables
        // Components
        private Rigidbody2D Rigidbody;
        private CircleCollider2D CircleCollider;
        private Animator Animator;
        private AIDestinationSetter AiDestinationSetter;
        private Seeker Seeker;
        private AIPath AiPath;
        private Renderer Renderer;
        private Material DefaultMaterial;
        [SerializeField]
        public Material FlashMaterial;


        // Movement
        private Path Path;
        private bool ReachedEndOfPath;
        private float CurrentTimer = 0f;

        private float MaxTimer = 3f;                // time to move to the next random spot
        // private float speed = 160f;         // walk speed
        private Vector3 HomePosition;       // original position on the level
        private float WalkableRange = 1f;   // Distance it can walk while isnt chasing the player 
        public GameObject Target;

   

        // Searching for player
        [SerializeField] private Transform PreFabFieldOfView = null;
        private FieldOfView FieldOfViewComponent = null;
        [SerializeField] private float FieldOfViewValue = 0;
        [SerializeField] private float ViewDistance = 0;
        [SerializeField] private GameObject TargetPlayer = null;
        private GameObject Player;
        [SerializeField] private float SurroundingDistance = 2f;

        // Animation
        private Vector3 CurrentDirection;
        private float CurAngle;
        private Vector3 FaceDirection;

        // State machine
        private States State;
        private bool IsWalkingAround = false;
        
        public bool IsFrozen = false;
        public bool IsOnFire = false;
        public bool IsPrimaryTarget = false;
        public bool IsParalyzed = false;
        

        // Health
        private EnemyHealthManager EnemyHealthManager;
        
        // Ally search
        private float SearchForAlliesRange = 5f;
        private LayerMask EnemiesLayer;

        // Attack
        private float Damage = 20;
        private bool Invincible = false;
        private float TimeInvencible = .9f;
        
        // Game Manager
        private GameManager GameManager;
        
        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            CircleCollider = GetComponent<CircleCollider2D>();
            AiDestinationSetter = GetComponent<AIDestinationSetter>();
            AiPath = GetComponent<AIPath>();
            Seeker = GetComponent<Seeker>();
            EnemyHealthManager = GetComponent<EnemyHealthManager>();
            EnemiesLayer = LayerMask.GetMask("Enemies");
            Renderer = GetComponent<Renderer>();
            DefaultMaterial = Renderer.material;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            Target = new GameObject("target " + gameObject.name);
            State = States.Standard;
            // Set initial enemy position according to its initial position
            HomePosition = Rigidbody.position;

            // Set the first random target movement 
            Target.transform.position = GenerateNewTarget();
            AiDestinationSetter.target = Target.transform;

            // Instantiate prefab field of view
            FieldOfViewComponent = Instantiate(PreFabFieldOfView, null).GetComponent<FieldOfView>();
            FieldOfViewComponent.gameObject.name = "Field of view" + gameObject.name;
            FieldOfViewComponent.SetFieldOfView(FieldOfViewValue);
            FieldOfViewComponent.SetViewDistance(ViewDistance);
            FieldOfViewComponent.SetMyEnemyBehavior(this);

            // Game Manager
            GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

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
            else
            {
                #region State Machine
                switch (State)
                {
                    default:

                    case States.Standard:
                        IsWalkingAround = true;
                        // Checking if player is close
                        CheckSurroundings();
                    
                        // Updating field of view
                        FieldOfViewComponent.SetAimDirection(FaceDirection);
                        FieldOfViewComponent.SetOrigin(transform.position);
                        // Looking for the player
                        if (TargetPlayer != null)
                        {
                            ChangeState(States.Chasing);
                        }
                        break;
                

                    case States.Chasing:
                        // Verify if there is close enemies chasing the player
                    


                        if (TargetPlayer != null) // Know where the player is
                        {
                            Animate();
                            SetCurrentFaceDirection();

                            // Return to Standard state
                            if (Vector2.Distance(transform.position, TargetPlayer.transform.position) > 7f)
                            {
                                TargetPlayer = null;
                            }
                        }
                        else // Lost sight of player
                        {
                            ChangeState(States.Standard);
                        }
                        break;

                    case States.Attacking:

                        break;

                }
                #endregion
            }
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(IsWalkingAround)
                WalkAround();
        }
        #endregion

        #region Auxiliar Methods

        private Vector3 GenerateNewTarget()
        {
            return new Vector3(HomePosition.x + (WalkableRange * Random.Range(-1, 2)), HomePosition.y + (WalkableRange * Random.Range(-1, 2)), transform.position.z);
        }
    
        private void WalkAround()
        {
            // Generate new target every maxTimer (three seconds)

            if (AiPath.reachedEndOfPath) //if reached desired location, wait three seconds and move to another
            {
                // Animate standing still
                Animator.SetBool("isMoving", false);
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

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetTargetPlayer(GameObject player)
        {
            this.TargetPlayer = player;
        }
   
        private void ChangeState(States state)
        {
            switch (state)
            {
                default:

                case (States.Standard):
                    IsWalkingAround = true;
                    TargetPlayer = null;
                    AiDestinationSetter.target = Target.transform;
                    FieldOfViewComponent.gameObject.SetActive(true);
                    break;

                case (States.Chasing):
                    IsWalkingAround = false;
                    FieldOfViewComponent.gameObject.SetActive(false);
                    AiDestinationSetter.target = TargetPlayer.transform;
                    AstarPath.active.Scan();
                    break;

                case (States.Attacking):
                    break;
            }
            this.State = state;
        }

        private void CheckSurroundings()
        {
            if(Vector2.Distance(Player.transform.position, this.transform.position) < SurroundingDistance)
            {
                TargetPlayer = Player;
            }
        }

        private void SearchForAlliesActivityNearby() // if there is an ally nearby chasing the player, the object starts to follow the player too
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

        private void SetCurrentFaceDirection()
        {
            if (Animator.GetBool("isMoving")) // just want to change the facing direction if the object is walking
            {
                CurrentDirection = AiPath.desiredVelocity;
            }
            CurAngle = Utilities.GetAngleFromVectorFloat(CurrentDirection);
            // Actual set of face direction
            FaceDirection = Utilities.GetDirectionFromAngle(CurAngle);
        }

        private void Animate()
        {
            Animator.SetBool("isMoving", true);
            Animator.SetFloat("moveX", AiPath.velocity.x);
            Animator.SetFloat("moveY", AiPath.velocity.y);
        }
        #endregion

        public void TakeDamage(float weaponDamage, float weaponKnockback, Vector3 attackDirection, float knockbackDuration, float weaponAttackSpeed)
        {
            if (Invincible) return; // if takes damage recently, dont take damage;

            Invincible = true;
            
            // Make hit noise
            AudioManager.instance.Play("Hit enemy");
            
            FlashSprite();

            // Decrease health
            EnemyHealthManager.TakeDamage((int) weaponDamage);
           
            // Take knockback
            AiPath.enabled = false;
            Rigidbody.AddForce(attackDirection * weaponKnockback, ForceMode2D.Impulse);
            StartCoroutine(TakeKnockback(knockbackDuration));

            // Work on it
            float timeFlashing = TimeInvencible / (weaponAttackSpeed / 6f);
            Invoke(nameof(EndFlash), timeFlashing / 3);
            Invoke(nameof(FlashSprite), timeFlashing / 2);
            Invoke(nameof(EndFlash), timeFlashing);
            Invoke(nameof(Endinvincibility), timeFlashing - 0.05f);
        }

        public float GetDamage()
        {
            return Damage;
        }

        private void FlashSprite()
        {
            Renderer.material = FlashMaterial;
        }

        private void EndFlash()
        {
            Renderer.material = DefaultMaterial;
        }
        private void Endinvincibility ()
        {
            Invincible = false;
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage((int)Damage);

        }

        private IEnumerator TakeKnockback(float knockbackTime)
        {
            yield return new WaitForSeconds(knockbackTime);
            AiPath.enabled = true;
        }
    }
}
