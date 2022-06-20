using Player;
using UnityEngine;

public class FlurryRush : MonoBehaviour
{
    // public PlayerController PlayerController;
    // public BoxCollider2D PlayerCollider;
    // public Rigidbody2D PlayerRigidBody;
    //
    // public bool CanFlurryRush;
    // public Vector2 EnemyPosition;
    // public Vector2 PlayerEndDashPosition;
    // public float DistanceToEnemy;
    //
    // [SerializeField]
    // private float TimeToStartRush = 2f;
    // private float CurrentTimeToStartRush;
    // public bool hasStartedLerp = false;
    //
    // public bool isFlurryAttacking;
    //
    // // LERP TIME
    // public float LerpTime;
    // public float TimeStartLerping; 
    //
    // public bool DrawGizmos; 
    private void Awake()
    {
    //     PlayerController = GetComponent<PlayerController>();
    //     PlayerRigidBody = GetComponent<Rigidbody2D>();
    //     PlayerCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     CanFlurryRush = false;
    //     CurrentTimeToStartRush = TimeToStartRush;
    // }

    // public void RushHandler()
    // {
    //     TimeManager.ChangeTimeScale(0.1f);
    //     CurrentTimeToStartRush -= Time.unscaledDeltaTime;
    //     if (CurrentTimeToStartRush < 0)
    //     {
    //         CanFlurryRush = false;
    //         CurrentTimeToStartRush = TimeToStartRush;
    //         RushEnd();
    //     }
    //     else
    //     {
    //         if (Input.GetKeyDown(KeyCode.Z)) // Start Rush
    //         {
    //             // TimeManager.BackTimeToStandardFlow();
    //             
    //             // LERP 
    //             EnemyPosition = PlayerController.TargetedEnemy.BoxCollider2D.ClosestPoint(transform.position);
    //             PlayerEndDashPosition = transform.position;
    //             
    //             CanFlurryRush = false;
    //             isFlurryAttacking = false;
    //             PlayerController.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    //             PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Rushing);
    //             PlayerController.CreateDust();
    //             PlayerController.TargetedEnemy.EnemyStateMachine.ChangeState(EnemyStateMachine.States.BeenRushed);
    //         }
    //     }
    // }

    // public bool RushToEnemyPosition()
    // {
    //     DistanceToEnemy = Physics2D.Distance(PlayerCollider, PlayerController.TargetedEnemy.BoxCollider2D).distance;
    //     if (DistanceToEnemy < 0.2f)
    //         return true;
    //     PlayerRigidBody.MovePosition(UtilitiesClass.CustomLerp(PlayerEndDashPosition, EnemyPosition, TimeStartLerping, LerpTime));
    //     return false;
    //
    // }

    // public void FlurryAttack()
    // {
    //     if (!isFlurryAttacking)
    //     {
    //         PlayerController.PlayerAttackManager.AnimateAttack();
    //         isFlurryAttacking = true;
    //         RushEnd();
    //     }
    // }
    //
    // private void RushEnd()
    // {
    //     TimeManager.BackTimeToStandardFlow();
    //     hasStartedLerp = false;
    //     PlayerController.PlayerStateMachine.ChangeState(PlayerStateMachine.States.Standard);
    //     PlayerController.Animator.updateMode = AnimatorUpdateMode.Normal;
    //     if (PlayerController.TargetedEnemy.gameObject != null)
    //     {
    //         StartCoroutine(PlayerController.TargetedEnemy.EnemyStateMachine.ReturnEnemyToStateAfterSeconds(EnemyStateMachine.States.Chasing ,0.5f));
    //     }
    //     StartCoroutine(PlayerController.PlayerAttackManager.PlayerHealthManager.EndInvincibilityAfterSeconds(1f));
    // }
    
    private void OnDrawGizmos()
    {
        // if (DrawGizmos)
        // {
        //     Gizmos.DrawSphere(EnemyPosition, 0.2f);
        // }
    }
}