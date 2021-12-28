using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;
using UnityEngine.Assertions;

public class BehaviorTree
{
    private CompositeNode _root;
    private bool _pause;

    public void Init(CompositeNode root)
    {
        _root = root;
        _pause = false;
    }

    public void SetOwner(IBehaviorTreeOwner owner)
    {
        _root.SetOwner(owner);
    }
    
    public void AddNode(string parentName, BehaviorTreeNode node)
    {
        var targetNode = FindNode(parentName);
        if(targetNode != null)
            targetNode.AddNode(node);
    }

    public BehaviorTreeNode FindNode(string name)
    {
        var result = _root.FindNode(name);
        Assert.IsNotNull(result, $"[Failed] {name} is not exist in tree");
        return _root.FindNode(name);
    }

    public void Reset()
    {
        _root.Reset();
    }

    public void Pause(bool pause)
    {
        _pause = pause;
    }

    public BehaviorNodeState OnUpdate()
    {
        if (_pause)
            return BehaviorNodeState.Fail;
        
        return _root.Execute();
    }
}
