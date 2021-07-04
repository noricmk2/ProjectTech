using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class PlayerController
{
    private CharacterBase _testCharacter;
    private readonly float moveValue = 0.5f;
    private Dictionary<int, PlayerCharacter> _playerSqaud;

    public void Init()
    {

    }
    
    public void OnUpdate()
    {
        if (InputWrapper.Input.touchCount > 0)
        {
            var cam = IngameManager.Instance.IngameCamera;
            var touch = InputWrapper.Input.GetTouch(0);

            RaycastHit hit;
            var ray = cam.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hit, 100))
            {
                var obj = hit.collider.gameObject;
                var tile = obj.transform.parent.GetComponent<TileBase>();
                if (tile != null)
                {
                    var node = tile.Info.nodeInfo;
                    var charPos = Func.GetTilePos(_testCharacter.CachedTransform.position);
                    var path = MapManager.Instance.GetPathNodeList(charPos, new Vector2Int(node.X, node.Y));

                    if (path != null && path.Count > 0)
                    {
                        var firstNode = path.Peek();
                        _testCharacter.MovePath(Func.NodeToVectorList(path), 2);
                        //_testCharacter.MovePath(Func.NodeToVectorList(path), firstNode.F  * moveValue);
                    }
                }
            }
        }
    }

    public bool FindEnemy(IBehaviorTreeOwner owner)
    {
        return false;
    }

    public bool FindMoveTarget(IBehaviorTreeOwner owner)
    {
        return false;
    }
}
