using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController
{
    public class MapData
    {
        public int width;
        public int height;
        public List<JPSNode> nodeList;
    }

    private PathfindController _pathfindController = new PathfindController();
    private MapData _curMapData = new MapData();
    private Transform _mapRoot;

    private readonly Vector3 _tileStartPos = new Vector3(0, 0, 0);

    public void Init(MapData data)
    {
        _curMapData = data;
        var grid = new PathfindGrid();
        grid.Init(data.width, data.height, data.nodeList);
        _pathfindController.Init(grid);
        _pathfindController.SetDiagonalMovement(DiagonalMovement.IfAtLeastOneWalkable);
    }

    public void GenerateMap(Transform root)
    {
        _mapRoot = root;
        for (int i = 0; i < _curMapData.nodeList.Count; ++i)
        {
            var node = _curMapData.nodeList[i];
            var tile = ObjectFactory.Instance.CreateObject<TileBase>(ResourceType.Tile, "TestTile", _mapRoot);
            var tilePos = _tileStartPos;
            tilePos.x += node.X;
            tilePos.z += node.Y;
            tile.CachedTransform.position = tilePos;

            var tileInfo = new TileBase.TileInfo();
            tileInfo.nodeInfo = node;
            tile.Init(tileInfo);
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

    public void OnUpdate()
    {

    }
}
