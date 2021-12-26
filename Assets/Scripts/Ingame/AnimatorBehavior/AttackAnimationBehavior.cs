using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AttackAnimationBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex, controller);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex, controller);
        if (stateInfo.normalizedTime >= 1)
        {
            var owner = animator.gameObject.GetComponent<CharacterBase>();
            owner?.SetAnimatorTrigger("Idle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
    }
}