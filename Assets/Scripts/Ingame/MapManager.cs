using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    public class MapPlayData
    {
        public List<TileBase> tileList;
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
        GenerateMap();
    }

    //TODO: 맵 프리팹 로드 및 데이터 세팅
    public void GenerateMap()
    {
        _curMapData = new MapPlayData();
        _curMapData.tileList = new List<TileBase>();
        int tempSpanwerCount = 1;
        for (int i = 0; i < _curRawData.nodeList.Count; ++i)
        {
            var node = _curRawData.nodeList[i];
            var tile = ObjectFactory.Instance.CreateObject<TileBase>("TestTile", _mapRoot);
            var tilePos = _tileStartPos;
            tilePos.x += node.X;
            tilePos.z += node.Y;
            tile.CachedTransform.position = tilePos;

            var tileInfo = new TileBase.TileInfo();
            tileInfo.nodeInfo = node;
            tileInfo.nodeType = (MapNodeType) node.state;
            if (tileInfo.nodeType == MapNodeType.Spanwer)
            {
                tileInfo.spanwerName = $"Spawner{tempSpanwerCount}";
                ++tempSpanwerCount;
            }
            tile.Init(tileInfo);
            _curMapData.tileList.Add(tile);
        }
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
}