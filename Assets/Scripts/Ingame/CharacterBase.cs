using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MoveObject, IBehaviorTreeOwner
{
    #region Define
    public class CharacterInitData
    {
        public CharacterData charData;
        public AIData aiData;
    }

    public class AIData
    {
        public CompositeNode rootNode;
    }
    #endregion

    protected CharacterStatus _characterStatus;
    protected BehaviorTree _behaviorTree = new BehaviorTree();
    protected bool _enableAI = false;
    
    public virtual void Init(CharacterInitData data)
    {
        _enableAI = false;
        _characterStatus = new CharacterStatus();
        _characterStatus.Init(data.charData.statusData);
    }

    public virtual void SetAIEnable(bool enable)
    {
        _enableAI = enable;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if(_enableAI) 
            _behaviorTree.OnUpdate();
    }

    public virtual bool FindMoveTarget()
    {
        return false;
    }
}


