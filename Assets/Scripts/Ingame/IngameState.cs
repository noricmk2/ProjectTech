using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameStageMachine
{
    public enum IngameState
    {
        IngameStateInit,
        IngameStateUpdate,
        Length
    }
    private IngameStateBase _curState;
    private Dictionary<IngameState, IngameStateBase> _stateDict = new Dictionary<IngameState, IngameStateBase>();

    public void AddState(IngameState type, IngameStateBase instance)
    {
        _stateDict[type] = instance;
    }

    public void ChangeState(IngameState state)
    {
        if (!_stateDict.ContainsKey(state))
        {
            DebugEx.LogError($"[Failed] state not exist:{state}");
            return;
        }

        if (_curState != null)
            _curState.OnStateExit();

        var nextState = _stateDict[state];
        nextState.OnStateEnter();
        _curState = nextState;
    }
    public void OnUpdate()
    {
        if (_curState != null)
            _curState.OnUpdate();
    }
}

public class IngameStateBase
{
    private Action _onEnter;
    private Action _onExit;
    private Action _onUpdate;
    public IngameStageMachine.IngameState StateType { get; set; }
    public Action EnterAction { set { _onEnter = value; } }
    public Action ExitAction { set { _onExit = value; } }
    public Action UpdateAction { set { _onUpdate = value; } }
    public void OnStateEnter()
    {
        DebugEx.Log($"[Notify] on enter {StateType}");
        _onEnter?.Invoke();
    }
    public void OnUpdate()
    {
        _onUpdate?.Invoke();
    }
    public void OnStateExit()
    {
        DebugEx.Log($"[Notify] on exit {StateType}");
        _onExit?.Invoke();
    }
}