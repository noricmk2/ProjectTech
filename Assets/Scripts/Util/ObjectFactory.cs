using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TCUtil;

public interface IPoolObjectBase
{
    void PushAction();
    void PopAction();
    GameObject GetGameObject();
}

public class ObjectFactory : MonoSingleton<ObjectFactory>
{
    private Dictionary<string, ObjectPool<IPoolObjectBase>> _poolListDict = new Dictionary<string, ObjectPool<IPoolObjectBase>>();

    #region StaticMethod
    public static GameObject Instantiate(Transform parent, GameObject prefab, bool stayWorldPos = false)
    {
        GameObject createdObj = Instantiate(prefab);
        if (parent != null)
        {
            createdObj.transform.Init(parent, stayWorldPos);
        }
        return createdObj;
    }

    public static T Instantiate<T>(Transform parent, GameObject prefab, int layer = -1, bool stayWorldPos = false)
    {
        var createdObj = Instantiate(parent, prefab, stayWorldPos);
        if (layer > -1)
            createdObj.transform.SetLayerRecursively(layer);
        return createdObj.GetComponent<T>();
    }
    #endregion
    public T CreateObject<T>(string name, Transform parent, int layer = -1, bool stayWorldPos = false) where T : UnityEngine.Object
    {
        T result = null;
        var gameObject = AddressableManager.Instance.InstantiateSync(name, parent, stayWorldPos);
        result = gameObject.GetComponent<T>();
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

    public void DeactivePoolObject<T>(string name, T obj) where T : IPoolObjectBase
    {
        if (_poolListDict.ContainsKey(name))
            _poolListDict[name].Push(obj);
    }

    public void CreatePool<T>(int count, string name, Transform parent = null) where T : IPoolObjectBase
    {
        var pool = new ObjectPool<IPoolObjectBase>(count, () =>
        {
            T poolObj = AddressableManager.Instance.InstantiateSync(name, parent).GetComponent<T>();
            if (poolObj == null)
            {
                DebugEx.LogError("[Failed]pool object is null: " + name);
            }
            return poolObj;
        }
        , (IPoolObjectBase pushObj) => { pushObj.PushAction(); }, (IPoolObjectBase popObj) => { popObj.PopAction(); });

        _poolListDict[name] = pool;
    }

    public void Release()
    {
        var iter = _poolListDict.GetEnumerator();
        while (iter.MoveNext())
            iter.Current.Value.Release();
        _poolListDict.Clear();
    }
}
