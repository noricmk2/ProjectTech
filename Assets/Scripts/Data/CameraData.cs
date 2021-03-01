using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CameraData", menuName = "DataAsset/IngameCameraData")]
public class CameraData : ScriptableObject
{
    public Vector3 Position;
    public Vector3 Rotate;
}
