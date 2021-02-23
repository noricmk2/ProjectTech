using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T> where T : IPoolObjectBase
{
    public delegate T CreateAction();

    private readonly CreateAction _createAction;
    private readonly Action<T> _pushAction;
    private readonly Action<T> _popAction;

    private Stack<T> m_Pool = new Stack<T>();
    private List<T> m_RestoreList = new List<T>();

    public int Count
    {
        get { return m_Pool.Count; }
    }

    public ObjectPool(int count, CreateAction createAction, Action<T> pushAction, Action<T> popAction)
    {
        _createAction = createAction;
        _pushAction = pushAction;
        _popAction = popAction;

        for (int i = 0; i < count; ++i)
            Add();
    }

    public void Push(T pushObj)
    {
        if (m_Pool.Contains(pushObj))
            return;
        m_Pool.Push(pushObj);

        if (m_RestoreList.Contains(pushObj))
            m_RestoreList.Remove(pushObj);

        if (_pushAction != null)
            _pushAction(pushObj);
    }

    public T Pop()
    {
        if (m_Pool.Count == 0)
            Add();

        var retObj = m_Pool.Pop();
        if (_popAction != null)
            _popAction(retObj);
        m_RestoreList.Add(retObj);
        return retObj;
    }

    public T Peek()
    {
        if (m_Pool.Count == 0)
            Add();

        return m_Pool.Peek();
    }

    private void Add()
    {
        if (_createAction != null)
        {
            var poolObj = _createAction();
            Push(poolObj);
        }
    }

    public void Restore()
    {
        for (int i = 0; i < m_RestoreList.Count; ++i)
            Push(m_RestoreList[i]);
    }

    public void Release()
    {
        m_Pool.Clear();
        m_RestoreList.Clear();
    }
}
