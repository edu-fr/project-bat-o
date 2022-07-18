using System;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
    
        public enum States
        {
            Standard,
            Attacking,
            Frozen,
            Paralyzed,
            Dashing, 
            CanRush,
            Rushing,
            Petrified
        };

        public PlayerController PlayerController;
        public PlayerAttackManager PlayerAttackManager;
        public FlurryRush FlurryRush;
        public LayerMask ObjectsLayerMask;

        public States State;

        private float crowdControlTotalDuration;
        private float crowdControlCurrentTime;

        private void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerAttackManager = GetComponent<PlayerAttackManager>();
            FlurryRush = GetComponent<FlurryRush>();
        }
    
        private void FixedUpdate()
        {
            if (LevelManager.GameIsPaused) return; 

            switch (State)
            {
                case States.Standard:
                    PlayerController.HandleMovement(1f);
                    if (PlayerController.Joystick.Horizontal == 0 && PlayerController.Joystick.Vertical == 0) // If not moving
                        PlayerAttackManager.TryToAttack();
                    /***/
                    break;

                case States.Attacking:
                    if (PlayerController.Joystick.Horizontal != 0 || PlayerController.Joystick.Vertical != 0)
                        PlayerAttackManager.AttackEnd();
                    break;

                case States.Dashing:
                    PlayerController.Dash();
                    break;

                case States.Rushing:
                    // PlayerAttackManager.PlayerHealthManager.Invincible = true;
                    // if (!FlurryRush.hasStartedLerp)
                    // {
                    //     FlurryRush.TimeStartLerping = Time.unscaledTime;
                    //     FlurryRush.hasStartedLerp = true;
                    // }
                    // if (FlurryRush.RushToEnemyPosition())
                    // {
                    //     FlurryRush.FlurryAttack();
                    // }
                    break;

                case States.Paralyzed:
                    break;

                case States.Frozen:
                    break;
            
                case States.Petrified:
                    crowdControlCurrentTime += Time.deltaTime;
                    if (crowdControlCurrentTime >= crowdControlTotalDuration)
                    {
                        ChangeState(States.Standard);
                    }
                    break;
            }
        }

        public void ChangeState(States newState)
        {
            switch(newState)
            {
                case States.Standard:
                    State = States.Standard;
                    break;
                
                case States.Attacking:
                    State = States.Attacking;
                    break;
            
                case States.Dashing:
                    State = States.Dashing;
                    break;
            
                case States.Frozen:
                    State = States.Frozen;
                    break;
            
                case States.Paralyzed:
                    State = States.Paralyzed;
                    break; 
            
                case States.CanRush:
                    State = States.CanRush;
                    break;
            
                case States.Rushing:
                    State = States.Rushing;
                    break;
            
                case States.Petrified:
                    State = States.Petrified;
                    PlayerController.Animator.speed = 0;
                
                    // Make the player gray
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

        }   
        public void PetrifyPlayer(float duration)
        {
            ChangeState(States.Petrified);
            crowdControlTotalDuration = duration; // - tenacity modificators
            crowdControlCurrentTime = 0;
        }
        public void FrostPlayer(float duration)
        {
        
        }
    
        public void BurnPlayer(float duration)
        {
        
        }
    }
}
