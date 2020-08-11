﻿using System;
using System.Security.Cryptography;
using Enemy;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        #region Enums

        private enum Directions {UpLeft, UpRight, DownLeft, DownRight, Up, Down, Left, Right}

        private enum WeaponType {Sword}

        #endregion

        #region Variables

        [SerializeField] 
        private bool HasWeaponEquipped = true;
        [HideInInspector]
        public bool IsAttacking { private set; get;} = false;
        
        private WeaponType CurrentWeaponType = WeaponType.Sword;
        private float CurrentWeaponDamage;
        private float CurrentWeaponKnockback;
        private float CurrentWeaponAttackSpeed;
        private float CurrentKnockbackDuration;

        private Animator Animator;

        private PowerUpController PowerUpController;

        [SerializeField]
        private GameObject SwordHitBox;
        
        private LayerMask EnemyLayers;
        private Directions Direction;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            PowerUpController = GetComponent<PowerUpController>();
        }

        private void Update()
        {
            // Set position according to player's direction and give an offset 
            Direction = GetAnimationDirection();

            if (HasWeaponEquipped)
            {
                SetWeaponStats();
                if(Input.GetKeyDown(KeyCode.Z) && !Animator.GetBool("IsAttacking"))
                    Attack();
            }
        }

        #endregion

        #region Auxiliar Methods

        private void Attack()
        {
            // Set attack animation
            Animator.speed = CurrentWeaponAttackSpeed * 0.2f;
            Animator.SetTrigger("Attack");
            Animator.SetBool("IsAttacking", true);
        }
        
        private void AttackEnd()
        {
            Animator.speed = 1f;
            Animator.SetBool("IsAttacking", false);
            IsAttacking = false;
        }

        public void VerifyAttackCollision(GameObject enemy)
        {
            Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
            enemy.GetComponent<EnemyBehavior>().TakeDamage(CurrentWeaponDamage, CurrentWeaponKnockback, attackDirection, CurrentKnockbackDuration,  CurrentWeaponAttackSpeed);
            PowerUpController.ApplyEffects(enemy);
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

        private void SetWeaponStats()
        {
            switch(CurrentWeaponType)
            {
                default:

                    break;
                case (WeaponType.Sword):
                    CurrentWeaponDamage = 34;
                    CurrentWeaponKnockback = 1.7f;
                    CurrentKnockbackDuration = 0.1f;
                    CurrentWeaponAttackSpeed = 15f;
                    break;
               

            }
        }
        

        #endregion
    }
}