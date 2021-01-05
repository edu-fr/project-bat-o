using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.XR;

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
    
    public States State;
    
    // Start is called before the first frame update
    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerAttackManager = GetComponent<PlayerAttackManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (State)
        {
            case States.Standard:
                PlayerController.HandleMovement();
                PlayerAttackManager.HandleAttack();
                break;
                
            case States.Attacking:

                break;
            
            case States.Dashing:
                PlayerController.Dash();
                break;
            
            case States.CanRush:
                
                break;
            
            case States.Rushing:
                
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
