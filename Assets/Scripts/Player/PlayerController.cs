using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        private Animator Animator;
        private Rigidbody2D RigidBody;
        private PlayerAttackManager PlayerAttackManager;
        
        [SerializeField]
        private float Speed;


        private float MoveX;
        private float MoveY;
        #endregion

        #region Unity Callbacks

        // Start is called before the first frame update
        private void Start()
        {
            Animator = GetComponent<Animator>();
            RigidBody = GetComponent<Rigidbody2D>();
            PlayerAttackManager = GetComponent<PlayerAttackManager>();
            Speed = 3f;
            
        }

        // Update is called once per frame
        private void Update()
        {
            HandleMovement();
        }
        #endregion

        #region Auxiliar Methods
        private void HandleMovement()
        {
            RigidBody.velocity = PlayerAttackManager.IsAttacking ? 
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed / 2 : 
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;

            Animator.SetFloat("MoveX", RigidBody.velocity.x);
            Animator.SetFloat("MoveY", RigidBody.velocity.y);

            if (!PlayerAttackManager.IsAttacking)
            {
                if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
                {
                    Animator.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));                    
                    Animator.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));                    
                }
                

            }
            
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion
    }
}

