using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using Enemy;
using Game;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;
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

        private WeaponType CurrentWeaponType = WeaponType.Sword;
        public float CurrentDamage;
        public float CurrentKnockback;
        public float CurrentAttackSpeed;
        public float CurrentKnockbackDuration;
        public float CurrentCriticalHitMultiplier;
        public float CurrentCriticalHitChance;

        public PowerUpController.Effects CurrentEffect = PowerUpController.Effects.None;

        private List<GameObject> EnemiesHit;

        private Animator Animator;
        public PlayerHealthManager PlayerHealthManager;
        public PlayerStateMachine PlayerStateMachine;
        public Material StandardMaterial;
        public Material FireMaterial;
        public Material IceMaterial;
        public Material ThunderMaterial;
        private Renderer Renderer;
        public Transform PrefabDamagePopup;

        public PowerUpController PowerUpController;
        public PowerUpActivator PowerUpActivator;

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
        }

        private void Start()
        {
            EnemiesHit = new List<GameObject>();
            SetWeaponStats();
        }

        public void HandleAttack()
        {
            Direction = GetAnimationDirection();
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
                CurrentEffect = PowerUpActivator.GenerateEffect();
                switch (CurrentEffect)
                {
                    case (PowerUpController.Effects.Fire):
                        Renderer.material = FireMaterial;
                        break;
        
                    case (PowerUpController.Effects.Ice):
                        Renderer.material = IceMaterial;
                        break;
        
                    case (Player.PowerUpController.Effects.Thunder):
                        Renderer.material = ThunderMaterial;
                        break;
        
                    default:
                        Renderer.material = StandardMaterial;
                        break;
                }
        
                Attack();
            }
        }

        public void Attack()
        {
            // Set attack animation
            Animator.speed = CurrentAttackSpeed * 0.2f;
            Animator.SetTrigger("Attack");
            Animator.SetBool("IsAttacking", true);
        }

        public void FlurryAttack()
        {
            PlayerStateMachine.ChangeState(PlayerStateMachine.States.Attacking);
            Renderer.material = FireMaterial;
            Animator.speed = CurrentAttackSpeed * 0.2f;
            Animator.SetTrigger("Attack");
            Animator.SetBool("IsAttacking", true);
        }

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
                if (CriticalTest())
                {
                    // critical hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(CurrentDamage * CurrentCriticalHitMultiplier,
                        CurrentKnockback * 1.3f, attackDirection, CurrentKnockbackDuration, CurrentAttackSpeed);
                    DamagePopup.Create(enemy.transform.position, (int) (CurrentDamage * CurrentCriticalHitMultiplier),
                        true, attackDirection, PrefabDamagePopup);
                }
                else
                {
                    // normal hit
                    enemy.GetComponent<EnemyCombatManager>().TakeDamage(CurrentDamage, CurrentKnockback,
                        attackDirection, CurrentKnockbackDuration, CurrentAttackSpeed);
                    DamagePopup.Create(enemy.transform.position, (int) CurrentDamage, false, attackDirection,
                        PrefabDamagePopup);
                }
        
                PowerUpActivator.ApplyEffectsOnEnemies(enemy, CurrentEffect);
            }
        }

        private bool CriticalTest()
        {
            var random = Random.Range(0, 100);
            Random.InitState((int) Time.realtimeSinceStartup);
            return random < CurrentCriticalHitChance;
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
            switch (CurrentWeaponType)
            {
                default:

                    break;
                case (WeaponType.Sword):
                    CurrentDamage = 34;
                    CurrentKnockback = 1.7f;
                    CurrentKnockbackDuration = 0.1f;
                    CurrentAttackSpeed = 15f;
                    break;
            }
        }

        private void ClearEnemiesHitList()
        {
            if (EnemiesHit.Count > 0)
            {
                EnemiesHit.Clear();
            }
        }
    }
}