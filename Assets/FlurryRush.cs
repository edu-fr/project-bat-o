using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Enemy;
using Player;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FlurryRush : MonoBehaviour
{
    public PlayerController PlayerController;
    public BoxCollider2D PlayerCollider;
    public Rigidbody2D PlayerRigidBody;

    public bool CanFlurryRush;
    public Vector2 EnemyPosition;
    public Vector2 PlayerEndDashPosition;
    public float DistanceToEnemy;

    public float TimeSinceStartLerping;
    public float LerpingPercentage;

    [SerializeField]
    private float TimeToStartRush = 2f;
    private float CurrentTimeToStartRush;

    public bool isFlurryAttacking;

    // LERP TIME
    public float LerpTime = 3f;
    public float TimeStartLerping; 

    public bool DrawGizmos; 
    private void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerRigidBody = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CanFlurryRush = false;
        CurrentTimeToStartRush = TimeToStartRush;
    }

    public void RushHandler()
    {
        TimeManager.ChangeTimeScale(0.01f);
        CurrentTimeToStartRush -= Time.unscaledDeltaTime;
        if (CurrentTimeToStartRush < 0)
        {
            CanFlurryRush = false;
            CurrentTimeToStartRush = TimeToStartRush;
            RushEnd();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F)) // Start Rush
            {
                TimeManager.BackTimeToStandardFlow();
                
                // LERP 
                EnemyPosition = PlayerController.TargetedEnemy.CircleCollider.ClosestPoint(transform.position);
                TimeStartLerping = Time.unscaledTime;
                PlayerEndDashPosition = transform.position;
                
                CanFlurryRush = false;
                isFlurryAttacking = false;
                PlayerController.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Rushing);
                PlayerController.TargetedEnemy.EnemyStateMachine.ChangeState(EnemyStateMachine.States.BeenRushed);
            }
        }
    }

    public bool RushToEnemyPosition()
    {
        DistanceToEnemy = Physics2D.Distance(PlayerCollider, PlayerController.TargetedEnemy.CircleCollider).distance;
        if (DistanceToEnemy < 0.1f)
            return true;
        transform.position = (UtilitiesClass.CustomLerp(PlayerEndDashPosition, EnemyPosition, TimeStartLerping, LerpTime));
        return false;

    }

    public void FlurryAttack()
    {
        if (!isFlurryAttacking)
        {
            PlayerController.PlayerAttackManager.Attack();
            isFlurryAttacking = true;
            RushEnd();
        }
    }

    private void RushEnd()
    {
        TimeManager.BackTimeToStandardFlow();
        PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
        PlayerController.Animator.updateMode = AnimatorUpdateMode.Normal;
        if (PlayerController.TargetedEnemy.gameObject != null)
        {
            StartCoroutine(PlayerController.TargetedEnemy.EnemyStateMachine.ReturnEnemyToStateAfterSeconds(EnemyStateMachine.States.Chasing ,0.5f));
        }
        
        PlayerController.PlayerAttackManager.PlayerHealthManager.Invincible = false;
    }


    

    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            Gizmos.DrawSphere(EnemyPosition, 0.2f);
        }
    }
}