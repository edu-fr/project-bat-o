using System;
using System.Collections.Generic;
using Enemy;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public Animator Animator;
        public Rigidbody2D RigidBody;
        public PlayerStateMachine PlayerStateMachine;
        public PlayerAttackManager PlayerAttackManager;
        public Collider2D PlayerCollider;
        public FlurryRush FlurryRush;
        public ParticleSystem Dust;

        // Move speeds
        public float StandardMoveSpeed { private set; get; } = 4f;
        public float AttackingMoveSpeedMultiplier { private set; get; }
        public float ZTargetingMoveSpeedMultiplier { private set; get; }
        public float DashInitialMoveSpeedMultiplier;

        // Z-targeting
        [SerializeField]
        public float ZTargetingRadius = 2f;

        [SerializeField]
        private LayerMask EnemyLayerMask;

        private Collider2D[] NearbyEnemiesArray;

        [SerializeField]
        private int MaxNumEnemiesNearby = 10;

        public EnemyBehavior TargetedEnemy;
        public bool IsZTargeting = false;

        // Dash
        public float DashMoveSpeedDecreaseMultiplier = 5f;

        public float DashCurrentMoveSpeedMultiplier;

        [SerializeField]
        private float DashCooldown = 2f;

        [SerializeField]
        private float DashCurrentCooldown;

        private float DashStartTime;
        public bool DodgeFailed;
        private bool DodgeSuccessful;
        private bool CanCounterAttack;
        
        // Dust
        private float DustVelocity = .5f; 

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
            PlayerStateMachine = GetComponent<PlayerStateMachine>();

            // Z-targeting
            EnemyLayerMask = LayerMask.GetMask("Enemies");
            NearbyEnemiesArray = new Collider2D[MaxNumEnemiesNearby];
            //

            // Flurry rush
            FlurryRush = GetComponent<FlurryRush>();
        }

        public void HandleMovement()
        {
            if (IsZTargeting && TargetedEnemy == null) // Verify if the targeted enemy has died
                IsZTargeting = false;
            FaceDirection();
            MovementAnimation();
            ZTargeting();
            Dash();
        }

        public void FaceDirection()
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
            }
            else
            {
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
            if (PlayerStateMachine.State != PlayerStateMachine.States.Attacking && !IsZTargeting)
            {
                if ((Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1) &&
                    (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1))
                {
                    lastMoveX = Input.GetAxisRaw("Horizontal");
                    lastMoveY = Input.GetAxisRaw("Vertical");
                }
                else if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1)
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

            if (Input.GetKeyUp(KeyCode.C) && !CanCounterAttack && PlayerStateMachine.State != PlayerStateMachine.States.Dashing)
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

        public void Dash()
        {
            if (PlayerStateMachine.State != PlayerStateMachine.States.Dashing)
            {
                if (DashCurrentCooldown > 0)
                    DashCurrentCooldown -= Time.deltaTime;
                else
                {
                    DashCurrentCooldown = 0;
                }

                if (RigidBody.velocity != Vector2.zero)
                {
                    if (Input.GetKey(KeyCode.X))
                    {
                        if (DashCurrentCooldown <= 0)
                        {
                            PlayerStateMachine.ChangeState(PlayerStateMachine.States.Dashing);
                            CreateDust();
                            DashStartTime = Time.time;
                            DashCurrentMoveSpeedMultiplier = DashInitialMoveSpeedMultiplier;
                            DashCurrentCooldown = DashCooldown;
                            DodgeSuccessful = false;
                            DodgeFailed = false;
                            CanCounterAttack = false;
                        }
                    }
                }
            }
            else
            {
                DashCurrentMoveSpeedMultiplier -= Time.deltaTime * DashMoveSpeedDecreaseMultiplier;
            }

            if (DashCurrentMoveSpeedMultiplier <= 1)
            {
                if (PlayerStateMachine.State == PlayerStateMachine.States.Dashing)
                {
                    Invoke(nameof(DodgeTest), 0.15f);
                    PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
                }

                DashCurrentMoveSpeedMultiplier = DashInitialMoveSpeedMultiplier;

                /* Perfect time dash */
                if (!DodgeFailed && !DodgeSuccessful)
                {
                    if (IsZTargeting)
                    {
                        if (TargetedEnemy.EnemyStateMachine.EnemyType == EnemyStateMachine.Type.Melee)
                            if (TargetedEnemy.EnemyStateMachine.EnemyCombatManager.IsAttacking)
                                if (TargetedEnemy.EnemyStateMachine.EnemyMeleeAttackManager.ProbablyGonnaHit)
                                    if (!PlayerAttackManager.PlayerHealthManager.Invincible)
                                    {
                                        //Debug.Log("Player apanhou " + TargetedEnemy.EnemyStateMachine.EnemyCombatManager.LastTimeHitPlayerDuringAttack + " Dash começou aos " + DashStartTime);
                                        //if (!TargetedEnemy.EnemyStateMachine.EnemyMeleeAttackManager.IsOnHalfOfAttackAnimation)
                                        //Debug.Log("DESVIOU " + Time.time);
                                        DodgeSuccessful = true;
                                    }
                                    else
                                    {
                                        DodgeFailed = true;
                                        //Debug.Log("FALHA! Player invencivel aos " + Time.time);
                                    }
                                else
                                {
                                    DodgeFailed = true;
                                    //Debug.Log("INIMIGO ERROU aos " + Time.time + "!");
                                }
                    }
                }

                /***/
            }


            if (CanCounterAttack)
            {
                Debug.Log("DODGE SUCCESSFUL!");
                FlurryRush.CanFlurryRush = true;
                CanCounterAttack = false;
            }
        }

        private void DodgeTest()
        {
            CanCounterAttack = (!DodgeFailed && DodgeSuccessful);
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

        public void CreateDust()
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = Dust.velocityOverLifetime;
            velocityOverLifetimeModule.x = Mathf.Clamp(- RigidBody.velocity.x, -DustVelocity, DustVelocity); 
            velocityOverLifetimeModule.y = Mathf.Clamp(- RigidBody.velocity.y, -DustVelocity + DustVelocity/2, DustVelocity + DustVelocity/2); 
            Dust.Play();
        }
    }
}