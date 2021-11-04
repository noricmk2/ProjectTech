using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeController
{
    private readonly int _treeNodeCount = 3;
    private QuadTree<IQuadTreeObject> _quadTree;
    private List<IQuadTreeObject> _nodeObjList = new List<IQuadTreeObject>();

    public void Init(Rect rect)
    {
        _quadTree = new QuadTree<IQuadTreeObject>(rect, _treeNodeCount, 5, 0);
        _nodeObjList.Clear();
    }

    private void InsertNodeList()
    {
        _quadTree.ClearAllNodes();
        for (int i = 0; i < _nodeObjList.Count; ++i)
            _quadTree.Insert(_nodeObjList[i]);
    }

    public void RegisterQuadTreeObject(IQuadTreeObject obj)
    {
        if (!_nodeObjList.Contains(obj))
        {
            _nodeObjList.Add(obj);
            InsertNodeList();
        }
    }

    public void RemoveQuadTreeObject(IQuadTreeObject obj)
    {
        if (_nodeObjList.Contains(obj))
        {
            _nodeObjList.Remove(obj);
            InsertNodeList();
        }
    }

    public List<IQuadTreeObject> QueryRect(Rect targetRect)
    {
        return _quadTree.Query(targetRect);
    }

    public void OnUpdate()
    {
#if SHOW_BOUNDRY
        _quadTree.ShowBoundries(Color.blue);
#endif
    }

    public void Release()
    {
        _nodeObjList.Clear();
        _quadTree.ClearAllNodes();
    }
}
