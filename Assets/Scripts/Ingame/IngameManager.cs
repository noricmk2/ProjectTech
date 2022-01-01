using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TCUtil;

public class IngameManager : MonoSingleton<IngameManager>
{
    #region Define
    public class GameEndData
    {
        public bool victory;
    }
    #endregion
    
    #region Inspector
    [SerializeField] private Transform _mapRoot;
    [SerializeField] private Transform _characterRoot;
    [SerializeField] private Transform _proejctileRoot;
    [SerializeField] private Camera _ingameCamera;
    [SerializeField] private IngameCameraMove _cameraMove;
    #endregion

    #region Property
    private IngameStageMachine _stateMachine = new IngameStageMachine();
    private PlayerController _charController = new PlayerController();
    private ProjectileController _projectileController = new ProjectileController();
    private WaveController _waveController = new WaveController();
    private QuadTreeController _quadTreeController = new QuadTreeController();
    private UIIngameController _uiIngameController;
    private Queue<Action> _initActionQueue = new Queue<Action>();
    private Action _currentInitAction;
    private int _currentStageIndex;
    private StageData _currentStageData;

    public IngameCameraMove CameraMove => _cameraMove;
    public Camera IngameCamera => _ingameCamera;
    public Transform CharacterRoot => _characterRoot;
    public Transform ProjectileRoot => _proejctileRoot;

    private bool _initComplete;
    #endregion

    public void Init()
    {
        _initComplete = false;
        //Temp
        _currentStageIndex = 4;
        _currentStageData = DataManager.Instance.GetStageDataByIndex(_currentStageIndex);

        _currentInitAction = null;
        InitIngameState();
        SetInitAction();
    }

    private void SetInitAction()
    {
        _initActionQueue.Clear();
        _initActionQueue.Enqueue(InitCamera);
        _initActionQueue.Enqueue(InitMap);
        _initActionQueue.Enqueue(InitUI);
        _initActionQueue.Enqueue(InitCharacter);
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateInit); 
    }

    public void OnEndInitJob()
    {
        if (_initActionQueue.Count > 0)
        {
            _initActionQueue.Dequeue();
            _currentInitAction = null;
        }
        
        if(_initActionQueue.Count == 0)
            _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateStart);
    }
    
    private void InitMap()
    {
        //TODO:타일사이즈 및 맵크기 계산
        var mapData = _currentStageData.mapData;
        var size = Vector3.one;
        var tileStartPos = Vector3.zero;
        var rect = new Rect(tileStartPos.x - (size.x * 0.5f), tileStartPos.z - (size.z * 0.5f),
            mapData.width * size.x, mapData.height * size.z);
        _quadTreeController.Init(rect);
        MapManager.Instance.Init(mapData);
    }

    private void InitCharacter()
    {
        _charController.Init();
        _waveController.Init(_currentStageData.waveList);
        _projectileController.Init();
        OnEndInitJob();
    }

    private void InitUI()
    {
        _uiIngameController = UIManager.Instance.OpenUI<UIIngameController>();
        OnEndInitJob();
    }

    private void InitIngameState()
    {
        var initState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateInit);
        initState.UpdateAction = ProgressInit;
        var startState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateStart);
        startState.UpdateAction = IngameStart;
        var updateState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateUpdate);
        updateState.UpdateAction = ControllerUpdate;
        var endState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateEnd);
        endState.UpdateAction = OnEnd;
        endState.EnterAction = StageEndAction;
        
        _stateMachine.AddState(initState);
        _stateMachine.AddState(startState);
        _stateMachine.AddState(updateState);
        _stateMachine.AddState(endState);
    }
    
    private void IngameStart()
    {
        UIManager.Instance.SetLoadingView(false);
        var followData = new IngameCameraMove.FollowMoveData();
        followData.followTargets = new List<Transform>();
        var charList = _charController.GetPlayerCharacterList();
        for (int i = 0; i < charList.Count; ++i)
        {
            followData.followTargets.Add(charList[i].transform);
        }

        followData.lerpValue = 0.1f;
        followData.maxZoomValue = 10f;
        followData.minZoomValue = 5f;
        _cameraMove.SetFollowData(followData);
        _cameraMove.StartFollow();
        
        _uiIngameController.ShowUI();
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateUpdate);
    }

    private void InitCamera()
    {
        var cameraData = AddressableManager.Instance.LoadAssetSync<CameraData>("CameraData");
        _ingameCamera.transform.position = cameraData.Position;
        _ingameCamera.transform.rotation = Quaternion.Euler(cameraData.Rotate);
        OnEndInitJob();
    }

    private void ProgressInit()
    {
        if (_currentInitAction == null && _initActionQueue.Count > 0)
        {
            _currentInitAction = _initActionQueue.Peek();
            _currentInitAction?.Invoke();
        }
    }

    private void ControllerUpdate()
    {
        _charController.OnUpdate();
        _waveController.OnUpdate();
        _projectileController.OnUpdate();
        _quadTreeController.OnUpdate();
    }

    private void StageEndAction()
    {
    }

    private void OnEnd()
    {
    }

    private void Update()
    {
        _stateMachine.OnUpdate();
    }

    public void SetIngameEnd(GameEndData data)
    {
        HttpRequest.SendEndIngame(data.victory, res =>
        {
            _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateEnd);
            UIManager.Instance.CloseCurrentUI();
            _uiIngameController.ShowResult(data.victory);
        });
    }

    public void RegistQuadTreeObject(IQuadTreeObject obj)
    {
        _quadTreeController.RegisterQuadTreeObject(obj);
    }

    public List<IQuadTreeObject> QueryRectCollision(Rect rect)
    {
        return _quadTreeController.QueryRect(rect);
    }

    public List<CharacterBase> GetCharacterInRange(CharacterBase source, float range, CharacterType findType = CharacterType.None)
    {
        var sourcePos = source.CachedTransform.position;
        var list = new List<CharacterBase>();
        switch (findType)
        {
            case CharacterType.Enemy:
                var enemyList = _waveController.GetActivateEnemyList();
                for (int i = 0; i < enemyList.Count; ++i)
                {
                    if(Func.InRange(sourcePos, enemyList[i].CachedTransform.position, range))
                        list.Add(enemyList[i]);
                }
                break;
            case CharacterType.Player:
                var playerList = _charController.GetPlayerCharacterList();
                for (int i = 0; i < playerList.Count; ++i)
                {
                    if(Func.InRange(sourcePos, playerList[i].CachedTransform.position, range))
                        list.Add(playerList[i]);
                }
                break;
            case CharacterType.None:
                var allList = _waveController.GetActivateEnemyList();
                allList.AddRange(_charController.GetPlayerCharacterList());
                for (int i = 0; i < allList.Count; ++i)
                {
                    if(Func.InRange(sourcePos, allList[i].CachedTransform.position, range))
                        list.Add(allList[i]);
                }
                break;
        }
        return list;
    }

    public void RegistProjectile(ProjectileObject obj)
    {
        _projectileController.AddProjectile(obj);
    }

    public ObjectHUD CreateHUD(CharacterStatus status, Transform target)
    {
        return _uiIngameController.CreateHUD(status, target);
    }
    
    public void RetryBattle()
    {
        TCSceneManager.Instance.EnterScene(SceneBase.GameSceneType.Ingame, forcedEnter:true);
    }
}
