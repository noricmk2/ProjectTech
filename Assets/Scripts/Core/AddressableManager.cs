using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableManager : MonoSingleton<AddressableManager>
{
    #region Property
    private readonly string atlasDataName = "SpriteAtlasInfo";
    private SpriteAtlasData _atlasData;

    private Queue<AddressableOperation> _operationQueue = new Queue<AddressableOperation>();
    private AddressableOperation _curOperation;
    private bool _isInit;
    private int _initLoadCount;
    #endregion
    
    public void Init(Action onComplete)
    {
        if(_isInit)
            return;

        _initLoadCount = 0;
        DebugEx.Log($"[GameInfo] Addressable Init Start");
        ResourceManager.ExceptionHandler = CustomExceptionHandler;
        
        var textAsset =LoadAssetSync<TextAsset>(atlasDataName);
        _atlasData = JsonConvert.DeserializeObject<SpriteAtlasData>(textAsset.text);

        _isInit = true;
        StartCoroutine(InitCheck_C(onComplete));
    }

    private IEnumerator InitCheck_C(Action action)
    {
        int totalCount = 0;
        while (_initLoadCount < totalCount)
        {
            yield return null;
        }
        action?.Invoke();
    }
    
    private void CustomExceptionHandler(AsyncOperationHandle handle, Exception exception)
    {
        
    }

    void Update()
    {
        if (_curOperation == null && _operationQueue.Count > 0)
        {
            _curOperation = _operationQueue.Dequeue();
            _curOperation.StartOperation();
            DebugEx.Log($"operation start:{_curOperation.Handler.DebugName}");
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

    public void CheckDownloadAsync(string label, Action<long> endAction)
    {
        var handler = Addressables.GetDownloadSizeAsync(label);
        handler.Completed += hdl =>
        {
            endAction?.Invoke(hdl.Result);
        };
    }

    private IEnumerator CheckDownloadSize_C(string address, Action<bool> onCheck)
    {
        var handler = Addressables.GetDownloadSizeAsync(address);
        yield return handler;
        onCheck?.Invoke(handler.Result > 0);
    }

    public void DownloadDependencyAsync(string label, Action<AsyncOperationHandle> onLoadDone, Action onFailed = null, Action<AsyncOperationHandle> onLoad = null)
    {
        var downloadOper = new AddressableDownloadOperation();
        downloadOper.Init(label, onLoadDone, onFailed, onLoad);
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
        var spriteName = _atlasData.GetAddressBySpriteName(address);
        if (string.IsNullOrEmpty(spriteName))
            return null;
        else
        {
            var oper = Addressables.LoadAssetAsync<Sprite>(spriteName);
            oper.WaitForCompletion();

            return oper.Result;
        }
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
            onFail?.Invoke();

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

    public void InstantiateAsync(string address,Transform parent, Action<AsyncOperationHandle> onLoadDone, bool inWorldSpace = false, Action onFailed = null)
    {
        var handler = Addressables.InstantiateAsync(address, parent, inWorldSpace);
        handler.Completed += hdl =>
        {
            onLoadDone?.Invoke(hdl);
        };

        // var instatiateOper = new AddressableInstantiateOperation(parent);
        // instatiateOper.Init(address, onLoadDone, onFailed);
        // _operationQueue.Enqueue(instatiateOper);
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
        if(!Addressables.ReleaseInstance(obj))
            Destroy(obj);
    }
}
