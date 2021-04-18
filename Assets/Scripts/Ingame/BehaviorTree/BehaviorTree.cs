using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class BehaviorTree
{
    private CompositeNode _root;

    public void Init(CompositeNode root)
    {
        _root = root;
    }

    public void AddNode(BehaviorTreeNode node)
    {
        _root.AddNode(node);
    }

    public BehaviorNodeState OnUpdate()
    {
        return _root.Execute();
    }
}
