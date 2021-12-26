using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DeadAnimationBehavior : StateMachineBehaviour
{
    private bool _animationEnd;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex, controller);
        _animationEnd = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex, controller);
        if (stateInfo.normalizedTime >= 1 && !_animationEnd)
        {
            _animationEnd = true;
            var owner = animator.gameObject.GetComponent<CharacterBase>();
            owner?.DeadAnimationEnd();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
    }
}
