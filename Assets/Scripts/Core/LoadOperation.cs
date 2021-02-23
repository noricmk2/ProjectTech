using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOperation : MonoBehaviour
{
    private AsyncOperation _request;
    private Action<UnityEngine.Object> _endAction;
    private string _assetPath;

    public void Init(string path, Action<UnityEngine.Object> endAction)
    {
        _assetPath = path;
        _endAction = endAction;
    }

    public void Load()
    {
#if LOAD_FROM_RESOURCES
        _request = Resources.LoadAsync(_assetPath);
#else
#endif
    }

    public bool IsDone()
    {
        if (_request != null)
            return _request.isDone;
        return false;
    }

    public void ExcuteEndAction()
    {
#if LOAD_FROM_RESOURCES
        var resourceRequest = _request as ResourceRequest;
        _endAction?.Invoke(resourceRequest.asset);
#else
#endif
    }
}
