using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public interface IQuadTreeObject
{
    Guid ID { get; }
    Rect rect { get; }
    Collider OwnCollider { get; }
    void OnQuadQuery(List<IQuadTreeObject> colList);
}

class QuadTree<T> where T : IQuadTreeObject
{
    public Rect _boundry;
    T[] _nodes;
    bool _root = false;
    bool _divided = false;
    int _numberOfNodesInserted = 0;
    int _maxSize;
    QuadTree<T> _northEast, _northWest, _southEast, _southWest;
    int _curLevel;
    int _maxLevel;
    List<T> _maxLevelNodeList;

    public int Count()
    {
        int result = _numberOfNodesInserted;
        if (_divided && _root)
        {
            result += _northEast.Count();
            result += _northWest.Count();
            result += _southEast.Count();
            result += _southWest.Count();
        }
        return result;
    }
    public QuadTree(Rect boundry, int size, int maxLevel, int curLevel)
    {
        if (boundry.width == 0 || boundry.height == 0)
            Debug.LogError("Radius of the boundry cannot be zero.");

        _maxLevel = maxLevel;
        _curLevel = curLevel;
        _nodes = new T[size];
        _maxSize = size;
        this._boundry = boundry;
    }

    #region Methods

    //Clear all the nodes in the Quad-Tree
    public void ClearAllNodes()
    {
        if (_numberOfNodesInserted == 0 && !_root) return;
        _numberOfNodesInserted = 0;
        _root = false;

        if(_maxLevelNodeList != null)
        {
            _maxLevelNodeList.Clear();
            _maxLevelNodeList = null;
        }

        if (_divided)
        {
            _northEast.ClearAllNodes();
            _northWest.ClearAllNodes();
            _southEast.ClearAllNodes();
            _southWest.ClearAllNodes();
        }
        _divided = false;
    }
    /// <summary>Insert a node in the Quad-Tree</summary>
    public bool Insert(T node)
    {
        if (node.rect.width == 0 || node.rect.height == 0)
			Debug.LogError("boundry cannot be zero.");

        //Checking if the position is in the boundries of the node.
        if (!InBoundry(node.rect))
            return false;

        bool success = false;

        if (_numberOfNodesInserted < _maxSize && !_root) 
		{
			_nodes[_numberOfNodesInserted] = node;
			_numberOfNodesInserted++;
			return true;
		}
		else if(_root)
		{
            success |= _northEast.Insert(node);
            success |= _northWest.Insert(node);
            success |= _southEast.Insert(node);
            success |= _southWest.Insert(node);
            return success;
        }
		else if(!_root && _numberOfNodesInserted == _maxSize)
		{
            if (_curLevel == _maxLevel)
            {
                if (_maxLevelNodeList == null)
                    _maxLevelNodeList = _nodes.ToList();
                _maxLevelNodeList.Add(node);
            }
            else
            {
                _root = true;
                _numberOfNodesInserted = 0;

                if (!_divided)
                    SubDivide();

                //Moving current nodes to subnodes
                for (int i = 0; i < _maxSize; i++)
                {
                    _northEast.Insert(_nodes[i]);
                    _northWest.Insert(_nodes[i]);
                    _southEast.Insert(_nodes[i]);
                    _southWest.Insert(_nodes[i]);
                }

                success |= _northEast.Insert(node);
                success |= _northWest.Insert(node);
                success |= _southEast.Insert(node);
                success |= _southWest.Insert(node);
                return success;
            }
		}
		return false;
	}

    private bool InBoundry(Rect target)
    {
        bool inBoundry = false;
        if (target.x <= _boundry.x + _boundry.width &&
            target.x + target.width >= _boundry.x &&
            target.y <= _boundry.y + _boundry.height &&
            target.y + target.height >= _boundry.y)
        {
            inBoundry = true;
        }

        return inBoundry;
    }


    private List<IQuadTreeObject> QueryObject(Rect targetRect)
    {
        Rect searchingArea = targetRect;
        var founded = new List<IQuadTreeObject>();

        if (_curLevel == _maxLevel)
        {
            if (_maxLevelNodeList != null)
            {
                for (int i = 0; i < _maxLevelNodeList.Count; ++i)
                {
                    if (searchingArea.Overlaps(_maxLevelNodeList[i].rect))
                        founded.Add(_maxLevelNodeList[i]);
                }
            }
            return founded;
        }

        if (_numberOfNodesInserted == 0 && !_root)
            return founded;
        if (!InBoundry(searchingArea))
            return founded;

        if (!_root && _numberOfNodesInserted != 0)
        {
            for (int i = 0; i < _numberOfNodesInserted; i++)
            {
                if (searchingArea.Overlaps(_nodes[i].rect))
                    founded.Add(_nodes[i]);
            }
            return founded;
        }
        else if (_root && _numberOfNodesInserted == 0)
        {
            founded.AddRange(_northEast.QueryObject(targetRect));
            founded.AddRange(_northWest.QueryObject(targetRect));
            founded.AddRange(_southEast.QueryObject(targetRect));
            founded.AddRange(_southWest.QueryObject(targetRect));
        }

        return founded;
    }

    public List<IQuadTreeObject> Query(Rect targetRect)
    {
        var foundList = QueryObject(targetRect);
        var distinctList = foundList.Distinct().ToList();
        return distinctList;
    }
    #endregion

    #region HelperMethods
    /// <summary>Divide the Quadtree into 4 equal parts and set it's boundries, NorthEast, NorthWest, SouthEast and SouthWest.</summary>
    private void SubDivide() 
	{
		//Size of the sub boundries 
		if(_northEast == null) 
		{	
			float x = _boundry.x;
			float y = _boundry.y;
			float width = _boundry.width * 0.5f;
			float height = _boundry.height * 0.5f;
	
			_northEast = new QuadTree<T>(new Rect(x + width, y + height, width, height), _maxSize, _maxLevel, _curLevel + 1);
			_northWest = new QuadTree<T>(new Rect(x, y + height, width, height), _maxSize, _maxLevel, _curLevel + 1);
			_southEast = new QuadTree<T>(new Rect(x + width, y, width, height), _maxSize, _maxLevel, _curLevel + 1);
			_southWest = new QuadTree<T>(new Rect(x, y, width, height), _maxSize, _maxLevel, _curLevel + 1);
		} 
		_divided = true; 
	}
	
	public void ShowBoundries(Color col)
	{ 
		float x = _boundry.x;
		float y = _boundry.y;
		float w = _boundry.width;
		float h = _boundry.height;

		Vector3 bottomLeftPoint = new Vector3(x, 0, y);
		Vector3 bottomRightPoint = new Vector3(x + w, 0, y);
		Vector3 topRightPoint = new Vector3(x + w, 0, y + h);
		Vector3 topLeftPoint = new Vector3(x, 0, y + h);
		
		Debug.DrawLine(bottomLeftPoint, bottomRightPoint, col);	//bottomLine
		Debug.DrawLine(bottomLeftPoint, topLeftPoint, col);		//leftLine
		Debug.DrawLine(bottomRightPoint, topRightPoint, col);		//rightLine
		Debug.DrawLine(topLeftPoint, topRightPoint, col);			//topLine

		if(_divided)
		{
			_northEast.ShowBoundries(col);
			_northWest.ShowBoundries(col);
			_southEast.ShowBoundries(col);
			_southWest.ShowBoundries(col);
		}
	}
	#endregion
}
