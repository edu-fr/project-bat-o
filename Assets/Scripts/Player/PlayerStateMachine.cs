using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;

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
        Rushing
    };

    public PlayerController PlayerController;
    public PlayerAttackManager PlayerAttackManager;
    public FlurryRush FlurryRush;
    public LayerMask ObjectsLayerMask; 
    
    public States State;
    
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
                break;
            
            case States.Frozen:
                
                break;
            
            case States.Paralyzed:
                
                break; 
        }
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case States.Standard:
                /* Movement */
                PlayerController.RigidBody.velocity =
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized *
                    PlayerController.StandardMoveSpeed;
                /***/
                break;

            case States.Attacking:
                /* Movement */
                PlayerController.RigidBody.velocity =
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * (PlayerController.StandardMoveSpeed * PlayerController.AttackingMoveSpeedMultiplier);
                /***/
                break;

            case States.Dashing:
                /* Movement */
                PlayerController.RigidBody.velocity =
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * (PlayerController.StandardMoveSpeed * PlayerController.DashCurrentMoveSpeedMultiplier);
                /***/
                break;
            
            case States.Rushing:
                /*
                var nothingBetween = false;
                var elementBetweenPlayerAndEnemy = Physics2D.Raycast(transform.position,
                    Physics2D.Distance(PlayerController.TargetedEnemy.CircleCollider, PlayerController.PlayerCollider)
                        .normal, 3f, ObjectsLayerMask);
                if (elementBetweenPlayerAndEnemy.collider != null)
                {
                    if (elementBetweenPlayerAndEnemy.collider.gameObject == PlayerController.TargetedEnemy.gameObject) // if the first thing that the raycast hit is the locked enemy, the player is able to attack!
                    {
                        nothingBetween = true;
                    }
                    else
                    {
                        Debug.Log("Object found! " + elementBetweenPlayerAndEnemy.collider.gameObject.name);
                    }
                }
                */
                if (FlurryRush.RushToEnemyPosition() /* && nothingBetween */)
                {
                    FlurryRush.FlurryAttack();
                }/*
                else
                {
                    if (!nothingBetween) // If there is things between the player and the enemy, the rush end;
                    {
                        if (elementBetweenPlayerAndEnemy.collider != null)
                        {
                            Debug.Log("Object found! " + elementBetweenPlayerAndEnemy.collider.gameObject.name);
                            
                        }
                        FlurryRush.RushEnd();
                    }
                    
                }*/

                break;

            case States.Paralyzed:

                break;

            case States.Frozen:

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
        }
    }   
}
