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
            
            RigidBody.velocity = Animator.GetBool("IsAttacking") ? 
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed / 50 : 
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;

            Animator.SetFloat("MoveX", RigidBody.velocity.x);
            Animator.SetFloat("MoveY", RigidBody.velocity.y);

            float lastMoveX = Animator.GetFloat("LastMoveX");
            float lastMoveY = Animator.GetFloat("LastMoveY");

            if (!Animator.GetBool("IsAttacking"))
            {
                if ((Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1) && (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1))
                {
                    lastMoveX = Input.GetAxisRaw("Horizontal");
                    lastMoveY = Input.GetAxisRaw("Vertical");
                }
                else if(Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1)
                {
                    lastMoveX = Input.GetAxisRaw("Horizontal");
                    lastMoveY = 0;
                } 
                else if (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
                {
                    lastMoveX = 0;
                    lastMoveY = Input.GetAxisRaw("Vertical");
                }

            }
            
            Animator.SetFloat("LastMoveX", lastMoveX);
            Animator.SetFloat("LastMoveY", lastMoveY);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion
    }
}

