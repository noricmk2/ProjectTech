using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum MoveState
{
    Stational,
    MoveTo,
    MoveDirection,
    MoveTween,
}

public class MoveObject : ObjectBase
{
    protected Vector3 _smoothDampVelocity = Vector3.zero;
    protected Vector3 _accelVelocity = Vector3.zero;
    protected virtual float _accelMagnitude => 3f;
    protected virtual float _accelSqr => 1.732f;
    protected MoveState _curState;
    protected List<Vector3> _pathList;
    protected float _moveToSpeed;

    public virtual void MoveInit()
    {
        _curState = MoveState.Stational;
    }

    public virtual void MoveDirection(Vector3 direction, float speed, bool moveLocal = true)
    {
        var velocity = direction * Time.deltaTime * speed;
        if (moveLocal)
            CachedTransform.localPosition += velocity;
        else
            CachedTransform.position += velocity;
    }

    public virtual void MoveDirectionAccel(Vector3 direction, float speed)
    {
        CachedTransform.position += _accelVelocity * Time.deltaTime * speed;
        _accelVelocity += direction * Time.deltaTime * speed;
        if (_accelVelocity.sqrMagnitude > _accelMagnitude)
        {
            _accelVelocity = _accelVelocity.normalized * _accelSqr;
        }
    }

    public virtual void Translate(Vector3 translation, float speed, Space space = Space.Self)
    {
        var velocity = translation * Time.deltaTime * speed;
        CachedTransform.Translate(velocity, space);
    }

    public virtual void SmoothDamp(Vector3 targetPos, float duration)
    {
        CachedTransform.position = Vector3.SmoothDamp(CachedTransform.position, targetPos, ref _smoothDampVelocity, duration);
    }

    public virtual void MovePath(List<Vector3> path, float duration, Ease ease = Ease.Linear)
    {
        CachedTransform.DOKill();
        var array = path.ToArray();
        CachedTransform.DOPath(array, duration, PathType.CatmullRom, gizmoColor: Color.blue).SetEase(ease);
    }

    public virtual void MoveTo(List<Vector3> path, float speed)
    {
        _curState = MoveState.MoveTo;
        _pathList = path;
        _moveToSpeed = speed;
    }

    public virtual void OnUpdate()
    {
        switch (_curState)
        {
            case MoveState.MoveTo:
                break;
        }
    }

    public static Vector3 CatmullRomPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return p1 + (0.5f * (p2 - p0) * t) + 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                0.5f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t;
    }
}
