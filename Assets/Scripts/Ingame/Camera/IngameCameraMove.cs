using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameCameraMove : MonoBehaviour
{
    #region Define
    public class ShakeData
    {
        public float time;
    }

    public class FollowMoveData
    {
        public List<Transform> followTargets;
        public float lerpValue;
        public float maxZoomValue;
        public float minZoomValue;
    }
    #endregion

    #region Property
    private CameraState _curCameraState;
    private Vector3 _followOffset;
    private FollowMoveData _followData;
    private ShakeData _shakeData;
    #endregion
    
    [SerializeField] private Camera _mainCamera;

    public void Init()
    {
        _curCameraState = CameraState.Stational;
    }
    
    //TODO:타겟 삭제시 처리
    public void SetFollowData(FollowMoveData data)
    {
        _followData = data;
    }

    public void StartFollow()
    {
        if (_followData != null)
        {
            var centerPos = GetCenterPos(_followData.followTargets.Select(x => x.position).ToList());
            _followOffset = _mainCamera.transform.position - centerPos;
            _curCameraState = CameraState.FollowTarget;
        }
    }

    //쉐이크 구현
    public void SetShakeData(ShakeData data)
    {
        _shakeData = data;
    }

    public void StartShake()
    {
        if (_shakeData != null)
            _curCameraState = CameraState.Shake;
    }

    private void LateUpdate()
    {
        switch (_curCameraState)
        {
            case CameraState.Stational:
                break;
            case CameraState.FollowTarget:
            {
                var centerPos = GetCenterPos(_followData.followTargets.Select(x=>x.position).ToList());
                var targetPos = centerPos + _followOffset;
                var prevPos = _mainCamera.transform.position;
                targetPos.x = prevPos.x;
                _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, targetPos, _followData.lerpValue);
            }
                break;
            case CameraState.Shake:
                break;
            case CameraState.Animation:
                break;
        }
    }

    private Vector3 GetCenterPos(List<Vector3> positions)
    {
        if (positions == null || positions.Count == 0)
            return Vector3.zero;
        
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < positions.Count; ++i)
            sum += positions[i];

        return sum / positions.Count;
    }
}
