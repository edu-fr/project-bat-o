using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game;
using Player;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;
using UI;

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

    // Start is called before the first frame update
    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerAttackManager = GetComponent<PlayerAttackManager>();
        FlurryRush = GetComponent<FlurryRush>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (LevelManager.GameIsPaused) return; 
        
        switch (State)
        {
            case States.Standard:
                PlayerController.HandleMovement();
                PlayerAttackManager.HandleAttack();
                if (FlurryRush.CanFlurryRush)
                {
                    ChangeState(States.CanRush);
                }
                break;
                
            case States.Attacking:

                break;
            
            case States.Dashing:
                PlayerController.Dash();
                break;
            
            case States.CanRush:
                PlayerController.FaceDirection();
                FlurryRush.RushHandler();
                break;
            
            case States.Rushing:
                PlayerController.FaceDirection();
                PlayerAttackManager.PlayerHealthManager.Invincible = true;
                break;
            
            case States.Frozen:
                
                break;
            
            case States.Paralyzed:
                
                break; 
            
            case States.Petrified:
                crowdControlCurrentTime += Time.deltaTime;
                if (crowdControlCurrentTime >= crowdControlTotalDuration)
                {
                    ChangeState(States.Standard);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        var joystickValue = PlayerController.Joystick.HandleRange ;
        var joystickDirection = PlayerController.Joystick.Direction;
        
        switch (State)
        {
            case States.Standard:
                /* Movement */
                PlayerController.RigidBody.velocity = joystickDirection * (PlayerController.StandardMoveSpeed * joystickValue);
                /***/
                break;

            case States.Attacking:
                /* Movement */
                PlayerController.RigidBody.velocity = joystickDirection * (PlayerController.StandardMoveSpeed * PlayerController.AttackingMoveSpeedMultiplier * joystickValue);
                /***/
                break;

            case States.Dashing:
                /* Movement */
                PlayerController.RigidBody.velocity = joystickDirection * (PlayerController.StandardMoveSpeed * PlayerController.DashCurrentMoveSpeedMultiplier);
                /***/
                break;

            case States.Rushing:
                if (!FlurryRush.hasStartedLerp)
                {
                    FlurryRush.TimeStartLerping = Time.unscaledTime;
                    FlurryRush.hasStartedLerp = true;
                }
                if (FlurryRush.RushToEnemyPosition())
                {
                    FlurryRush.FlurryAttack();
                }
                break;

            case States.Paralyzed:
                PlayerController.RigidBody.velocity = Vector2.zero;
                break;

            case States.Frozen:
                PlayerController.RigidBody.velocity = Vector2.zero;
                break;
            
            case States.Petrified:
                PlayerController.RigidBody.velocity = Vector2.zero;
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
