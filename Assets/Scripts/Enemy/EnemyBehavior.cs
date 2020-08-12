﻿using System;
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
        #region Enums

        public enum States {Standard, Chasing, Attacking, DyingBurned, Frozen, Paralyzed};

        #endregion

        #region Variables

      
        // Components
        private Rigidbody2D Rigidbody;
        public CircleCollider2D CircleCollider;
        private Animator Animator;
        private AIDestinationSetter AiDestinationSetter;
        private AIPath AiPath;
        private Renderer Renderer;
        private Material DefaultMaterial;
        private Material CurrentMaterial;
        public Material FlashMaterial;
        public Material BurnedMaterial;
        public Material FrozenMaterial;
        public Material ParalyzedMaterial;


        // Movement
        private Path Path;
        private bool ReachedEndOfPath;
        private float CurrentTimer = 0f;

        private float MaxTimer = 3f;            // time to move to the next random spot
        public float WalkingAroundSpeed = 2;    // walk speed
        public float ChasingSpeed = 3.5f;       // chasing speed
        public float DyingBurnedSpeed = 4.5f;   // running on fire speed
        private Vector3 HomePosition;           // original position on the level
        private float WalkableRange = 1f;       // Distance it can walk while isnt chasing the player 
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
        private States PreviousState;
        private bool IsWalkingAround = false;
        
        public bool IsFrozen = false;
        public float DefrostCurrentTimer;
        public float DefrostTime;
        public bool IsOnFire = false;
        public bool IsPrimaryTarget = false;
        public bool IsParalyzed = false;
        public float ParalyzeHealCurrentTimer;
        public float ParalyzeHealTime;
        
        public bool WillDieBurned = false;
        

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
            GetComponent<Seeker>();
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

            // Current sprite material
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
            else
            {
                
                if (IsOnFire)
                {
                    CurrentMaterial = BurnedMaterial;
                }
                else if (IsFrozen)
                {
                    CurrentMaterial = FrozenMaterial;
                }
                else if (IsParalyzed)
                {
                    CurrentMaterial = ParalyzedMaterial;
                }
                else
                {
                    CurrentMaterial = DefaultMaterial;
                    Renderer.material = CurrentMaterial;
                }

                #region State Machine
                switch (State)
                {
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
                    
                    case States.DyingBurned:
                        Animate();
                        SetCurrentFaceDirection();
                        
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
                    Animator.speed = 1;
                    TargetPlayer = null;
                    AiPath.maxSpeed = WalkingAroundSpeed;
                    AiDestinationSetter.target = Target.transform;
                    FieldOfViewComponent.gameObject.SetActive(true);
                    break;

                case (States.Chasing):
                    // Making sure that it isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    Animator.speed = 1.2f;
                    IsWalkingAround = false;
                    AiPath.maxSpeed = ChasingSpeed;
                    FieldOfViewComponent.gameObject.SetActive(false);
                    AiDestinationSetter.target = TargetPlayer.transform;
                    AstarPath.active.Scan();
                    break;

                case (States.Attacking):
                    
                    break;
                
                case (States.DyingBurned):
                    // Making sure that isn't frozen or paralyzed
                    IsFrozen = false;
                    IsParalyzed = false;
                    
                    IsWalkingAround = false;
                    AiPath.maxSpeed = DyingBurnedSpeed;
                    
                    if (FieldOfViewComponent.gameObject != null)
                    {
                        FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    RunFromThePlayer();
                    Animator.speed = 1.5f;
                    AiDestinationSetter.target = Target.transform;
                    break;
                
                case (States.Frozen):
                    IsWalkingAround = false;
                    IsFrozen = true;
                    Animator.speed = 0;
                    AiPath.maxSpeed = 0;
                    if (FieldOfViewComponent.gameObject != null)
                    {
                        FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    break;
                
                case (States.Paralyzed):
                    IsWalkingAround = false;
                    IsParalyzed = true;
                    Animator.speed = 0;
                    AiPath.maxSpeed = 0;
                    if (FieldOfViewComponent.gameObject != null)
                    {
                        FieldOfViewComponent.gameObject.SetActive(false);
                    }
                    break;
            }
            this.State = state;
        }

        
        private Vector3 GenerateNewTarget()
        {
            return new Vector3(HomePosition.x + (WalkableRange * Random.Range(-1, 2)), HomePosition.y + (WalkableRange * Random.Range(-1, 2)), transform.position.z);
        }

        private void RunFromThePlayer()
        {
            Vector3 oldTargetPosition =
                TargetPlayer != null ? TargetPlayer.transform.position : Target.transform.position;
            
            Vector3 newTarget = new Vector3(0, 0, 0);

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
            Renderer.material = CurrentMaterial;
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, 1.7f);
        }
    }
}
