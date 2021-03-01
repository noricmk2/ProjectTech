using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    private Transform _cachedTransform;
    public Transform CachedTransform
    {
        get
        {
            if (_cachedTransform == null)
                _cachedTransform = transform;
            return _cachedTransform;
        }
    }
}
