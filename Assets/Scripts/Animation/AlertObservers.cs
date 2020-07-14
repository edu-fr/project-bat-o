// This C# function can be called by an Animation Event
using UnityEngine;
using System.Collections;


public class AlertObservers : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FinishAttackAnimation()
    {
        animator.SetBool("isAttacking", false);
        
    }
}