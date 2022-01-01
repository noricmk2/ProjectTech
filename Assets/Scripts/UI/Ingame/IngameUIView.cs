using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameUIView : UIView
{
    #region Inspector
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private CustomButton _pauseBtn;
    #endregion
    
    public class UIData : UIDataBase
    {
    }

    public override void Init(UIDataBase data)
    {
        base.Init(data);
        var uiData = data as UIData;
    }

    public override bool PreActivate()
    {
        return base.PreActivate();
    }

    public override bool PreDeactivate()
    {
        return base.PreDeactivate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void Activate()
    {
        base.Activate();
    }

    public void OnClickPause()
    {
        
    }
}
