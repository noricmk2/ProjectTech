using System;
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

    #region Inspector
    [SerializeField] private Animator _animator;
    #endregion
    
    protected CharacterStatus _characterStatus;
    protected BehaviorTree _behaviorTree = new BehaviorTree();
    protected bool _enableAI = false;
    protected float _delayDeltaTime;

    protected readonly float _defaultAttackTerm = 1f;
    
    public virtual void Init(CharacterInitData data)
    {
        _enableAI = false;
        _characterStatus = new CharacterStatus();
        _characterStatus.Init(this, data.charData.statusData);
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
    
    public virtual bool FindAttackTarget()
    {
        return false;
    }

    public virtual bool Attack(Action onAttackEnd)
    {
        return false;
    }

    public virtual void OnDamaged(DamageData data)
    {
        _characterStatus?.CalDamage(data);
    }

    public virtual void OnDead(Action onDead)
    {
    }

    public virtual void Release()
    {
        _delayDeltaTime = 0;
    }
}


