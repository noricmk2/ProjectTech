using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{

}

public class ResourceManager : Singleton<ResourceManager>
{
    #region Property
    private Dictionary<string, UnityEngine.Object> _resourceCacheDict = new Dictionary<string, UnityEngine.Object>();
    private Queue<LoadOperation> _resourceLoadQueue = new Queue<LoadOperation>();
    private List<LoadOperation> _loadingList = new List<LoadOperation>();
    private int _loadLimit = 10;
    #endregion

    public static string GetPathByResourcesType(ResourceType type)
    {
        string path = string.Empty;
        return path;
    }

    public T LoadResourceFromResources<T>(string path, string objectName) where T : UnityEngine.Object
    {
        string fullPath = string.Concat(path, objectName);
        T retObj = null;
        if (_resourceCacheDict.ContainsKey(fullPath))
        {
            retObj = _resourceCacheDict[fullPath] as T;
        }
        else
        {
            retObj = Resources.Load<T>(fullPath);
            _resourceCacheDict[fullPath] = retObj;
        }
        return retObj;
    }

    public T LoadResource<T>(string path, string objectName) where T : UnityEngine.Object
    {
        string fullPath = string.Concat(path, objectName);
        T retObj = null;
#if LOAD_FROM_RESOURCES
        if (_resourceCacheDict.ContainsKey(fullPath))
        {
            retObj = _resourceCacheDict[fullPath] as T;
        }
        else
        {
            retObj = Resources.Load<T>(fullPath);
            _resourceCacheDict[fullPath] = retObj;
        }
#else
#endif
        return retObj;
    }

    public void LoadResourceAsync(string path, string objectName, Action<UnityEngine.Object> endAction)
    {
        string fullPath = string.Concat(path, objectName);
#if LOAD_FROM_RESOURCES
        if (_resourceCacheDict.ContainsKey(fullPath))
        {
            endAction(_resourceCacheDict[fullPath]);
        }
        else
        {
            var oper = new LoadOperation();
            oper.Init(fullPath, (obj) =>
            {
                _resourceCacheDict[fullPath] = obj;
                endAction(obj);
            });
            _resourceLoadQueue.Enqueue(oper);
        }
#else
#endif
    }

    private void Update()
    {
        for (int i = 0; i < _loadLimit; ++i)
        {
            if (_resourceLoadQueue.Count > 0)
            {
                var oper = _resourceLoadQueue.Dequeue();
                oper.Load();
                _loadingList.Add(oper);
            }
            else
                break;
        }

        for (int i = 0; i < _loadingList.Count; ++i)
        {
            if (_loadingList[i].IsDone())
                _loadingList[i].ExcuteEndAction();
        }
    }

    public void Release()
    {
        _resourceCacheDict.Clear();
    }
}
