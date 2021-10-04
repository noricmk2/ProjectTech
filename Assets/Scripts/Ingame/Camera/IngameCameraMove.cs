using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCameraMove : MonoBehaviour
{
    #region Define
    public class ShakeData
    {
        
    }

    public class FollowMoveData
    {
        public List<Transform> followTargets;
    }
    #endregion
    
    [SerializeField] private Camera _mainCamera;

    public void SetFollowData(FollowMoveData data)
    {
        
    }

    public void Shake(ShakeData data)
    {
        
    }

    private void LateUpdate()
    {
        
    }
}
