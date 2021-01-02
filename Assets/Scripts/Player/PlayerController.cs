        using System;
        using System.Collections.Generic;
        using Enemy;
        using UnityEngine;

        namespace Player
        {
            public class PlayerController : MonoBehaviour
            {
                #region Variables
                private Animator Animator;
                private Rigidbody2D RigidBody;
                private PlayerAttackManager PlayerAttackManager;
                public Collider2D PlayerCollider;

                [SerializeField]
                private float Speed = 0;

                // Z-targeting
                private float ZTargetingRadius = 2f;
                private LayerMask EnemyLayerMask;
                private Collider2D[] NearbyEnemiesArray;
                private int MaxNumEnemiesNearby = 10;
                private EnemyBehavior TargetedEnemy;
                private bool IsZTargeting = false; 
                
                private float MoveX;
                private float MoveY;
                
                #endregion

                #region Unity Callbacks

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
                #endregion

                #region Auxiliar Methods
                private void HandleMovement()
                {
                    if (IsZTargeting && TargetedEnemy == null)
                    {
                        IsZTargeting = false;
                    }
                    
                    RigidBody.velocity = Animator.GetBool("IsAttacking") ? 
                        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed / 50 : 
                        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;
                    
                    float lastMoveX;
                    float lastMoveY;

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

                    Animator.SetFloat("LastMoveX", lastMoveX);
                    Animator.SetFloat("LastMoveY", lastMoveY);
                }

                public Vector3 GetPosition()
                {
                    return transform.position;
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
                
                #endregion
            }
        }

