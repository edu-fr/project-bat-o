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
            Petrified
        };

        private PlayerController _playerController;
        private PlayerAttackManager _playerAttackManager;
        private Animator _animator;
        public LayerMask ObjectsLayerMask;

        public States State;

        private float crowdControlTotalDuration;
        private float crowdControlCurrentTime;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();
            _playerAttackManager = GetComponent<PlayerAttackManager>();
        }
    
        private void FixedUpdate()
        {
            if (LevelManager.GameIsPaused) return; 

            switch (State)
            {
                case States.Standard:
                    _playerController.HandleMovement(1f);
                    if (_playerController.joystick.Horizontal == 0 && _playerController.joystick.Vertical == 0) // If not moving
                        _playerAttackManager.TryToAttack();
                    /***/
                    break;

                case States.Attacking:
                    if (_playerController.joystick.Horizontal != 0 || _playerController.joystick.Vertical != 0)
                        _playerAttackManager.AttackEnd();
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

                case States.Frozen:
                    State = States.Frozen;
                    break;
            
                case States.Paralyzed:
                    State = States.Paralyzed;
                    break;

                case States.Petrified:
                    State = States.Petrified;
                    _animator.speed = 0;
                
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
