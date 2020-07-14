using System;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    #region Enums

    enum Directions {UpperLeft, UpperRight, DownLeft, DownRight}

    enum WeaponType {Bat}

    #endregion

    #region Variables

    [SerializeField] private bool hasWeaponEquipped = false;
    private WeaponType currentWeaponType = WeaponType.Bat;
    private float currentWeaponDamage = 0;
    private float currentWeaponKnockback = 0;
    private float currentKnockbackDuration = 0;
    private Animator myAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 1f;
    private LayerMask enemyLayers;
    private Directions direction;

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        // Creating an attack point
        attackPoint = new GameObject("Attack point").transform;
        
        // Make the attack point follows player
        attackPoint.parent = this.gameObject.transform;

        // set attack point default position as same as player position
        attackPoint.transform.position = transform.position;
        
        // set inactive while not attacking
        attackPoint.gameObject.SetActive(false);

        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        // Set position according to player's direction and give an offset 
        direction = GetAnimationDirection();
        SetAttackPointPositionAccordToDirection(direction);

        
        if (hasWeaponEquipped)
        {
            SetWeaponStats();
            if(Input.GetKeyDown(KeyCode.Z))
                Attack();
        }
    }

    #endregion

    #region Auxiliar Methods

    private void Attack()
    {
        attackPoint.gameObject.SetActive(true);
        
        // Set attack animation
        myAnimator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach(Collider2D enemy in hitEnemies)
        {
            if(enemy.CompareTag("Enemy"))
            {
                Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.GetComponent<EnemyBehavior>().TakeDamage(currentWeaponDamage, currentWeaponKnockback, attackDirection, currentKnockbackDuration);
            }
        }
        
        attackPoint.gameObject.SetActive(false);
    }

    private Directions GetAnimationDirection()
    {
        float lastMoveX = myAnimator.GetFloat("lastMoveX");
        float lastMoveY = myAnimator.GetFloat("lastMoveY");
        if (lastMoveX < 0 && lastMoveY < 0)
            return Directions.DownLeft;
        else if (lastMoveX > 0 && lastMoveY < 0)
            return Directions.DownRight;
        else if (lastMoveX > 0 && lastMoveY > 0)
            return Directions.UpperRight;
        else if (lastMoveX < 0 && lastMoveY > 0)
            return direction = Directions.UpperLeft;
        else
            return direction = Directions.DownRight;
    }

    private void SetAttackPointPositionAccordToDirection(Directions direction)
    {
        
        switch (direction)
        {
            default:

            case (Directions.UpperLeft):
                attackPoint.position = transform.position + new Vector3(-xOffset, yOffset, 0);
                break;
            case (Directions.UpperRight):
                attackPoint.position = transform.position + new Vector3(xOffset, yOffset, 0);

                break;
            case (Directions.DownLeft):
                attackPoint.position = transform.position + new Vector3(-xOffset, -yOffset, 0);

                break;
            case (Directions.DownRight):
                attackPoint.position = transform.position + new Vector3(xOffset, -yOffset, 0);

                break;
        }
    }

    private void SetWeaponStats()
    {
        switch(currentWeaponType)
        {
            default:

                break;
            case (WeaponType.Bat):
                currentWeaponDamage = 25;
                currentWeaponKnockback = 1f;
                currentKnockbackDuration = 0.1f;
                break;
               

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }

    #endregion
}