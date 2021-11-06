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
}
