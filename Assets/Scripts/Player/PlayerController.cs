using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        private Animator Animator;
        private Rigidbody2D RigidBody;
        [SerializeField]
        private float Speed;

    
        float MoveX;
        float MoveY;
        #endregion

        #region Unity Callbacks

        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponent<Animator>();
            RigidBody = GetComponent<Rigidbody2D>();
            Speed = 5f;
        }

        // Update is called once per frame
        void Update()
        {

            HandleMovement();
        }
        #endregion

        #region Auxiliar Methods
        private void HandleMovement()
        {
            RigidBody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;
            Animator.SetFloat("moveX", RigidBody.velocity.x);
            Animator.SetFloat("moveY", RigidBody.velocity.y);

            if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1)
            {
                Animator.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
            }

            if (Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
            {
                Animator.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
            }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion
    }
}

