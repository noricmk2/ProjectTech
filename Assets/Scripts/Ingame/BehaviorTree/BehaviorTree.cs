using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;
using UnityEngine.Assertions;

public class BehaviorTree
{
    private CompositeNode _root;

    public void Init(CompositeNode root)
    {
        _root = root;
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

    public BehaviorNodeState OnUpdate()
    {
        return _root.Execute();
    }
}
