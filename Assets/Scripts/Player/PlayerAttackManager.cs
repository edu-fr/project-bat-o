using System.Security.Cryptography;
using Enemy;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        #region Enums

        private enum Directions {UpperLeft, UpperRight, DownLeft, DownRight}

        private enum WeaponType {Bat}

        #endregion

        #region Variables

        [SerializeField] 
        private bool HasWeaponEquipped = true;
        [HideInInspector]
        public bool IsAttacking { private set; get;} = false;
        
        private WeaponType CurrentWeaponType = WeaponType.Bat;
        private float CurrentWeaponDamage;
        private float CurrentWeaponKnockback;
        private float CurrentWeaponAttackSpeed;
        private float CurrentKnockbackDuration;

        private Animator MyAnimator;
        [SerializeField] 
        private Transform AttackPoint;
        [SerializeField] 
        private float AttackRange = 1f;
        [SerializeField] 
        private float XOffset = 1f;
        [SerializeField] 
        private float YOffset = 1f;
        private LayerMask EnemyLayers;
        private Directions Direction;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Creating an attack point
            AttackPoint = new GameObject("Attack point").transform;
        
            // Make the attack point follows player
            AttackPoint.parent = this.gameObject.transform;

            // set attack point default position as same as player position
            AttackPoint.transform.position = transform.position;
        
            // set inactive while not attacking
            AttackPoint.gameObject.SetActive(false);

            MyAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Set position according to player's direction and give an offset 
            Direction = GetAnimationDirection();
            SetAttackPointPositionAccordToDirection(Direction);
            
            if (HasWeaponEquipped)
            {
                SetWeaponStats();
                if(Input.GetKeyDown(KeyCode.Z) && !MyAnimator.GetBool("IsAttacking"))
                    Attack();

                if (IsAttacking)
                {
                    // Detect if hit enemies
                    VerifyAttackCollision();
                }
            }
        }

        #endregion

        #region Auxiliar Methods

        private void Attack()
        {
            AttackPoint.gameObject.SetActive(true);
        
            // Set attack animation
            MyAnimator.speed = CurrentWeaponAttackSpeed * 0.2f;
            MyAnimator.SetTrigger("Attack");
            MyAnimator.SetBool("IsAttacking", true);
        }
        
        private void AttackEnd()
        {
            MyAnimator.speed = 1f;
            MyAnimator.SetBool("IsAttacking", false);
            IsAttacking = false;
            AttackPoint.gameObject.SetActive(false);
        }

        private void VerifyAttackCollision()
        {
            // Detect enemies in range of attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange);

            foreach(Collider2D enemy in hitEnemies)
            {
                if(enemy.CompareTag("Enemy"))
                {
                    Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
                    enemy.GetComponent<EnemyBehavior>().TakeDamage(CurrentWeaponDamage, CurrentWeaponKnockback, attackDirection, CurrentKnockbackDuration);
                }
            }
        }
        
        private Directions GetAnimationDirection()
        {
            float lastMoveX = MyAnimator.GetFloat("LastMoveX");
            float lastMoveY = MyAnimator.GetFloat("LastMoveY");
            if (lastMoveX < 0 && lastMoveY < 0)
                return Directions.DownLeft;
            else if (lastMoveX > 0 && lastMoveY < 0)
                return Directions.DownRight;
            else if (lastMoveX > 0 && lastMoveY > 0)
                return Directions.UpperRight;
            else if (lastMoveX < 0 && lastMoveY > 0)
                return Direction = Directions.UpperLeft;
            else
                return Direction = Directions.DownRight;
        }

        private void SetAttackPointPositionAccordToDirection(Directions direction)
        {
        
            switch (direction)
            {
                default:

                case (Directions.UpperLeft):
                    AttackPoint.position = transform.position + new Vector3(-XOffset, YOffset, 0);
                    break;
                case (Directions.UpperRight):
                    AttackPoint.position = transform.position + new Vector3(XOffset, YOffset, 0);

                    break;
                case (Directions.DownLeft):
                    AttackPoint.position = transform.position + new Vector3(-XOffset, -YOffset, 0);

                    break;
                case (Directions.DownRight):
                    AttackPoint.position = transform.position + new Vector3(XOffset, -YOffset, 0);

                    break;
            }
        }

        private void SetWeaponStats()
        {
            switch(CurrentWeaponType)
            {
                default:

                    break;
                case (WeaponType.Bat):
                    CurrentWeaponDamage = 12;
                    CurrentWeaponKnockback = 1f;
                    CurrentKnockbackDuration = 0.1f;
                    CurrentWeaponAttackSpeed = 6f;
                    break;
               

            }
        }

        private void OnDrawGizmosSelected()
        {
            if (AttackPoint == null) return;
            
            Gizmos.DrawWireSphere(AttackPoint.transform.position, AttackRange);
        }

        #endregion
    }
}