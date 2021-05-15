using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;

public class IngameManager : MonoSingleton<IngameManager>
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
    private IngameStageMachine _stateMachine = new IngameStageMachine();
    private MapController _mapController = new MapController();
    private CharacterController _charController = new CharacterController();
    private Spawner _spawner = new Spawner();
    public Camera IngameCamera => _ingameCamera;
    #endregion

    public void Init()
    {
        InitCamera();

        var initState = new IngameStateBase();
        initState.UpdateAction = InitCheck;
        initState.StateType = IngameStageMachine.IngameState.IngameStateInit;
        var updateState = new IngameStateBase();
        updateState.UpdateAction = ControllerUpdate;
        updateState.StateType = IngameStageMachine.IngameState.IngameStateUpdate;
        _stateMachine.AddState(IngameStageMachine.IngameState.IngameStateInit, initState);
        _stateMachine.AddState(IngameStageMachine.IngameState.IngameStateUpdate, updateState);

        //Test
        var stageData = DataManager.Instance.GetStageDataByIndex(-1);
        var mapData = stageData.mapData;
        _mapController.Init(mapData);
        _mapController.GenerateMap(_mapRoot);
        _charController.Init(_characterRoot);

        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateInit);
    }

    private List<CharacterBase.CharacterInitData> CreateCharacterInitData()
    {
        return null;
    }

    private void InitCamera()
    {
        var cameraData = AddressableManager.Instance.LoadAssetSync<CameraData>("CameraData");
        _ingameCamera.transform.position = cameraData.Position;
        _ingameCamera.transform.rotation = Quaternion.Euler(cameraData.Rotate);
    }

    private void InitCheck()
    {
        //Test
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateUpdate);
    }

    private void ControllerUpdate()
    {
        _charController.OnUpdate();
        _mapController.OnUpdate();
    }

    private void Update()
    {
        _stateMachine.OnUpdate();
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
