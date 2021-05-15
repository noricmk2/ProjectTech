using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviorTreeOwner
{
}

public enum BehaviorNodeState
{
    Success,
    Fail,
    Running,
}

public class BehaviorTreeNode
{
    protected BehaviorTreeNode _parent;
    protected bool _isActivate;
    protected IBehaviorTreeOwner _owner;
    public IBehaviorTreeOwner Owner => _owner;

    public void SetOwner(IBehaviorTreeOwner owner)
    {
        _owner = owner;
    }

    public virtual void AddNode(BehaviorTreeNode node)
    {
        node.SetParent(this);
    }

    public void SetParent(BehaviorTreeNode node)
    {
        _parent = node;
        _owner = _parent.Owner;
    }

    public void Activate()
    {
        if (_isActivate)
            return;
        _isActivate = true;
        OnActivate();
    }

    protected virtual void OnActivate()
    {

    }

    public BehaviorNodeState Execute()
    {
        if (!_isActivate)
            Activate();

        var state = OnExecute();
        if (state != BehaviorNodeState.Running)
            Deactivate();

        return state;
    }

    public virtual BehaviorNodeState OnExecute()
    {
        return BehaviorNodeState.Success;
    }

    public void Deactivate()
    {
        if (!_isActivate)
            return;
        _isActivate = false;
        OnDeactivate();
    }

    protected virtual void OnDeactivate()
    {

    }

    public virtual void Release()
    {
        _owner = null;
        _parent = null;
    }
}

#region Composite
public class CompositeNode : BehaviorTreeNode
{
    protected int _curStep;
    protected BehaviorTreeNode _curNode;
    protected List<BehaviorTreeNode> _childList = new List<BehaviorTreeNode>();

    public override void AddNode(BehaviorTreeNode node)
    {
        base.AddNode(node);
        _childList.Add(node);
    }

    public void ClearChild()
    {
        for (int i = 0; i < _childList.Count; ++i)
            _childList[i].Release();
        _childList.Clear();
    }

    public override void Release()
    {
        ClearChild();
        base.Release();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        _curStep = 0;
        _curNode = null;
        for (int i = 0; i < _childList.Count; ++i)
            _childList[i].Deactivate();
    }
}

public class SequenceNode : CompositeNode
{
    public override BehaviorNodeState OnExecute()
    {
        if (_childList.Count == 0)
            return BehaviorNodeState.Success;

        if (_childList.Count <= _curStep)
        {
            Debug.LogError("[Failed]" + typeof(SequenceNode) + "child list out of range:" + _curStep);
            return BehaviorNodeState.Fail;
        }

        _curNode = _childList[_curStep];
        var state = _curNode.Execute();

        switch (state)
        {
            case BehaviorNodeState.Success:
                ++_curStep;
                if (_curStep >= _childList.Count)
                {
                    _curStep = 0;
                    _curNode = null;
                    return state;
                }
                else
                    return BehaviorNodeState.Running;
            case BehaviorNodeState.Fail:
            case BehaviorNodeState.Running:
                return state;
        }

        return BehaviorNodeState.Fail;
    }
}

public class SelectorNode : CompositeNode
{
    public override BehaviorNodeState OnExecute()
    {
        if (_childList.Count == 0)
            return BehaviorNodeState.Success;

        if (_childList.Count <= _curStep)
        {
            Debug.LogError("[Failed]" + typeof(SelectorNode) + "child list out of range:" + _curStep);
            return BehaviorNodeState.Fail;
        }

        _curNode = _childList[_curStep];
        var state = _curNode.Execute();

        switch (state)
        {
            case BehaviorNodeState.Success:
                return state;
            case BehaviorNodeState.Fail:
                ++_curStep;
                if (_curStep >= _childList.Count)
                {
                    _curStep = 0;
                    _curNode = null;
                    return state;
                }
                else
                    return BehaviorNodeState.Running;
            case BehaviorNodeState.Running:
                return state;
        }

        return BehaviorNodeState.Fail;
    }
}

public class RandomSelector : SelectorNode
{
    public override BehaviorNodeState OnExecute()
    {
        if (_childList.Count == 0)
            return BehaviorNodeState.Success;

        if (_childList.Count <= _curStep)
        {
            Debug.LogError("[Failed]" + typeof(RandomSelector) + "child list out of range:" + _curStep);
            return BehaviorNodeState.Fail;
        }

        if(_curNode == null)
        {
            var rand = Random.Range(0, _childList.Count);
            _curNode = _childList[rand];
        }

        var state = _curNode.Execute();
        switch (state)
        {
            case BehaviorNodeState.Success:
                return state;
            case BehaviorNodeState.Fail:
                _curNode = null;
                return state;
            case BehaviorNodeState.Running:
                return state;
        }

        return BehaviorNodeState.Fail;
    }
}
#endregion

#region Decorator
public class DecoratorNode : BehaviorTreeNode
{
    protected BehaviorTreeNode _child;

    public override void AddNode(BehaviorTreeNode node)
    {
        base.AddNode(node);
        _child = node;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (_child != null)
            _child.Deactivate();
    }

    public override void Release()
    {
        if (_child != null)
            _child.Release();
        base.Release();
    }
}

public class Repeater : DecoratorNode
{
    private int _repeatLimit;
    private int _curRepeatCount;

    public void SetCount(int limit)
    {
        _repeatLimit = limit;
        _curRepeatCount = 0;
    }

    public override BehaviorNodeState OnExecute()
    {
        if (_child == null)
            return BehaviorNodeState.Success;

        var state = _child.Execute();

        switch (state)
        {
            case BehaviorNodeState.Success:
                ++_curRepeatCount;
                if (_curRepeatCount >= _repeatLimit)
                {
                    _curRepeatCount = 0;
                    return state;
                }
                else
                    return BehaviorNodeState.Running;
            case BehaviorNodeState.Fail:
            case BehaviorNodeState.Running:
                return BehaviorNodeState.Running;
        }

        return BehaviorNodeState.Fail;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        _curRepeatCount = 0;
        _repeatLimit = 0;
    }
}

public class ConditionNode : DecoratorNode
{
    public delegate bool ConditionCheckDelegate(IBehaviorTreeOwner owner);
    public ConditionCheckDelegate ConditionCheckFunc { private get; set; }

    public override BehaviorNodeState OnExecute()
    {
        if (ConditionCheckFunc(_owner))
        {
            return _child.Execute();
        }
        else
            return BehaviorNodeState.Fail;
    }
}
#endregion
