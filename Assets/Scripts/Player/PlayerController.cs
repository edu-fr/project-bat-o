        using System;
        using System.Collections.Generic;
        using Enemy;
        using UnityEngine;

        namespace Player
        {
            public class PlayerController : MonoBehaviour
            {
                private Animator Animator;
                private Rigidbody2D RigidBody;
                private PlayerAttackManager PlayerAttackManager;
                public Collider2D PlayerCollider;

                // Move speeds
                [SerializeField] private float StandardMoveSpeed = 4;
                [SerializeField] private float MoveSpeedMultiplier = 1;
                [SerializeField] private float AttackingMoveSpeedMultiplier;
                [SerializeField] private float ZTargetingMoveSpeedMultiplier;
                [SerializeField] private float DashMoveSpeedMultiplier = 2.5f; 
                [SerializeField] private float MoveSpeed; 

                // Z-targeting
                [SerializeField] private float ZTargetingRadius = 2f;
                private LayerMask EnemyLayerMask;
                private Collider2D[] NearbyEnemiesArray;
                [SerializeField] private int MaxNumEnemiesNearby = 10;
                private EnemyBehavior TargetedEnemy;
                private bool IsZTargeting = false; 
                
                // Dash
                private bool IsDashing = false;
                [SerializeField] private float DashDuration = 0.05f;
                private float DashCurrentTime;
                [SerializeField] private float DashCooldown = 2f;
                [SerializeField] private float DashCurrentCooldown;
                
                // Animation
                private float MoveX;
                private float MoveY;
                private float lastMoveX;
                private float lastMoveY;

                // Start is called before the first frame update
                private void Start()
                {
                    Animator = GetComponent<Animator>();
                    // Setting player face direction upwards
                    Animator.SetFloat("LastMoveX", 0);
                    Animator.SetFloat("LastMoveY", 1);
                    
                    RigidBody = GetComponent<Rigidbody2D>();
                    PlayerAttackManager = GetComponent<PlayerAttackManager>();
                    
                    // Z-targeting
                    EnemyLayerMask = LayerMask.GetMask("Enemies");
                    NearbyEnemiesArray = new Collider2D[MaxNumEnemiesNearby];
                    //
                }

                // Update is called once per frame
                private void Update()
                {
                    HandleMovement();
                }

                private void FixedUpdate()
                {
                    /* Movement */
                    RigidBody.velocity =
                        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * GetMoveSpeed();
                    /***/
                }

                private void HandleMovement()
                {
                    if (IsZTargeting && TargetedEnemy == null) // Verify if the targeted enemy has died
                        IsZTargeting = false;
                    
                    FaceDirection();
                    MovementAnimation();
                    ZTargeting();
                    Dash();
                }
                
                private void FaceDirection()
                {
                    /* Face direction handler */
                    if (IsZTargeting)
                    {
                        var FaceDirection = UtilitiesClass.Get8DirectionFromAngle(UtilitiesClass.GetAngleFromVectorFloat(
                            new Vector3(TargetedEnemy.transform.position.x - transform.position.x,
                                TargetedEnemy.transform.position.y - transform.position.y)));
                        Animator.SetFloat("MoveX", FaceDirection.x);
                        Animator.SetFloat("MoveY", FaceDirection.y);
                        
                        lastMoveX = FaceDirection.x;
                        lastMoveY = FaceDirection.y;
                    } else {
                        Animator.SetFloat("MoveX", RigidBody.velocity.x);
                        Animator.SetFloat("MoveY", RigidBody.velocity.y);
                        
                        lastMoveX = Animator.GetFloat("LastMoveX");
                        lastMoveY = Animator.GetFloat("LastMoveY");
                    }
                    /***/
                }

                  private void MovementAnimation()
                {
                    /* Movement animation handler */ 
                    if (!Animator.GetBool("IsAttacking") && !IsZTargeting)
                    {
                        if ((Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1) && (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1))
                        {
                            lastMoveX = Input.GetAxisRaw("Horizontal");
                            lastMoveY = Input.GetAxisRaw("Vertical");
                        }
                        else if(Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1)
                        {
                            lastMoveX = Input.GetAxisRaw("Horizontal");
                            lastMoveY = 0;
                        } 
                        else if (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
                        {
                            lastMoveX = 0;
                            lastMoveY = Input.GetAxisRaw("Vertical");
                        }
                    }
                    
                    Animator.SetFloat("LastMoveX", lastMoveX);
                    Animator.SetFloat("LastMoveY", lastMoveY);
                    /***/
                }

                
                private void ZTargeting()
                {
                    /* Z-targeting */
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        if (!IsZTargeting)
                        {
                            TargetedEnemy = GetZTargetEnemy();
                            if (TargetedEnemy != null)
                            {
                                var enemyStateMachine = TargetedEnemy.GetComponent<Enemy.EnemyStateMachine>();
                                if (enemyStateMachine != null)
                                {
                                    enemyStateMachine.IsTargeted = true;
                                }
                                IsZTargeting = true;
                            }
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.C))
                    {
                        if (IsZTargeting && TargetedEnemy != null)
                        {
                            var enemyStateMachine = TargetedEnemy.GetComponent<Enemy.EnemyStateMachine>();
                            if (enemyStateMachine != null)
                            {
                                enemyStateMachine.IsTargeted = false;
                            }
                            IsZTargeting = false;
                            TargetedEnemy = null;
                        }
                    }
    
                    /**/
                }

                private void Dash()
                {
                    if (!IsDashing)
                    {
                        if (DashCurrentCooldown > 0)
                            DashCurrentCooldown -= Time.deltaTime;

                        if (Input.GetKey(KeyCode.X))
                        {
                            if (!Animator.GetBool("IsAttacking") && DashCurrentCooldown <= 0)
                            {
                                Debug.Log("COOLDOWN!");
                                IsDashing = true;
                                DashCurrentTime = DashDuration;
                                DashCurrentCooldown = DashCooldown;
                            }
                        }    
                    }
                    DashCurrentTime -= Time.deltaTime;

                    if (DashCurrentTime <= 0)
                    {
                        IsDashing = false;
                    }
                }

                public Vector3 GetPosition()
                {
                    return transform.position;
                }

                private float GetMoveSpeed()
                {
                    if (Animator.GetBool("IsAttacking"))
                        return StandardMoveSpeed * AttackingMoveSpeedMultiplier;
                    if (IsDashing)
                        return StandardMoveSpeed * DashMoveSpeedMultiplier;
                    if (IsZTargeting)
                        return StandardMoveSpeed * ZTargetingMoveSpeedMultiplier;
                    return StandardMoveSpeed;
                }

              
                private EnemyBehavior GetZTargetEnemy()
                {
                    NearbyEnemiesArray = Physics2D.OverlapCircleAll(transform.position, ZTargetingRadius, EnemyLayerMask);
                    var ShorterDistanceEnemyIndex = -1;
                    var ShorterDistanceEnemy = 100f;
                    var CurrentEnemyDistance = -1f; 
                    for (var i = 0; i < NearbyEnemiesArray.Length; i++)
                    {
                        if (NearbyEnemiesArray[i] != null)
                        {
                            CurrentEnemyDistance = NearbyEnemiesArray[i].Distance(PlayerCollider).distance;
                            if (CurrentEnemyDistance < ShorterDistanceEnemy)
                            {
                                ShorterDistanceEnemy = CurrentEnemyDistance;
                                ShorterDistanceEnemyIndex = i; 
                            }
                        }
                    }

                    if (ShorterDistanceEnemyIndex != -1)
                    {
                        var TargetEnemyBehavior = NearbyEnemiesArray[ShorterDistanceEnemyIndex].GetComponent<EnemyBehavior>();
                        return TargetEnemyBehavior;
                    }
                    return null;
                }
  
            }
        }

