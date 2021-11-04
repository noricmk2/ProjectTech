using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterBase : MoveObject, IBehaviorTreeOwner, IPoolObjectBase
{
    #region Define
    public class CharacterInitData
    {
        public CharacterData charData;
        public AIData aiData;
        public List<LauncherTable> launcherTableList;
    }

    public class AIData
    {
        public CompositeNode rootNode;
    }
    #endregion

    #region Inspector
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform[] _laucherSlots;
    #endregion
    
    protected CharacterStatus _characterStatus;
    protected BehaviorTree _behaviorTree = new BehaviorTree();
    protected bool _enableAI = false;
    protected bool _waitRemove;
    protected float _delayDeltaTime;
    protected List<ActiveSkillBase> _ownSkillList = new List<ActiveSkillBase>();
    protected List<Launcher> _laucherList = new List<Launcher>();
    protected CharacterType _curCharType;

    protected readonly float _defaultAttackTerm = 1f;

    public bool WaitRemove
    {
        get => _waitRemove;
    }

    public virtual void Init(CharacterInitData data)
    {
        _curCharType = data.charData.characterType;
        _waitRemove = false;
        _enableAI = false;
        _characterStatus = new CharacterStatus();
        _characterStatus.Init(this, data.charData.statusData);
        for (int i = 0; i < data.launcherTableList.Count; ++i)
        {
            var launcher = new Launcher();
            var initData = new Launcher.LauncherInitData();
            initData.owner = this;
            initData.tableData = data.launcherTableList[i];
            launcher.Init(initData);
            _laucherList.Add(launcher);
        }
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
        for (int i = 0; i < _laucherList.Count; ++i)
        {
            _laucherList[i].OnUpdate();
        }
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

    public virtual void ExecuteActiveSkill(int idx)
    {
    }

    public CharacterStatus GetStatus()
    {
        return _characterStatus;
    }

    public virtual void OnDamaged(DamageData data)
    {
        _characterStatus?.CalDamage(data);
    }

    public virtual void OnDead(Action onDead)
    {
    }

    public void SetWaitRemove()
    {
        _waitRemove = true;
    }

    public virtual void Release()
    {
        _delayDeltaTime = 0;
    }

    public void PushAction()
    {
        gameObject?.SetActive(false);
    }

    public void PopAction()
    {
        gameObject?.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        if(gameObject != null) 
            return gameObject;
        return null;
    }

    public Transform GetLaucherSlot(int index = 0)
    {
        if (_laucherSlots.Length <= index)
        {
            DebugEx.LogError($"[Failed] index:{index} is not exist launcher slot");
            return transform;
        }

        return _laucherSlots[index];
    }

    public virtual void Fire(int launcherIndex = 0, int slotIndex = 0)
    {
    }

    public virtual void LookAt(Transform target, Action endAction = null)
    {
        var targetRot = Quaternion.LookRotation(target.position - CachedTransform.position);
        CachedTransform.rotation = Quaternion.Slerp(CachedTransform.rotation, targetRot, Time.deltaTime * 2);
    }

    public virtual void OnCover()
    {
        DebugEx.Log($"[Debug] {this} on cover");
    }
    
    public virtual CharacterType GetCharacterType()
    {
        return _curCharType;
    }

    public virtual Vector2 GetCharacterSize()
    {
        return Vector2.one;
    }
}


