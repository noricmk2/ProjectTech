using System.Collections;
using System.Collections.Generic;
using TCUtil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHUD : UIItemBase, IPoolObjectBase
{
    #region Inspector
    [SerializeField]
    private Image _hpGauge;

    [SerializeField]
    private GameObject _root;

    [SerializeField]
    private TextMeshProUGUI _hpText;
    #endregion
    
    private float _maxHp;
    private UIIngameController _controller;
    private Transform _targetTrans;
    
    public void Init(UIIngameController controller, CharacterStatus status, Transform target)
    {
        _controller = controller;
        CachedRectTransform.SetSiblingIndex(1);
        _maxHp = status.MaxHP;
        _targetTrans = target;
        CachedRectTransform.anchoredPosition = Func.GetAnchoredPosition(UIManager.Instance.OverlayCanvas, IngameManager.Instance.IngameCamera, _targetTrans.position);
        CachedRectTransform.SetAsFirstSibling();
        SetHP(_maxHp);
    }
    
    public void SetHP(float hp)
    {
        _hpGauge.fillAmount = hp / _maxHp;
        if (_hpText)
        {
            string percent = (_hpGauge.fillAmount).ToString("#0.0%");
            _hpText.text = percent;
        }
    }

    private void LateUpdate()
    {
        if (_targetTrans)
            _rectTransform.anchoredPosition = Func.GetAnchoredPosition(UIManager.Instance.OverlayCanvas, IngameManager.Instance.IngameCamera, _targetTrans.position);
    }

    public void SetActive(bool active)
    {
        _root.SetActive(active);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }

    public void PushAction()
    {
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        if (_isDestroyed)
            return null;
        return gameObject;
    }
}
