using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController
{
    public enum ControllerState
    {
        None,
        Init,
        PreActivate,
        Activate,
        Close,
    }

    protected UIManager.UIType _uiType;
    protected ControllerState _state;
    
    public virtual void Init()
    {
        CreateView();
        _state = ControllerState.Init;
    }

    protected virtual void CreateView()
    {
    }
    
    public virtual void ShowUI()
    {
        _state = ControllerState.PreActivate;
    }

    protected virtual void ActivateView()
    {
        _state = ControllerState.Activate;
    }

    public virtual void OnUpdate()
    {
        switch (_state)
        {
            case ControllerState.None:
                break;
            case ControllerState.Init:
                break;
            case ControllerState.PreActivate:
                break;
            case ControllerState.Activate:
                ActivateView();
                break;
            case ControllerState.Close:
                break;
        }
    }

    public virtual void CloseUI()
    {
        _state = ControllerState.Close;
    }
}
