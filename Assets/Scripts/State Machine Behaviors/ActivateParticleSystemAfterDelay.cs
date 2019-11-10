using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticleSystemAfterDelay : StateMachineBehaviour
{
    public float DelayAmount = 1; 
    private float TimePassed;
    private bool MethodCalled;
    private ParticleSystem ParticleSystem;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TimePassed = 0f;
        MethodCalled = false;
        ParticleSystem = animator.transform.parent.GetComponentInChildren<ParticleSystem>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(MethodCalled)return;
        if(TimePassed >= DelayAmount){
            MethodCalled = true;
            ParticleSystem.gameObject.SetActive(true);
        }
        TimePassed += Time.deltaTime;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
