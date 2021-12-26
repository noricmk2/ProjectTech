using System;
using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyCharacter : CharacterBase
{
    [SerializeField] private Transform _hudSlot;
    
    private List<Vector3> _nextMovePath;
    private CharacterBase _attackTarget;
    private readonly Vector2Int direction = new Vector2Int(0, -1);
    private ObjectHUD _hud;
    
    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        _behaviorTree = data.aiData.behaviorTree;
        Assert.IsNotNull(_behaviorTree, $"[Failed] {data.charData.index} has no aiData");
        _behaviorTree.SetOwner(this);
        _attackTarget = null;
        _hud = IngameManager.Instance.CreateHUD(_characterStatus, _hudSlot);
    }

    public override void OnDamaged(DamageData data)
    {
        base.OnDamaged(data);
        if (_characterStatus.GetStatusValueByType(StatusType.Hp) <= 0)
        {
            DebugEx.Log($"[Dead] hp is zero");
            _behaviorTree.Reset();
        }
        _hud?.SetHP(_characterStatus.GetStatusValueByType(StatusType.Hp));
    }

    public override bool CheckDead()
    {
        return _characterStatus.GetStatusValueByType(StatusType.Hp) <= 0;
    }

    public override void OnDead(Action onDead)
    {
        base.OnDead(onDead);
        for (int i = 0; i < _laucherList.Count; ++i)
            _laucherList[i].Reset();

        if (_hud != null)
        {
            ObjectFactory.Instance.DeactivePoolObject(_hud);
            _hud = null;
        }

        SetAnimatorTrigger("Dead");
        _deadCallback = onDead;
    }

    public override void DeadAnimationEnd()
    {
        _deadCallback?.Invoke();
        _waitRemove = true;
    }

    public override bool FindAttackTarget()
    {
        _attackTarget = null;
        var attackRange = _characterStatus.GetStatusValueByType(StatusType.AtkRange);
        var list = IngameManager.Instance.GetCharacterInRange(this, attackRange, CharacterType.Player);
        for (int i = 0; i < list.Count; ++i)
        {
            //TODO: get attack target by method
            _attackTarget = list[i];
            break;
        }
        return _attackTarget != null;
    }

    public override bool FindMoveTarget()
    {
        var moveRange = _characterStatus.GetStatusValueByType(StatusType.MoveRange);
        var startPos = Func.GetTilePos(CachedTransform.position);
        var coverPoint =
            MapManager.Instance.GetCoverPointInRange(startPos, direction, moveRange);

        List<Vector3> path = null;
        if (coverPoint == MapManager.NotExistPoint)
            path = MapManager.Instance.GetPathByRange(MapManager.RangePathType.Straight, startPos, direction,
                moveRange);
        else
            path = MapManager.Instance.GetPathPositionList(startPos, coverPoint);
        if (path != null && path.Count > 0)
        {
            _nextMovePath = path;
            return true;
        }
        else
            return false;
    }

    public bool MoveToSearchedPath(Action onMoveEnd)
    {
        if (_nextMovePath != null && _nextMovePath.Count > 0)
        {
            MovePath(_nextMovePath, _characterStatus.GetStatusValueByType(StatusType.MoveSpeed), obj =>
                {
                    FindCover(obj);
                    onMoveEnd?.Invoke();
                }
            );
            _nextMovePath = null;
            return true;
        }
        else
        {
            DebugEx.Log($"[Failed] there is no searched path");
            onMoveEnd?.Invoke();
            return false;
        }
    }

    private void FindCover(MoveObject obj)
    {
    }

    public override void Fire(int launcherIndex = 0, int slotIndex = 0)
    {
        base.Fire(launcherIndex, slotIndex);
        var launcher = _laucherList[launcherIndex];
        var slot = GetLaucherSlot(slotIndex);
        launcher.Fire(slot, _attackTarget);
    }

    public override bool Attack(Action onAttackEnd)
    {
        if (_attackTarget == null)
            return false;
        
        var dir = _attackTarget.transform.position - CachedTransform.position;
        var radian = Vector3.Dot(dir.normalized, CachedTransform.forward);
        float sight = Mathf.Cos(5f * Mathf.Deg2Rad);
        
        if (radian < sight)
        {
            LookAt(_attackTarget.transform);
        }
        else
        {
            if (_delayDeltaTime <= 0)
            {
                // TODO : do attack;
                Fire();
                onAttackEnd?.Invoke();
                _delayDeltaTime = defaultAttackTerm;
                return true;
            }          
        }
        return false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_delayDeltaTime > 0)
        {
            _delayDeltaTime -= Time.deltaTime * _characterStatus.GetStatusValueByType(StatusType.AtkSpeed);
        }
    }

    public override void Release()
    {
        base.Release();
        if (_hud != null)
        {
            ObjectFactory.Instance.DeactivePoolObject(_hud);
            _hud = null;
        }
    }
}
