using System.Collections.Generic;
using Resources.Scripts.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        private enum Directions
        {
            UpLeft,
            UpRight,
            DownLeft,
            DownRight,
            Up,
            Down,
            Left,
            Right
        }

        private enum WeaponType
        {
            Sword
        }

        // private WeaponType CurrentWeaponType = WeaponType.Sword;
        [SerializeField] private float CurrentWeaponRange;
        [SerializeField] private float DefaultAttackCooldown;
        [SerializeField] [Range(1f, 5f)] private float CurrentAttackSpeed;
        public float currentAttackCooldown { get; set; }
        
        [SerializeField] private PowerUpController.Effects CurrentEffect = PowerUpController.Effects.None;

        private List<GameObject> EnemiesHit;
        [SerializeField]
        private LayerMask EnemyLayerMask;
        
        private Animator Animator;
        public PlayerHealthManager PlayerHealthManager;
        public PlayerStateMachine PlayerStateMachine;
        public Material StandardMaterial;
        public Material FireMaterial;
        public Material IceMaterial;
        public Material ThunderMaterial;
        private Renderer Renderer;

        public PowerUpController PowerUpController;
        public PowerUpActivator PowerUpActivator;
        private PlayerStatsController PlayerStatsController;

        [SerializeField] private LayerMask EnemyLayers;
        private Directions Direction;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            PowerUpController = GetComponent<PowerUpController>();
            Renderer = GetComponent<Renderer>();
            PlayerHealthManager = GetComponent<PlayerHealthManager>();
            PlayerStateMachine = GetComponent<PlayerStateMachine>();
            PowerUpActivator = GetComponent<PowerUpActivator>();
            PlayerStatsController = GetComponent<PlayerStatsController>();
        }

        private void Start()
        {
            EnemiesHit = new List<GameObject>();
            // SetWeaponStats();
            currentAttackCooldown = 0; 
        }

        public void TryToAttack()
        {
            if (currentAttackCooldown / PlayerStatsController.AttackSpeed > 0)
            {
                currentAttackCooldown -= Time.deltaTime;
            }
            else
            {
                var closestEnemy = ThereIsEnemiesInRange();
                if (closestEnemy == null) return;
                LookToTheEnemy(closestEnemy);
                Attack();
                currentAttackCooldown = DefaultAttackCooldown;
            }
        }
        
        private void Attack()
        {
            Direction = GetAnimationDirection();
            PlayerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
            CurrentEffect = PowerUpActivator.GenerateEffect();
            AnimateAttack();
            // switch (CurrentEffect)
            // {
            //     case (PowerUpController.Effects.Fire):
            //         Renderer.material = FireMaterial;
            //         break;
            //
            //     case (PowerUpController.Effects.Ice):
            //         Renderer.material = IceMaterial;
            //         break;
            //
            //     case (Player.PowerUpController.Effects.Thunder):
            //         Renderer.material = ThunderMaterial;
            //         break;
            //
            //     default:
            //         Renderer.material = StandardMaterial;
            //         break;
            // }
            
        }

        public void AnimateAttack()
        {
            // Set attack animation
            Animator.speed = CurrentAttackSpeed; // CurrentAttackSpeed * 0.2f;
            Animator.SetTrigger("Attack");
            Animator.SetBool("IsAttacking", true);
        }

        public GameObject ThereIsEnemiesInRange()
        {
            var nearbyEnemies = new List<Collider2D>();
            var filter = new ContactFilter2D();
            filter.SetLayerMask(EnemyLayerMask);
            Physics2D.OverlapCircle(transform.position, CurrentWeaponRange * 5, filter, nearbyEnemies);
            var ShorterDistanceEnemyIndex = -1;
            var ShorterDistanceEnemy = 100f;
            var CurrentEnemyDistance = -1f;
            for (var i = 0; i < nearbyEnemies.Count; i++)
            {
                if (nearbyEnemies[i] == null) continue;
                CurrentEnemyDistance = nearbyEnemies[i].Distance(PlayerStateMachine.PlayerController.PlayerCollider).distance;
                if (CurrentEnemyDistance < ShorterDistanceEnemy)
                {
                    ShorterDistanceEnemy = CurrentEnemyDistance;
                    ShorterDistanceEnemyIndex = i;
                }
            }

            if (ShorterDistanceEnemyIndex == -1) return null;
            return nearbyEnemies[ShorterDistanceEnemyIndex].gameObject;

        }
            
        // public void FlurryAttack()
        // {
        //     PlayerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
        //     Animator.speed = CurrentAttackSpeed * 0.2f;
        //     Animator.SetTrigger("Attack");
        //     Animator.SetBool("IsAttacking", true);
        // }

        public void AttackEnd()
        {
            Animator.speed = 1f;
            Animator.SetBool("IsAttacking", false);
            PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
            Renderer.material = StandardMaterial;
            ClearEnemiesHitList();
        }

        public void VerifyAttackCollision(GameObject enemy)
        {
            // avoids the enemy been hit twice in the same attack
            if (!EnemiesHit.Contains(enemy))
            {
                EnemiesHit.Add(enemy);
        
                Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
                if (CriticalTest()) // critical hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(PlayerStatsController.PhysicalDamage * PlayerStatsController.CriticalDamage, attackDirection,
                        PlayerStatsController.AttackSpeed ,false, true, true, null);
                else                // normal hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(PlayerStatsController.PhysicalDamage, attackDirection,
                        PlayerStatsController.AttackSpeed,false, false, true, null);
                PowerUpActivator.ApplyEffectsOnEnemies(enemy, CurrentEffect);
            }
        }

        private bool CriticalTest()
        {
            Random.InitState((int) Time.realtimeSinceStartup);
            var random = Random.Range(0, 100);
            return random < PlayerStatsController.CriticalRate;
        }

        private Directions GetAnimationDirection()
        {
            float lastMoveX = Animator.GetFloat("LastMoveX");
            float lastMoveY = Animator.GetFloat("LastMoveY");

            switch (lastMoveX)
            {
                case 1: //Up Right, Right and Down Right
                    switch (lastMoveY)
                    {
                        case 1:
                            return Directions.UpRight;

                        case 0:
                            return Directions.Right;

                        case -1:
                            return Directions.DownRight;
                    }

                    break;

                case 0: // Down and Up
                    return lastMoveY == 1 ? Directions.Up : Directions.Down;

                case -1: // Up Left, Left and Down Left
                    switch (lastMoveY)
                    {
                        case 1:
                            return Directions.UpLeft;

                        case 0:
                            return Directions.Left;

                        case -1:
                            return Directions.DownLeft;
                    }

                    break;
            }

            return Directions.Down;
        }


        private void LookToTheEnemy(GameObject enemy)
        {
            var enemyPosition = enemy.transform.position;
            var playerPosition = transform.position;
            var faceDirection = UtilitiesClass.Get8DirectionFromAngle(UtilitiesClass.GetAngleFromVectorFloat(new Vector3(enemyPosition.x - playerPosition.x, enemyPosition.y - playerPosition.y)));
            Animator.SetFloat("MoveX", faceDirection.x);
            Animator.SetFloat("MoveY", faceDirection.y);
            
            Animator.SetFloat("LastMoveX", faceDirection.x);
            Animator.SetFloat("LastMoveY", faceDirection.y);
            
            /* Necessary? */
            PlayerStateMachine.PlayerController.lastMoveX = faceDirection.x;
            PlayerStateMachine.PlayerController.lastMoveY = faceDirection.y;
            
            /* I need to set the face dir variable? */ 
            
        }
        
        // private void SetWeaponStats()
        // {
        //     switch (CurrentWeaponType)
        //     {
        //         default:
        //
        //             break;
        //         case (WeaponType.Sword):
        //             CurrentDamage = 34;
        //             CurrentAttackSpeed = 15f;
        //             break;
        //     }
        // }

        private void ClearEnemiesHitList()
        {
            if (EnemiesHit.Count > 0)
            {
                EnemiesHit.Clear();
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, CurrentWeaponRange);
        }
    }
}