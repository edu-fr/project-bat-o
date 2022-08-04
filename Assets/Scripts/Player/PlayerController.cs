using UnityEngine;

namespace Player
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
        
        private PlayerAttackManager _playerAttackManager;
        private PlayerStateMachine _playerStateMachine;
        private PlayerStatsController _playerStats;
        private Rigidbody2D _rigidBody;
        private Collider2D _playerCollider;
        private Animator _animator;
        
        [Header("References")]
        [SerializeField] private ParticleSystem Dust;
        public Joystick joystick;

        // Dust
        private float DustVelocity = .5f; 

        // Animation
        private float MoveX;
        private float MoveY;
        public float lastMoveX;
        public float lastMoveY;
        
        public PlayerFaceDirection PlayerFaceDir { get; private set; }

        // Start is called before the first frame update
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerCollider = GetComponent<Collider2D>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerStats = GetComponent<PlayerStatsController>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _playerAttackManager = GetComponent<PlayerAttackManager>();
        }

        private void Start()
        {
            _animator.SetFloat("LastMoveX", 0);
            _animator.SetFloat("LastMoveY", 1);
        }

        public void HandleMovement(float modifier)
        {
            Walk(modifier);
            MovementAnimation();
        }

        private void Walk(float modifier)
        {
            var joystickDirection = joystick.Direction;
            var joystickValue = joystick.HandleRange;
            _rigidBody.velocity = joystickDirection * (_playerStats.CurrentMoveSpeed * modifier * joystickValue);
            // Reset's attack cooldown
            _playerAttackManager.CurrentAttackCooldown = 0;
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
            lastMoveX = joystick.Horizontal != 0 ? joystick.Horizontal : lastMoveX;
            lastMoveY = joystick.Vertical != 0 ? joystick.Vertical : lastMoveY;
            
            _animator.SetFloat("MoveX", joystick.Direction.x);
            _animator.SetFloat("MoveY", joystick.Direction.y);
            
            _animator.SetFloat("LastMoveX", lastMoveX);
            _animator.SetFloat("LastMoveY", lastMoveY);
            /***/
            SetFaceDirectionVariable(lastMoveX, lastMoveY);
        }

        public void CreateDust()
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = Dust.velocityOverLifetime;
            velocityOverLifetimeModule.x = Mathf.Clamp(- _rigidBody.velocity.x, -DustVelocity, DustVelocity); 
            velocityOverLifetimeModule.y = Mathf.Clamp(- _rigidBody.velocity.y, -DustVelocity + DustVelocity/2, DustVelocity + DustVelocity/2); 
            Dust.Play();
        }
    }
}