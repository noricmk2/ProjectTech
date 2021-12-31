using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUIController : UIController
{
    #region Property
    private Canvas _overlayCanvas;
    private IngameUIView _ingameView;

    private readonly string ingameViewName = "IngameUI";
    #endregion
    
    public override void Init()
    {
        base.Init();
        _overlayCanvas = UIManager.Instance.OverlayCanvas;
        var data = new IngameUIView.UIData();
        _ingameView.Init(data);
    }

    protected override void CreateView()
    {
        base.CreateView();
        _ingameView = UIManager.CreateView<IngameUIView>(ingameViewName);
        _ingameView.Deactivate();
    }

    public override void ShowUI()
    {
        base.ShowUI();
        _state = ControllerState.Activate;
    }

    protected override void ActivateView()
    {
        base.ActivateView();
        _ingameView.Activate();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        _ingameView.Deactivate();
    }

    public ObjectHUD CreateHUD(CharacterStatus status, Transform target)
    {
        var obj = ObjectFactory.Instance.GetPoolObject<ObjectHUD>("ObjectHUD");
        obj.transform.SetParent(_overlayCanvas.transform);
        obj.Init(this, status, target);
        return obj;
    }
}
