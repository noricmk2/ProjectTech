using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TCUtil;
using UnityEditor;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    public enum RangePathType
    {
        Random,
        Straight
    }

    public class MapPlayData
    {
        public int widthCount;
        public int heightCount;
        public List<TileBase> tileList;
        public List<ObstacleObject> obstacleList;
    }

    #region Inspector
    [SerializeField] private Transform _mapRoot;
    #endregion

    #region Property
    private PathfindController _pathfindController = new PathfindController();
    private MapRawData _curRawData = new MapRawData();
    private MapPlayData _curMapData = new MapPlayData();
    private readonly Vector3 tileStartPos = new Vector3(0, 0, 0);
    public static readonly Vector2Int NotExistPoint = new Vector2Int(-9999, -9999);

    public MapPlayData CurrentMapData => _curMapData;
    #endregion

    public void Init(MapRawData data)
    {
        _curRawData = data;
        var grid = new PathfindGrid();
        grid.Init(data.width, data.height, data.nodeList);
        _pathfindController.Init(grid);
        _pathfindController.SetDiagonalMovement(DiagonalMovement.IfAtLeastOneWalkable);
        GenerateMap(data.width, data.height);
        // GenerateMapWithPrefabName(data.prefabName, data.width, data.height);
    }

    public void GenerateMapWithPrefabName(string prefabName,int width,int height)
    {
        _curMapData = new MapPlayData();
        _curMapData.widthCount = width;
        _curMapData.heightCount = height;
        _curMapData.tileList = new List<TileBase>();
        _curMapData.obstacleList = new List<ObstacleObject>();
        int tempSpanwerCount = 1;
        
        var mapBase = ObjectFactory.Instance.CreateObject<MapBase>(prefabName, _mapRoot);
        mapBase.transform.localPosition = Vector3.zero;
        mapBase.transform.localRotation = Quaternion.identity;
        var enumator = _curRawData.nodeList.GetEnumerator();
        enumator.MoveNext();
        foreach (Transform childItem in mapBase.TileBaseObj.transform)
        {
          //  var tileItem = childItem.GetComponent<TileBase>();
            var tileItem = childItem.GetComponent<MapEditorTile>();
            if(tileItem == null)
                continue;
            
            
            var node = enumator.Current;
            if(node == null)
                continue;
            var tileInfo = new TileBase.TileInfo();
            var tilePos = Vector3.zero;
            MapDetailTable detailRecord = null;
            tileInfo.nodeInfo = node;
            tileInfo.nodeType = (MapNodeType) node.state;
      
            tilePos.x += node.X;
            tilePos.z += node.Y;
            // tileItem.CachedTransform.position = tilePos;


            var detailIndex = tileItem.tileIndex;
            if (detailIndex > 0)
                detailRecord = DataManager.Instance.GetRecord<MapDetailTable>(detailIndex);
            else
            {
                DebugEx.LogError($"[Failed] not exist map detail data : {node.X}, {node.Y}");
                continue;
            }
            
            switch (tileInfo.nodeType)
            {
                case MapNodeType.Block:
                    break;
                case MapNodeType.Road:
                    break;
                case MapNodeType.Spanwer:
                    tileInfo.spanwerName = detailRecord.StringValue;
                    ++tempSpanwerCount;
                    break;
                case MapNodeType.PassableObstacle:
                {
                    var initData = new ObstacleObject.ObstacleInitData();
                    initData.passableObstacle = true;
                    initData.rectSize = Vector2.one;
                    initData.tilePos = node.pos;
                    initData.statusData = DataManager.Instance.CreateStatusData(detailRecord.NumberValue);
                    initData.resourceName = detailRecord.SubResourceName;
                    var obj = CreateObstacle(tilePos, initData);
                    _curMapData.obstacleList.Add(obj);
                }
                    break;
                case MapNodeType.UnpassObstacle:
                {
                    var initData = new ObstacleObject.ObstacleInitData();
                    initData.passableObstacle = false;
                    initData.rectSize = Vector2.one;
                    initData.tilePos = node.pos;
                    initData.resourceName = detailRecord.SubResourceName;
                    var obj = CreateObstacle(tilePos, initData);
                    _curMapData.obstacleList.Add(obj);
                }
                    break;
            }

            tileItem.Init(tileInfo);
            _curMapData.tileList.Add(tileItem);  

            enumator.MoveNext();
        }

    }
    //TODO: 맵 프리팹 로드 및 데이터 세팅
    public void GenerateMap(int width, int height)
    {
        _curMapData = new MapPlayData();
        _curMapData.widthCount = width;
        _curMapData.heightCount = height;
        _curMapData.tileList = new List<TileBase>();
        _curMapData.obstacleList = new List<ObstacleObject>();
        int tempSpanwerCount = 1;
        for (int i = 0; i < _curRawData.nodeList.Count; ++i)
        {
            var node = _curRawData.nodeList[i];

            MapDetailTable detailRecord = null;
            var detailIndex = _curRawData.mapDetailData[node.X, node.Y];
            if (detailIndex > 0)
                detailRecord = DataManager.Instance.GetRecord<MapDetailTable>(detailIndex);
            else
            {
                DebugEx.LogError($"[Failed] not exist map detail data : {node.X}, {node.Y}");
                continue;
            }

            var tile = ObjectFactory.Instance.CreateObject<TileBase>(detailRecord.TileResourceName, _mapRoot);
            var tilePos = tileStartPos;
            tilePos.x += node.X;
            tilePos.z += node.Y;
            tile.CachedTransform.position = tilePos;

            var tileInfo = new TileBase.TileInfo();
            tileInfo.nodeInfo = node;
            tileInfo.nodeType = (MapNodeType) node.state;

            switch (tileInfo.nodeType)
            {
                case MapNodeType.Block:
                    break;
                case MapNodeType.Road:
                    break;
                case MapNodeType.Spanwer:
                    tileInfo.spanwerName = detailRecord.StringValue;
                    ++tempSpanwerCount;
                    break;
                case MapNodeType.PassableObstacle:
                {
                    var initData = new ObstacleObject.ObstacleInitData();
                    initData.passableObstacle = true;
                    initData.rectSize = Vector2.one;
                    initData.tilePos = node.pos;
                    initData.statusData = DataManager.Instance.CreateStatusData(detailRecord.NumberValue);
                    initData.resourceName = detailRecord.SubResourceName;
                    var obj = CreateObstacle(tilePos, initData);
                    _curMapData.obstacleList.Add(obj);
                }
                    break;
                case MapNodeType.UnpassObstacle:
                {
                    var initData = new ObstacleObject.ObstacleInitData();
                    initData.passableObstacle = false;
                    initData.rectSize = Vector2.one;
                    initData.tilePos = node.pos;
                    initData.resourceName = detailRecord.SubResourceName;
                    var obj = CreateObstacle(tilePos, initData);
                    _curMapData.obstacleList.Add(obj);
                }
                    break;
            }

            tile.Init(tileInfo);
            _curMapData.tileList.Add(tile);
        }
    }

    private ObstacleObject CreateObstacle(Vector3 pos, ObstacleObject.ObstacleInitData data)
    {
        var obj = ObjectFactory.Instance.GetPoolObject<ObstacleObject>(data.resourceName);
        obj.CachedTransform.Init(_mapRoot);
        obj.CachedTransform.position = pos;
        obj.Init(data);
        IngameManager.Instance.RegistQuadTreeObject(obj);
        return obj;
    }

    public Spawner GetSpawnerByName(string name)
    {
        var targetTile =
            _curMapData.tileList.Find(x => x.CharacterSpanwer != null && x.CharacterSpanwer.SpawnerName == name);
        if (targetTile == null)
        {
            DebugEx.LogError($"[Failed] not exist spawner: {name}");
            return null;
        }
        else
        {
            return targetTile.CharacterSpanwer;
        }
    }
    
    public List<Vector3> GetPathPositionList(Vector2Int start, Vector2Int dest)
    {
        var list = new List<Vector3>();
        var nodeList = _pathfindController.FindPath(start, dest);
        if (nodeList == null)
        {
            DebugEx.Log("[Failed] no exist path");
            return null;
        }

        for (int i = 0; i < nodeList.Count; ++i)
        {
            list.Add(new Vector3(nodeList[i].X, 0, nodeList[i].Y));
        }

        return list;
    }

    public List<JPSNode> GetPathNodeList(Vector2Int start, Vector2Int dest)
    {
        return _pathfindController.FindPath(start, dest);
    }

    public List<Vector3> GetPathByRange(RangePathType type, Vector2Int start, Vector2Int dir, float range)
    {
        var list = new List<Vector3>();
        List<JPSNode> nodeList = null;

        switch (type)
        {
            case RangePathType.Random:
                nodeList = _pathfindController.GetRandomPathInRange(start, (int)range);
                break;
            case RangePathType.Straight:
                nodeList = _pathfindController.GetStraightPathInRange(start, dir, (int) range);
                break;
        }
        
        if (nodeList == null || nodeList.Count < 1)
        {
            DebugEx.Log("[Failed] no exist path");
            return null;
        }

        for (int i = 0; i < nodeList.Count; ++i)
        {
            list.Add(new Vector3(nodeList[i].X, 0, nodeList[i].Y));
        }
        return list;
    }

    public Vector2Int GetCoverPointInRange(Vector2Int start, Vector2Int dir, float range)
    {
        var nodeList = _pathfindController.GetAllNodeInRange(start, (int)range);
        var obstacleList = nodeList.FindAll(x =>
            x.state == (int) MapNodeType.PassableObstacle || x.state == (int) MapNodeType.UnpassObstacle);

        if (obstacleList == null || obstacleList.Count == 0)
            return NotExistPoint;
        
        obstacleList.Sort((x, y) =>
        {
            var distX = (start - x.pos).sqrMagnitude;
            var distY = (start - y.pos).sqrMagnitude;

            if (distX < distY)
                return -1;
            else if (distY < distX)
                return 1;

            return 0;
        });

        int index = 0;
        while (index < obstacleList.Count)
        {
            var nearestObj = obstacleList[index];
            var dest = nearestObj.pos - dir;
            if (_pathfindController.IsWalkable(dest))
                return dest;
            ++index;
        }

        return NotExistPoint;
    }

    public Rect GetQuadTreeBoundry()
    {
        var tile = _curMapData.tileList.First();
        var size = tile.TileSize;
        var rect = new Rect(tileStartPos.x - (size.x * 0.5f), tileStartPos.z - (size.z * 0.5f),
            _curMapData.widthCount * size.x, _curMapData.heightCount * size.z);
        return rect;
    }
}