using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TCUtil;

public enum MoveState
{
    Stational,
    MoveTo,
    MoveDirection,
    MoveTween,
    MovePath,
}

public class MoveObject : ObjectBase
{
    #region Property
    protected readonly float nearWaypointValue = 1f;
    protected readonly float rotateSpeed = 3f;

    protected Vector3 _smoothDampVelocity = Vector3.zero;
    protected Vector3 _accelVelocity = Vector3.zero;
    protected virtual float _accelMagnitude => 3f;
    protected virtual float _accelSqr => 1.732f;
    protected MoveState _curState;
    protected Vector3[] _wayPoints;
    protected Vector3 _prevWaypoint;
    protected int _curWaypointIndex;
    protected float _moveSpeed;
    protected Action<MoveObject> _onPathEnd;
    protected int _segmentCount = 3;
    protected bool _movePathWithRotate;
    #endregion
    
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

    public virtual void MovePath(List<Vector3> path, float speed, Action<MoveObject> onPathEnd, bool withRotate = true)
    {
        if(path == null && path.Count == 0)
            return;

        _onPathEnd = onPathEnd;
        _movePathWithRotate = withRotate;
        _moveSpeed = speed;
        _curWaypointIndex = 0;
        var array = path.ToArray();
        float sqrMagnitude = 0;
        if (path.Count == 1)
        {
            sqrMagnitude = (CachedTransform.position - path[0]).sqrMagnitude;
            _wayPoints = new Vector3[path.Count];
            _wayPoints[0] = path[0];
        }
        else
        {
            for (int i = 0; i < path.Count - 1; ++i)
                sqrMagnitude += (path[i + 1] - path[i]).sqrMagnitude;
            int segment = (int)sqrMagnitude * _segmentCount;
            _wayPoints = new Vector3[segment];
            for (int i = 0; i < segment; ++i)
            {
                Vector3 waypoint = CatmullRom.EasyInterp3D(array, i / (float)segment);
                _wayPoints[i] = waypoint;
            }
        }
        _curState = MoveState.MovePath;
        _prevWaypoint = CachedTransform.position;
    }

    public virtual void OnUpdate()
    {
        switch (_curState)
        {
            case MoveState.MoveTo:
                break;
            case MoveState.MovePath:
                if (_wayPoints == null)
                {
                    DebugEx.Log("[Failed] waypoint is null");
                    _curState = MoveState.Stational;
                    return;
                }

                if (_curWaypointIndex >= _wayPoints.Length)
                {
                    DebugEx.Log($"[Notify] move path end : {this}");
                    _onPathEnd?.Invoke(this);
                    if (this is CharacterBase)
                    {
                        var character = this as CharacterBase;
                        character.OnMoveEnd();
                    }

                    _curState = MoveState.Stational;
                    return;
                }

                Vector3 targetPoint = _wayPoints[_curWaypointIndex];
                
                if (_movePathWithRotate)
                {
                    var targetRot = Quaternion.LookRotation(targetPoint - CachedTransform.position);
                    CachedTransform.rotation = Quaternion.Slerp(CachedTransform.rotation, targetRot, Time.deltaTime * rotateSpeed);
                }
                
                var curPos = CachedTransform.position;
                var lerpVal = Func.InverseLerp(_prevWaypoint, targetPoint, curPos);
                if (lerpVal > 0.99f)
                {
                    ++_curWaypointIndex;
                    _prevWaypoint = targetPoint;
                }
                else
                {
                    var dir = targetPoint - curPos;
                    MoveDirection(dir.normalized, _moveSpeed);
                }
                break;
        }
    }
}
