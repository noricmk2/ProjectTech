using System;
using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    private List<Vector3> _nextMovePath;
    private CharacterBase _attackTarget;
    
    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        var aiRootNode = data.aiData.rootNode;
        if (aiRootNode == null)
            DebugEx.LogError($"[Failed] {data.charData.index} has no aiData");
        else
        {
            aiRootNode.SetOwner(this);
            _behaviorTree.Init(aiRootNode);
        }

        _attackTarget = null;
        gameObject.name = string.Concat(gameObject.name, GetInstanceID());
    }

    public override bool FindAttackTarget()
    {
        _attackTarget = null;
        var attackRange = _characterStatus.GetStatus(StatusType.AtkRange);
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
        var moveRange = _characterStatus.GetStatus(StatusType.MoveRange);
        var path = MapManager.Instance.GetRandomPathByRange(Func.GetTilePos(CachedTransform.position), moveRange);
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
            MovePath(_nextMovePath, _characterStatus.GetStatus(StatusType.MoveSpeed), onMoveEnd);
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

    public override bool Attack(Action onAttackEnd)
    {
        if (_attackTarget != null && _delayDeltaTime <= 0)
        {
            //TODO : do attack;
            var damage = new DamageData();
            damage.atkDamage = _characterStatus.GetStatus(StatusType.Atk);
            _attackTarget.OnDamaged(damage);
            onAttackEnd?.Invoke();
            _delayDeltaTime = _defaultAttackTerm;
            return true;
        }
        return false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_delayDeltaTime > 0)
        {
            _delayDeltaTime -= Time.deltaTime * _characterStatus.GetStatus(StatusType.AtkSpeed);
        }
    }
}
