using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Enemy;
using Player;
using UnityEngine;

public class FlurryRush : MonoBehaviour
{
    public PlayerController PlayerController;

    public bool CanFlurryRush;

    [SerializeField]
    private float TimeToStartRush = 2f;

    private float CurrentTimeToStartRush;

    [SerializeField]
    private float RushDuration = 5f;

    private float RushCurrentTime = 0;

    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CanFlurryRush = false;
        CurrentTimeToStartRush = TimeToStartRush;
    }

    // Update is called once per frame
    void Update()
    {

        switch (PlayerController.PlayerStateMachine.State)
        {
            case PlayerStateMachine.States.Standard:
                if (CanFlurryRush)
                {
                    TimeManager.ChangeTimeScale(0f);
                    CurrentTimeToStartRush -= Time.unscaledDeltaTime;
                    if (CurrentTimeToStartRush < 0)
                    {
                        CanFlurryRush = false;
                        CurrentTimeToStartRush = TimeToStartRush;
                        TimeManager.BackTimeToStandardFlow();
                        return;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.F)) // Start Rush
                        {
                            PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Rushing);
                            CanFlurryRush = false;
                            PlayerController.TargetedEnemy.EnemyStateMachine.ChangeState(EnemyStateMachine.States.BeenRushed);
                            PlayerController.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                            DashTowardsEnemy();
                        }
                    }
                }
                break;
            
            case PlayerStateMachine.States.Rushing:
                RushCurrentTime += Time.unscaledDeltaTime;
                if (RushCurrentTime > RushDuration)
                {
                    TimeManager.BackTimeToStandardFlow();
                    PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
                    RushCurrentTime = 0;
                    PlayerController.Animator.updateMode = AnimatorUpdateMode.Normal;
                }
                break; 
        }
    }

    private void DashTowardsEnemy()
    {
        Rigidbody2D PlayerRigidBody = PlayerController.GetComponent<Rigidbody2D>();
        // PlayerRigidBody.bodyType = RigidbodyType2D.Kinematic;
        PlayerRigidBody.MovePosition((Vector2) transform.position +
                                     Physics2D.Distance(PlayerController.PlayerCollider,
                                         PlayerController.TargetedEnemy.CircleCollider).normal *
                                     Physics2D.Distance(PlayerController.PlayerCollider,
                                         PlayerController.TargetedEnemy.CircleCollider).distance);
        
    }
}