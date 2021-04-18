using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class CharacterController
{
    private CharacterBase _testCharacter;
    private Transform _charRoot;
    private readonly float moveValue = 0.5f;
    private BattleBlackBoard _blackBoard;

    public void Init(Transform charRoot, BattleBlackBoard blackBoard)
    {
        //TODO:블랙보드에 데이터 저장
        _blackBoard = blackBoard;
        _charRoot = charRoot;
        _testCharacter = ObjectFactory.Instance.CreateObject<CharacterBase>(ResourceType.Character, "TestCharacter", _charRoot);
        _testCharacter.CachedTransform.position = new Vector3(1, 0, 1);

        var charData = new CharacterBase.CharacterInitData();
        charData.blackBoard = blackBoard;
        charData.checkFindEnemyDelegate = FindEnemy;
        charData.checkMoveDelegate = FindMoveTarget;
        _testCharacter.Init(charData);
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
                    var path = IngameManager.Instance.GetPathNodeList(charPos, new Vector2Int(node.X, node.Y));

                    if (path != null && path.Count > 0)
                    {
                        var firstNode = path.Peek();
                        _testCharacter.MovePath(Func.NodeToVectorList(path), firstNode.F  * moveValue);
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
