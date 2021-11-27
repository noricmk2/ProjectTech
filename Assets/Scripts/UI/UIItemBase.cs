using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemBase : MonoBehaviour
{
    protected RectTransform _rectTransform;
    protected bool _isDestroyed;
    
    public RectTransform CachedRectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = transform as RectTransform;
            return _rectTransform;
        }
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }
}
