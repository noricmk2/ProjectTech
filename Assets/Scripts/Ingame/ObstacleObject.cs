using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : ObjectBase, IQuadTreeObject, IPoolObjectBase
{
    public class ObstacleInitData
    {
        public string resourceName;
        public StatusData statusData;
        public bool passableObstacle;
        public Vector2 rectSize;
        public Vector2 tilePos;
    }

    #region Inspector
    [SerializeField] private Transform[] _coverPoints;
    #endregion

    private Vector2 _tilePos;
    private bool _passable;
    private Rect _rect;
    private StatusData _statusData;

    public bool Passable => _passable;

    public void Init(ObstacleInitData data)
    {
        _statusData = data.statusData;
        _rectSize = data.rectSize;
        _tilePos = data.tilePos;
        _passable = data.passableObstacle;
        _rect = new Rect(CachedTransform.position.x - _rectSize.x * 0.5f,
            CachedTransform.position.y - _rectSize.y * 0.5f, _rectSize.x, _rectSize.y);
        CachedTransform.rotation = Quaternion.Euler(-90f, 0, -90f);
    }
    
    #region QuadTreeImplement
    private Guid _id = Guid.NewGuid();
    private Vector2 _rectSize;
    [SerializeField] private Collider _collider;
    public Guid ID => _id;
    public Rect rect => _rect;
    public Collider OwnCollider => _collider;
    
    public void OnQuadQuery(List<IQuadTreeObject> colList)
    {
        
    }
    #endregion
    
    public void PushAction()
    {
        gameObject?.SetActive(false);
    }

    public void PopAction()
    {
        gameObject?.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        if(gameObject != null) 
            return gameObject;
        return null;
    }

    public Transform[] GetCoverPoints()
    {
        return _coverPoints;
    }
}
