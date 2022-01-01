using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITitleView : UIView
{
    public class UIData : UIDataBase
    {
        public Action onClickAction;
    }

    private Action _onClickAction;

    public override void Init(UIDataBase data)
    {
        base.Init(data);
        var uiData = data as UIData;
        _onClickAction = uiData.onClickAction;
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public void OnClickStart()
    {
        _onClickAction?.Invoke();
    }
}
