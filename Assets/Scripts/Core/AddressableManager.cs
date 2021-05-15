using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableManager : MonoSingleton<AddressableManager>
{
    #region Property
    private readonly string atlasDataName = "SpriteAtlasInfo";
    private SpriteAtlasData _atlasData;

    private Queue<AddressableOperation> _operationQueue = new Queue<AddressableOperation>();
    private AddressableOperation _curOperation;
    #endregion

    void Update()
    {
        if (_curOperation == null && _operationQueue.Count > 0)
        {
            _curOperation = _operationQueue.Dequeue();
            _curOperation.StartOperation();
        }

        if (_curOperation != null)
        {
            _curOperation.OnUpdate();
            if (_curOperation.IsOver)
                _curOperation = null;
        }
    }

    public long CheckDownload(string label)
    {
        var handler = Addressables.GetDownloadSizeAsync(label);
        handler.WaitForCompletion();
        return handler.Result;
    }
    
    private IEnumerator CheckDownloadSize_C(string address, Action<bool> onCheck)
    {
        var handler = Addressables.GetDownloadSizeAsync(address);
        yield return handler;
        onCheck?.Invoke(handler.Result > 0);
    }

    public void DownloadDependencyAsync(string label, Action<AsyncOperationHandle> onLoadDone, Action onFailed = null)
    {
        var downloadOper = new AddressableDownloadOperation();
        downloadOper.Init(label, onLoadDone, onFailed);
        _operationQueue.Enqueue(downloadOper);
    }

    public void LoadSpriteAsync(string address, Action<AsyncOperationHandle> onLoadDone, Action onFailed = null)
    {
        var loadOper = new AddressableLoadOperation();
        loadOper.Init(_atlasData.GetAddressBySpriteName(address), onLoadDone, onFailed);
        _operationQueue.Enqueue(loadOper);
    }

    public Sprite LoadSpriteSync(string address)
    {
        var oper = Addressables.LoadAssetAsync<Sprite>(_atlasData.GetAddressBySpriteName(address));
        oper.WaitForCompletion();
        return oper.Result;
    }

    public void LoadAssetAsync(string address, Action<AsyncOperationHandle> onLoadDone, Action onFailed = null)
    {
        var loadOper = new AddressableLoadOperation();
        loadOper.Init(address, onLoadDone, onFailed);
        _operationQueue.Enqueue(loadOper);
    }

    public T LoadAssetSync<T>(string address, Action onFail = null) where T : UnityEngine.Object
    {
        T result = null;
        var oper = Addressables.LoadAssetAsync<T>(address);
        oper.WaitForCompletion();
        if (oper.Status == AsyncOperationStatus.Failed)
        {
            DebugEx.LogError($"{address} load is fail {oper.OperationException.Message}");
            onFail?.Invoke();
        }

        result = oper.Result;
        return result;
    }

    public static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocations(string address, Action<AsyncOperationHandle<IList<IResourceLocation>>> onLoadDone)
    {
        var validateKeyAsync = Addressables.LoadResourceLocationsAsync(address);
        if (onLoadDone != null)
            validateKeyAsync.Completed += onLoadDone;
        return validateKeyAsync;
    }

    public GameObject InstantiateSync(string address, Transform parent, bool inWorldSpace = false)
    {
        var handler = Addressables.InstantiateAsync(address, parent, inWorldSpace);
        handler.WaitForCompletion();
        return handler.Result;
    }

    public void ReleaseAsset(string group)
    {

    }

    public void Release<T>(T target)
    {
        Addressables.Release(target);
    }

    public void ReleaseInstance(GameObject obj)
    {
        Addressables.ReleaseInstance(obj);
    }
}
