using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : BehaviorTreeNode
{
    private bool _attackStart;
    private bool _attackEnd;
    
    protected override void OnActivate()
    {
        base.OnActivate();
        
        _attackStart = true;
        _attackEnd = false;
    }

    public override BehaviorNodeState OnExecute()
    {
        Blackboard.OnAIAttack(_owner, () => _attackEnd = true);
        if (_attackEnd)
            return BehaviorNodeState.Success;
        return BehaviorNodeState.Running;
    }
}

public class ExcuteSkillNode : BehaviorTreeNode
{
    public override BehaviorNodeState OnExecute()
    {
        return base.OnExecute();
        DebugEx.Log($"{_owner} is excute skill");
    }
}

public class MoveNode : BehaviorTreeNode
{
    private bool _moveStart;
    private bool _moveEnd;
    
    protected override void OnActivate()
    {
        base.OnActivate();
        _moveStart = false;
        _moveEnd = false;
        _moveStart = Blackboard.OnAIMove(_owner, () => _moveEnd = true);
    }

    public override BehaviorNodeState OnExecute()
    {
        if (_moveEnd)
            return BehaviorNodeState.Success;
        if (_moveStart)
            return BehaviorNodeState.Running;
        else
            return BehaviorNodeState.Fail;
        
        DebugEx.Log($"{_owner} is move");
    }
}

public class HideNode : BehaviorTreeNode
{
    public override BehaviorNodeState OnExecute()
    {
        DebugEx.Log($"{_owner} is hide");
        return base.OnExecute();
    }
}

public class DeadNode : BehaviorTreeNode
{
    private bool _deadEnd;
    public override BehaviorNodeState OnExecute()
    {
        DebugEx.Log($"{_owner} is dead");
        var character = _owner as CharacterBase;
        if (character != null && !_deadEnd)
        {
            character.OnDead(()=>_deadEnd = true);
            return BehaviorNodeState.Running;
        }
        else if (character != null && _deadEnd)
            return BehaviorNodeState.Success;
        else
            return BehaviorNodeState.Fail;
    }
}

public class IdleNode : BehaviorTreeNode
{
    public override BehaviorNodeState OnExecute()
    {
        return base.OnExecute();
        DebugEx.Log($"{_owner} is idle");
    }
}