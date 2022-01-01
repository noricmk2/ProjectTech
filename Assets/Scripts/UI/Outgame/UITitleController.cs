using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITitleController : UIController
{
    #region Property
    private UITitleView _titleView;
    
    private readonly string titleViewName = "UITitle";
    #endregion
    
    public override void Init()
    {
        base.Init();

        var uiData = new UITitleView.UIData();
        uiData.onClickAction = () => TCSceneManager.Instance.EnterScene(SceneBase.GameSceneType.Ingame);
        _titleView.Init(uiData);
    }

    protected override void CreateView()
    {
        base.CreateView();
        _titleView = UIManager.CreateView<UITitleView>(titleViewName);
        _titleView.Deactivate();
    }

    public override void ShowUI()
    {
        base.ShowUI();
        _state = ControllerState.Activate;
    }

    protected override void ActivateView()
    {
        base.ActivateView();
        _titleView.Activate();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        _titleView.Deactivate();
    }
}
