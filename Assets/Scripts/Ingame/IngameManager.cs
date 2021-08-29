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
    private PlayerController _charController = new PlayerController();
    private WaveController _waveController = new WaveController();
    public Camera IngameCamera => _ingameCamera;
    public Transform CharacterRoot => _characterRoot;
    #endregion

    public void Init()
    {
        InitCamera();
        InitIngameState();
       
        //Test
        var stageData = DataManager.Instance.GetStageDataByIndex(1);
        var mapData = stageData.mapData;
        MapManager.Instance.Init(mapData);
        _charController.Init();
        _waveController.Init(stageData.waveList);
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateInit);
    }

    private void InitIngameState()
    {
        var initState = new IngameStateBase();
        initState.UpdateAction = InitCheck;
        initState.StateType = IngameStageMachine.IngameState.IngameStateInit;
        var updateState = new IngameStateBase();
        updateState.UpdateAction = ControllerUpdate;
        updateState.StateType = IngameStageMachine.IngameState.IngameStateUpdate;
        _stateMachine.AddState(IngameStageMachine.IngameState.IngameStateInit, initState);
        _stateMachine.AddState(IngameStageMachine.IngameState.IngameStateUpdate, updateState);
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
        _waveController.OnUpdate();
    }

    private void Update()
    {
        _stateMachine.OnUpdate();
    }
}
