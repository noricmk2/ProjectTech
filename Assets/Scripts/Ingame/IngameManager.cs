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
    public IngameCameraMove CameraMove => _cameraMove;
    public Camera IngameCamera => _ingameCamera;
    public Transform CharacterRoot => _characterRoot;
    public Transform ProjectileRoot => _proejctileRoot;

    private bool _initComplete;
    #endregion

    public void Init()
    {
        _initComplete = false;

        InitCamera();
        InitIngameState();
        InitController();
    }

    private void InitController()
    {
        //Test
        var stageData = DataManager.Instance.GetStageDataByIndex(1);
        var mapData = stageData.mapData;
        MapManager.Instance.Init(mapData);
        _charController.Init();
        _waveController.Init(stageData.waveList);
        _projectileController.Init();
        
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateInit); 
    }

    private void InitIngameState()
    {
        var initState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateInit);
        initState.UpdateAction = InitCheck;
        var startState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateStart);
        startState.UpdateAction = IngameStart;
        var updateState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateUpdate);
        updateState.UpdateAction = ControllerUpdate;
        var endState = new IngameStateBase(IngameStageMachine.IngameState.IngameStateEnd);
        endState.UpdateAction = OnEnd;
        
        _stateMachine.AddState(initState);
        _stateMachine.AddState(startState);
        _stateMachine.AddState(updateState);
    }

    private void IngameStart()
    {
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
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateUpdate);
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
        //if(_initComplete)
        _stateMachine.ChangeState(IngameStageMachine.IngameState.IngameStateStart);
    }

    private void ControllerUpdate()
    {
        _charController.OnUpdate();
        _waveController.OnUpdate();
        _projectileController.OnUpdate();
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
        });
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

    #region AIMethod
    public static bool CheckFindEnemy(IBehaviorTreeOwner owner)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.FindAttackTarget();
        }
        
        return false;
    }
    
    public static bool CheckFindMove(IBehaviorTreeOwner owner)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.FindMoveTarget();
        }
        return false;
    }

    public bool OnAIMove(IBehaviorTreeOwner owner, Action onMoveEnd)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.MoveToSearchedPath(onMoveEnd);
        }

        return false;
    }

    public bool OnAIAttack(IBehaviorTreeOwner owner, Action onAttackEnd)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.Attack(onAttackEnd);
        }

        return false;
    }
    #endregion
}
