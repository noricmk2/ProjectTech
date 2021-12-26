using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;
using System;
using UnityEngine.AddressableAssets;

public interface IPoolObjectBase
{
    void PushAction();
    void PopAction();
    GameObject GetGameObject();
}

public class ObjectFactory : MonoSingleton<ObjectFactory>
{
    private readonly string cloneSuffix = "(Clone)";
    private readonly string keyName = "Async Load";
    private Dictionary<string, ObjectPool<IPoolObjectBase>> _poolListDict = new Dictionary<string, ObjectPool<IPoolObjectBase>>();
    public Transform IngamePoolParent { get; private set; }

    public T CreateObject<T>(string name, Transform parent, int layer = -1, bool stayWorldPos = false) where T : UnityEngine.Object
    {
        T result = null;
        var instance = AddressableManager.Instance.InstantiateSync(name, parent, stayWorldPos);
        if (instance == null)
        {
            DebugEx.LogError($"[Failed] Instantiate failed {name}");
            return null;
        }
        
        if (layer > -1)
            instance.transform.SetLayerRecursively(layer);
        result = instance.GetComponent<T>();

        if (result == null)
        {
            DebugEx.LogError($"[Failed] Target Component is empty {typeof(T)}");
            return null;
        }
        
        return result;
    }

    public T GetPoolObject<T>(string name) where T : class, IPoolObjectBase
    {
        var parent = transform;
        if (_poolListDict.ContainsKey(name))
        {
            var objectPool = _poolListDict[name];
            return objectPool.Pop() as T;
        }
        else
        {
            CreatePool<T>(1, name, parent);
            return _poolListDict[name].Pop() as T;
        }
    }

    public void DeactivePoolObject<T>(T obj) where T : IPoolObjectBase
    {
        var gameObj = obj.GetGameObject();
        if (gameObj == null)
        {
            DebugEx.LogError($"[Failed] object is null {typeof(T)}");
            return;
        }
        
        var poolName = gameObj.name;
        if (_poolListDict.ContainsKey(poolName))
            _poolListDict[poolName].Push(obj);
    }

    private void PreInstantiate(string poolName, int count, Action onLoadDone, int layer = -1, Transform parent = null)
    {
        int curLoadCount = 0;
        if (_poolListDict.ContainsKey(poolName))
        {
            for (int i = 0; i < count; ++i)
            {
                CreateObjectAsync(poolName, parent, obj =>
                {
                    var poolObj = obj.GetComponent<IPoolObjectBase>();
                    if (poolObj != null)
                    {
                        _poolListDict[poolName].Push(poolObj);
                        ++curLoadCount;
                        if(curLoadCount == count)
                            onLoadDone?.Invoke();
                    }
                }, layer);
            }
        }
        else
        {
            DebugEx.LogError($"[Failed] cannot preload, not exist pool:{poolName}");
        }
    }

    public void CreateObjectAsync(string name, Transform parent, Action<GameObject> onCreate, int layer = -1, bool stayWorldPos = false)
    {
        AddressableManager.Instance.InstantiateAsync(name, parent, handler =>
        {
            var obj = handler.Result as GameObject;
            if (Func.FastIndexOf(obj.name, cloneSuffix) != -1)
                obj.name = obj.name.Replace(cloneSuffix, "");
            obj.transform.Init(parent);
            obj.transform.SetLayerRecursively(layer);
            onCreate?.Invoke(obj);
        }, stayWorldPos);
    }

    public void CreatePool<T>(int count, string name, Transform parent = null) where T : IPoolObjectBase
    {
        var pool = new ObjectPool<IPoolObjectBase>(count, () =>
            {
                var instance = AddressableManager.Instance.InstantiateSync(name, parent);
                var poolObj = instance.GetComponent<IPoolObjectBase>();
                
                if (poolObj == null)
                {
                    DebugEx.LogError("[Failed]pool object is null: " + name);
                }
                else
                {
                    var gameObj = poolObj.GetGameObject();
                    gameObj.name = name;
                }

                return poolObj;
            }
            , (IPoolObjectBase pushObj) => { pushObj.PushAction(); },
            (IPoolObjectBase popObj) => { popObj.PopAction(); });

        _poolListDict[name] = pool;
    }

    public void ReleaseOnePool(string poolName)
    {
        if (_poolListDict.ContainsKey(poolName))
        {
            var targetPool = _poolListDict[poolName];
            targetPool.Release();
        }
    }

    public void Release()
    {
        var iter = _poolListDict.GetEnumerator();
        while (iter.MoveNext())
        {
            DebugEx.Log($"[Notify]{iter.Current.Key}/{iter.Current.Value}");
            iter.Current.Value.Release();
        }
        _poolListDict.Clear();
    }
}
