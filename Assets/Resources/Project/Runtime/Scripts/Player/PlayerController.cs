using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public enum PlayerFaceDirection
        {
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }

        [SerializeField] private PlayerAttackManager PlayerAttackManager;
        [SerializeField] private PowerUpController PowerUpController;
        [SerializeField] private Rigidbody2D RigidBody;
        [SerializeField] private ParticleSystem Dust;
        public PlayerStateMachine PlayerStateMachine;
        public Collider2D PlayerCollider;
        public Animator Animator;
        public Joystick Joystick;

        // Move speeds
        public float StandardMoveSpeed { private set; get; } = 4f;
        public float AttackingMoveSpeedMultiplier { private set; get; }
        // public float ZTargetingMoveSpeedMultiplier { private set; get; }
        public float DashInitialMoveSpeedMultiplier;

        // // Z-targeting
        // [SerializeField]
        // public float ZTargetingRadius = 2f;
        // public bool IsZTargeting = false;

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
        public float lastMoveX;
        public float lastMoveY;
        
        public PlayerFaceDirection PlayerFaceDir { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            Animator.SetFloat("LastMoveX", 0);
            Animator.SetFloat("LastMoveY", 1);
        }

        public void HandleMovement(float modifier)
        {
            Walk(modifier);
            MovementAnimation();
            Dash();
        }

        private void Walk(float modifier)
        {
            var joystickDirection = Joystick.Direction;
            var joystickValue = Joystick.HandleRange;
            RigidBody.velocity = joystickDirection * (StandardMoveSpeed * modifier * joystickValue);
            // Reset's attack cooldown
            PlayerAttackManager.currentAttackCooldown = 0;
        }

        
        private void SetFaceDirectionVariable(float x, float y)
        {
            x = x != 0 ? Mathf.Lerp(-1, 1, x) : 0;
            y = y != 0 ? Mathf.Lerp(-1, 1, y) : 0;

            PlayerFaceDir = y switch
            {
                0 => x switch
                {
                    1 => PlayerFaceDirection.Right,
                    -1 => PlayerFaceDirection.Left,
                    _ => PlayerFaceDir
                },
                1 => x switch
                {
                    1 => PlayerFaceDirection.UpRight,
                    -1 => PlayerFaceDirection.UpLeft,
                    0 => PlayerFaceDirection.Up,
                    _ => PlayerFaceDir
                },
                -1 => x switch
                {
                    1 => PlayerFaceDirection.DownRight,
                    -1 => PlayerFaceDirection.DownLeft,
                    0 => PlayerFaceDirection.Down,
                    _ => PlayerFaceDir
                },
                _ => PlayerFaceDir
            };
        }

        private void MovementAnimation()
        {
            lastMoveX = Joystick.Horizontal != 0 ? Joystick.Horizontal : lastMoveX;
            lastMoveY = Joystick.Vertical != 0 ? Joystick.Vertical : lastMoveY;
            
            Animator.SetFloat("MoveX", Joystick.Direction.x);
            Animator.SetFloat("MoveY", Joystick.Direction.y);
            
            Animator.SetFloat("LastMoveX", lastMoveX);
            Animator.SetFloat("LastMoveY", lastMoveY);
            /***/
            SetFaceDirectionVariable(lastMoveX, lastMoveY);
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
                    // Invoke(nameof(DodgeTest), 0.15f);
                    PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
                }

                DashCurrentMoveSpeedMultiplier = DashInitialMoveSpeedMultiplier;
                
                // /* Perfect time dash */
                // if (PowerUpController.PerfectDodgeLevel > 0)
                // {
                //     if (!DodgeFailed && !DodgeSuccessful)
                //     {
                //         /*if (IsZTargeting)
                //         {*/
                //             if (TargetedEnemy.EnemyStateMachine.EnemyCombatManager.IsAttacking)
                //                 if (TargetedEnemy.EnemyStateMachine.BaseAttack.ProbablyGonnaHit)
                //                     if (!PlayerAttackManager.PlayerHealthManager.Invincible)
                //                     {
                //                         //Debug.Log("Player apanhou " + TargetedEnemy.EnemyStateMachine.EnemyCombatManager.LastTimeHitPlayerDuringAttack + " Dash começou aos " + DashStartTime);
                //                         //if (!TargetedEnemy.EnemyStateMachine.EnemyMeleeAttackManager.IsOnHalfOfAttackAnimation)
                //                         //Debug.Log("DESVIOU " + Time.time);
                //                         DodgeSuccessful = true;
                //                     }
                //                     else
                //                     {
                //                         DodgeFailed = true;
                //                         //Debug.Log("FALHA! Player invencivel aos " + Time.time);
                //                     }
                //                 else
                //                 {
                //                     DodgeFailed = true;
                //                     //Debug.Log("INIMIGO ERROU aos " + Time.time + "!");
                //                 }
                //        /* } */
                //    }
                // }
                /***/
            }

            // if (CanCounterAttack)
            // {
            //     Debug.Log("DODGE SUCCESSFUL!");
            //     FlurryRush.CanFlurryRush = true;
            //     CanCounterAttack = false;
            // }
        }

        private void DodgeTest()
        {
            CanCounterAttack = (!DodgeFailed && DodgeSuccessful);
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