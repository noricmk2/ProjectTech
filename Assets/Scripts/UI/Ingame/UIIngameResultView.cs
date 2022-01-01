using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIngameResultView : UIView
{
    #region Inspector
    [SerializeField] private GameObject _victoryObj;
    [SerializeField] private GameObject _defeatObj;
    #endregion

    public void SetResult(bool isVictory)
    {
        _victoryObj.SetActive(isVictory);
        _defeatObj.SetActive(!isVictory);
    }

    public void OnClickRetry()
    {
        UIManager.Instance.CloseAll();
        IngameManager.Instance.RetryBattle();
    }
}
