using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : Singleton<IngameManager>
{
    #region Inspector
    [SerializeField]
    private Transform _mapRoot;

    [SerializeField]
    private Transform _characterRoot;

    [SerializeField]
    private Camera _ingameCamera;
    #endregion

    #region Property
    private MapController _mapController = new MapController();
    private CharacterController _charController = new CharacterController();

    private IngameState _curState;
    public IngameState CurrentIngameState => _curState;
    public Camera IngameCamera => _ingameCamera;
    #endregion

    private int[] testMap =
        {
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,0,0,0,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,0,0,1,1,
        0,1,1,1,1,1,0,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,0,1,1,1,1,1,
        0,1,1,0,0,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,1,1,1,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,0,0,0,0,
        0,1,1,1,0,0,0,0,0,0,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,1,0,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,0,0,0,1,1,1,1,1,1,
        0,1,1,1,1,1,1,0,1,1,
        0,1,1,1,1,1,1,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1
    };

    public void Init()
    {
        _curState = IngameState.Init;
        InitCamera();
        //Test
        var mapData = new MapController.MapData();
        mapData.width = 10;
        mapData.height = 50;
        mapData.nodeList = new List<JPSNode>();
        for (int i = 0; i < testMap.Length; ++i)
        {
            var node = new JPSNode(i % mapData.width, i / mapData.width, testMap[i]);
            mapData.nodeList.Add(node);
        }

        _mapController.Init(mapData);
        _mapController.GenerateMap(_mapRoot);

        _charController.Init(_characterRoot);
        _curState = IngameState.StartUpdate;
    }

    private void InitCamera()
    {
        var cameraData = ResourceManager.Instance.LoadResourceFromResources<CameraData>(ResourceType.DataAsset, "CameraData");
        _ingameCamera.transform.position = cameraData.Position;
        _ingameCamera.transform.rotation = Quaternion.Euler(cameraData.Rotate);
    }

    private void Update()
    {
        switch (_curState)
        {
            case IngameState.Init:
                break;
            case IngameState.StartUpdate:
                _mapController.OnUpdate();
                _charController.OnUpdate();
                break;
        }
    }

    public List<Vector3> GetPathPositionList(Vector2Int start, Vector2Int dest)
    {
        return _mapController.GetPathPositionList(start, dest);
    }

    public List<JPSNode> GetPathNodeList(Vector2Int start, Vector2Int dest)
    {
        return _mapController.GetPathNodeList(start, dest);
    }
}
