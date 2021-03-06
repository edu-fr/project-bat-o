﻿// This C# function can be called by an Animation Event

using UnityEngine;

namespace Animation
{
    public class AlertObservers : MonoBehaviour
    {
        private Animator Animator;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void FinishAttackAnimation()
        {
            if(Animator.GetBool("IsAttacking"))
            {
                Animator.SetBool("IsAttacking", false);
            }
        }
    }
}