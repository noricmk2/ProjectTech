using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    private List<Vector3> _nextMovePath;
    
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

    public bool MoveToSearchedPath()
    {
        if (_nextMovePath != null && _nextMovePath.Count > 0)
        {
            MovePath(_nextMovePath, _characterStatus.GetStatus(StatusType.MoveSpeed));
            _nextMovePath = null;
            return true;
        }
        else
        {
            DebugEx.Log($"[Failed] there is no searched path");
            return false;
        }
    }
}
