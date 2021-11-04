using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TCUtil;
using UnityEditor;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
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
    private readonly Vector3 _tileStartPos = new Vector3(0, 0, 0);

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
            var tilePos = _tileStartPos;
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

    //TODO:엄폐우선 길찾기
    public List<Vector3> GetPathPositionListWithCover(Vector2Int start, Vector2Int dest)
    {
        return null;
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

    public List<Vector3> GetRandomPathByRange(Vector2Int start, float range)
    {
        var list = new List<Vector3>();
        var nodeList = _pathfindController.GetRandomPathInRange(start, (int)range);
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

    public Rect GetQuadTreeBoundry()
    {
        var tile = _curMapData.tileList.First();
        var size = tile.TileSize;
        var rect = new Rect(_tileStartPos.x - (size.x * 0.5f), _tileStartPos.z - (size.z * 0.5f),
            _curMapData.widthCount * size.x, _curMapData.heightCount * size.z);
        return rect;
    }
}