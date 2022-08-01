using System.Collections.Generic;
using Pathfinding;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyMovementHandler : MonoBehaviour
    {
        // Components
        public BoxCollider2D boxCollider2D;
        public BoxCollider2D protectorCollider;
        public AIDestinationSetter aiDestinationSetter;
        public AIPath aiPath;
        public EnemyStateMachine enemyStateMachine;
        public EnemyAnimationController enemyAnimationController;
        public EnemyStatsManager enemyStats;

        // Movement
        [SerializeField] [Range(0f, 5f)] private float averageTimeWaitingToMove; // time to move to the next random spot
        [SerializeField] [Range(0f, 7.5f)] private float walkableRange; // Distance it can walk while isn't chasing the player 
        private float _timeWaitingToMove; // time to move to the next random spot
        private Vector3 _homePosition; // original position on the level
        private float _currentTimer;
        public Transform target;

        // Searching for player
        public FieldOfView fieldOfViewComponent;
        public GameObject TargetPlayer { get; set; }
        private GameObject _player;
        private PlayerStateMachine _playerStateMachine;
        public float surroundingDistance = 2f;
        
        private Vector3 _currentDirection;
        private float _curAngle;

        // Ally search
        private float _searchForAlliesRange = 5f;
        
        [SerializeField] private LayerMask enemiesLayer;
        
        
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerStateMachine = _player.GetComponent<PlayerStateMachine>();

            target = new GameObject("Target of " + gameObject.name).transform;
            target.parent = GameObject.FindGameObjectWithTag("EnemiesTargetsParent").transform; 
            
            aiPath.maxSpeed = enemyStats.MoveSpeed;
            
            // Set the first random target movement 
            target.transform.position = GenerateNewTarget();
            aiDestinationSetter.target = target.transform;

            // config field of view component
            fieldOfViewComponent.SetFieldOfView(enemyStats.FieldOfViewValue);
            fieldOfViewComponent.SetViewDistance(enemyStats.FieldOfViewDistance);
            fieldOfViewComponent.SetMyMovementHandler(this);
            fieldOfViewComponent.SetOrigin(transform.position);
        }

        // Fixed Update its used to treat physics matters
        private void FixedUpdate()
        {
            if(enemyStateMachine.IsWalkingAround)
                WalkAround();
        }
        
        public void WalkAround()
        {
            // Generate new target every maxTimer (three seconds)

            if (aiPath.reachedEndOfPath) //if reached desired location, wait some seconds and move to another
            {
                // Animate standing still
                enemyAnimationController.StopMoving();
                
                _currentTimer += Time.deltaTime;
                if (_currentTimer >= _timeWaitingToMove)
                {
                    _currentTimer = 0;
                    Random.InitState((int) Time.realtimeSinceStartup); // Randomize enemy standing still time
                    target.transform.position = GenerateNewTarget();
                    AstarPath.active.Scan();
                }
            }
            else 
            {
                // Animate movement
                enemyAnimationController.AnimateMovement(aiPath.desiredVelocity.x, aiPath.desiredVelocity.y);
            }
        }

        private Vector3 GenerateNewTarget()
        {
            _timeWaitingToMove = Random.Range(averageTimeWaitingToMove - 1.5f, averageTimeWaitingToMove + 1.5f);
            var position = transform.position;
            return new Vector3(position.x + (walkableRange * Random.Range(0.3f, 1f) * Random.Range(-1, 2)), position.y + (walkableRange * Random.Range(0.3f, 1f) * Random.Range(-1, 2)), position.z);
        }

        public void SetTargetPlayer(GameObject player)
        {
            this.TargetPlayer = player;
        }

        public void CheckSurroundings()
        {
            if (_player)
            {
                if(Vector2.Distance(_player.transform.position, this.transform.position) < surroundingDistance)
                {
                    TargetPlayer = _player;
                }
            }
        }

        public void SearchForAlliesActivityNearby() // if there is an ally nearby fighting, start to chase the player too
        {
            Collider2D[] results = new Collider2D[] { };
            Physics2D.OverlapCircleNonAlloc(transform.position, enemyStats.SearchForAlliesRange, results, enemiesLayer);
            foreach (Collider2D ally in results)
            {
                if (ally.GetComponent<EnemyStateMachine>().State is EnemyStateMachine.States.Chasing
                    or EnemyStateMachine.States.PreparingAttack or EnemyStateMachine.States.Attacking)
                {
                    TargetPlayer = ally.GetComponent<EnemyMovementHandler>().TargetPlayer;
                }
            }
        }

    }
}
