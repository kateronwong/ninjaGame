using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation_Control_N : MonoBehaviour
{
    public Vector3 rootMotionVelocity;

    private Animator animator;

    public GameObject Player;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAnimatorMove()
    {
        rootMotionVelocity = animator.deltaPosition / Time.deltaTime;
    }

    public void ResetAttack()
    { 
        //GetComponentInChildren<Sword_Combat>().ResetAttack();
    }

}
