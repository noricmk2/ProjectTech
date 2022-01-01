using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIngameController : UIController
{
    #region Property
    private Canvas _overlayCanvas;
    private UIIngameView _uiIngameView;

    private readonly string ingameViewName = "IngameUI";
    #endregion
    
    public override void Init()
    {
        base.Init();
        _overlayCanvas = UIManager.Instance.OverlayCanvas;
        var data = new UIIngameView.UIData();
        _uiIngameView.Init(data);
    }

    protected override void CreateView()
    {
        base.CreateView();
        _uiIngameView = UIManager.CreateView<UIIngameView>(ingameViewName);
        _uiIngameView.Deactivate();
    }

    public override void ShowUI()
    {
        base.ShowUI();
        _state = ControllerState.Activate;
    }

    protected override void ActivateView()
    {
        base.ActivateView();
        _uiIngameView.Activate();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        _uiIngameView.Deactivate();
    }

    public ObjectHUD CreateHUD(CharacterStatus status, Transform target)
    {
        var obj = ObjectFactory.Instance.GetPoolObject<ObjectHUD>("ObjectHUD");
        obj.transform.SetParent(_overlayCanvas.transform);
        obj.Init(this, status, target);
        return obj;
    }
}