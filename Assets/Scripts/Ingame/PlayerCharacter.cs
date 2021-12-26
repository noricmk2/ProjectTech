using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCharacter : CharacterBase
{
    private CharacterBase _attackTarget;
    
    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        if (data.aiData != null)
        {
            _behaviorTree = data.aiData.behaviorTree;
            Assert.IsNotNull(_behaviorTree, $"[Failed] {data.charData.index} has no aiData");
            _behaviorTree.SetOwner(this);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_curState == MoveState.Stational && FindAttackTarget())
        {
            Attack(null);
        }
        
        if (_delayDeltaTime > 0)
        {
            _delayDeltaTime -= Time.deltaTime * _characterStatus.GetStatusValueByType(StatusType.AtkSpeed);
        }
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
                Fire();
                onAttackEnd?.Invoke();
                _delayDeltaTime = defaultAttackTerm;
                return true;
            }          
        }
        return false;
    }

    public override void MovePath(List<Vector3> path, float speed, Action<MoveObject> onPathEnd, bool withRotate = true)
    {
        for(int i=0; i<_laucherList.Count; ++i)
            _laucherList[i].Reset();
        base.MovePath(path, speed, onPathEnd, withRotate);
    }

    public override void Fire(int launcherIndex = 0, int slotIndex = 0)
    {
        base.Fire(launcherIndex, slotIndex);
        var launcher = _laucherList[launcherIndex];
        var slot = GetLaucherSlot(slotIndex);
        launcher.Fire(slot, _attackTarget);
    }

    public override bool FindAttackTarget()
    {
        _attackTarget = null;
        var attackRange = _characterStatus.GetStatusValueByType(StatusType.AtkRange);
        var list = IngameManager.Instance.GetCharacterInRange(this, attackRange, CharacterType.Enemy);
        list.Sort((x, y) =>
        {
            var xDist = (CachedTransform.position - x.CachedTransform.position).sqrMagnitude;
            var yDist = (CachedTransform.position - y.CachedTransform.position).sqrMagnitude;
            if (xDist < yDist)
                return -1;
            else if (xDist > yDist)
                return 1;
            return 0;
        });
        
        for (int i = 0; i < list.Count; ++i)
        {
            //TODO: get attack target by method
            _attackTarget = list[i];
            break;
        }
        return _attackTarget != null;
    }
}
