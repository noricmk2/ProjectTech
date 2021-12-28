using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class PlayerController
{
    private PlayerCharacter _testCharacter;
    private readonly float moveValue = 0.5f;
    private Dictionary<int, PlayerCharacter> _playerSqaud;

    public void Init()
    {
        //Test
        var initData = new CharacterBase.CharacterInitData();
        initData.charData = DataManager.Instance.CreateCharacterData(1003);
        initData.launcherTableList = new List<LauncherTable>();
        initData.launcherTableList.Add(DataManager.Instance.GetRecord<LauncherTable>(10101));
        initData.aiData = DataManager.Instance.CreateAIData("TestAI2");
        
        _testCharacter = ObjectFactory.Instance.CreateObject<PlayerCharacter>(initData.charData.resourceName, IngameManager.Instance.CharacterRoot);
        _testCharacter.CachedTransform.position = new Vector3(1, 0, 1);
        _testCharacter.Init(initData);
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
                        _testCharacter.PauseAI(true);
                        var firstNode = path.Peek();
                        _testCharacter.MovePath(Func.NodeToVectorList(path), 2, FindCover);
                        //_testCharacter.MovePath(Func.NodeToVectorList(path), firstNode.F  * moveValue);
                    }
                }
            }
        }
        _testCharacter.OnUpdate();
    }

    private void FindCover(MoveObject moveObj)
    {
        var character = moveObj as PlayerCharacter;
        var size = character.GetCharacterSize();
        var rect = new Rect(moveObj.CachedTransform.position.x - size.x * 0.5f,
            moveObj.CachedTransform.position.y - size.y * 0.5f, size.x, size.y);
        var collisionList = IngameManager.Instance.QueryRectCollision(rect);
        var obstacleList = collisionList.FindAll(x => x is ObstacleObject);
        
        if(obstacleList.Count > 0)
            character.OnCover();
        character.PauseAI(false);
    }

    public List<CharacterBase> GetPlayerCharacterList()
    {
        var list = new List<CharacterBase>();
        //Test
        list.Add(_testCharacter);
        return list;
    }
}
