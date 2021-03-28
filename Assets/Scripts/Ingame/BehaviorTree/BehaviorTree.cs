using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class BehaviorTree
{
    private SelectorNode _root;

    public void Init()
    {

    }

    public void Execute()
    {
        _root.Execute();
    }
}
