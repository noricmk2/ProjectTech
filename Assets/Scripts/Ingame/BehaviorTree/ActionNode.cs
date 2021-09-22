using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : BehaviorTreeNode
{
    public override BehaviorNodeState OnExecute()
    {
        return base.OnExecute();
        DebugEx.Log($"{_owner} is attack");
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
        if (_owner is EnemyCharacter)
        {
            var enemy = _owner as EnemyCharacter;
            _moveStart = enemy.MoveToSearchedPath();
        }
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
        return base.OnExecute();
        DebugEx.Log($"{_owner} is hide");
    }
}

public class DeadNode : BehaviorTreeNode
{
    public override BehaviorNodeState OnExecute()
    {
        return base.OnExecute();
        DebugEx.Log($"{_owner} is dead");
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