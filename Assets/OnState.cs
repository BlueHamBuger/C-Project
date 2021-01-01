using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnState : StateMachineBehaviour
{

    private ActorController ac = null;
    public Global.BodyAction ba = Global.BodyAction.NONE;
    public Global.HandAction ha = Global.HandAction.NONE;
    
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 要求 只有 在 动画过渡完毕之后 才 切换状态

        if(!ac){ac = animator.GetComponent<ActorController>();}
        if(!ac) return;
        if(ba!= Global.BodyAction.NONE)
            ac.SetBodyAction(ba);
            //ac.SetBodyActionInTime(ba,animator.GetAnimatorTransitionInfo(0).duration/2.0F);
            //ac.setbod
        if(ha != Global.HandAction.NONE)
            ac.SetHandAction(ha);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     ac.OnSkillOver();
    // }

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
