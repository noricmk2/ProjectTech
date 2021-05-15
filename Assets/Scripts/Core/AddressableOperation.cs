using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Threading.Tasks;

public class AddressableOperation
{
    #region Property
    protected AsyncOperationHandle _handler;
    protected Action _onFailed;
    protected Action<AsyncOperationHandle> _onLoadDone;
    protected string _address;
    public AsyncOperationHandle Handler => _handler;
    protected bool _startOper = false;
    protected bool _isOver;
    public bool IsOver => _isOver;
    #endregion

    public void Init(string address, Action<AsyncOperationHandle> onLoadDone, Action onFailed = null)
    {
        _isOver = false;
        _address = address;
        _onLoadDone = onLoadDone;
        _onFailed = onFailed;
        _startOper = false;
    }

    public virtual void StartOperation()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public void Release()
    {
        _startOper = false;
        _isOver = true;
        Addressables.Release(_handler);
    }
}

public class AddressableDownloadOperation : AddressableOperation
{
    public override void StartOperation()
    {
        base.StartOperation();
        _handler = Addressables.DownloadDependenciesAsync(_address);
        _handler.Completed += _onLoadDone;
        _handler.Completed += handler=> Release();
        _startOper = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_startOper)
        {
            if (_handler.Status == AsyncOperationStatus.Failed)
            {
                DebugEx.Log($"[Failed] download failed:{_handler.OperationException.Message}");
                _startOper = false;
                _onFailed?.Invoke();
                Release();
            }
            else
            {
                var dowloadStatus = _handler.GetDownloadStatus();
                DebugEx.Log($"{dowloadStatus.Percent}");
            }
        }
    }
}

public class AddressableLoadOperation : AddressableOperation
{
    public override void StartOperation()
    {
        base.StartOperation();
        AddressableManager.LoadResourceLocations(_address, validateAsync =>
        {
            if (validateAsync.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Failed] '{_address}' invalid!");
                return;
            }

            if (validateAsync.Result.Count <= 0)
            {
                Debug.LogError($"[Failed] address '{_address}' results.Count == 0!");
                return;
            }

            _handler = Addressables.LoadAssetAsync<object>(_address);
            _handler.Completed += _onLoadDone;
            _handler.Completed += handler=> Release();
            _startOper = true;
        });
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_startOper)
        {
            if (_handler.Status == AsyncOperationStatus.Failed)
            {
                DebugEx.Log($"[Failed]{_handler.DebugName} download failed:{_handler.OperationException.Message}");
                _startOper = false;
                _onFailed?.Invoke();
                Release();
            }
        }
    }
}

public class AddressableInstantiateOperation : AddressableOperation
{
    private Vector3 _position;
    private Quaternion _rotation;
    private Transform _parent;

    public AddressableInstantiateOperation(Vector3 position, Quaternion rotation, Transform parent)
    {
        _position = position;
        _rotation = rotation;
        _parent = parent;
    }

    public override void StartOperation()
    {
        base.StartOperation();
        _handler = Addressables.InstantiateAsync(_address, _position, _rotation, _parent);
        _handler.Completed += _onLoadDone;
    }
}