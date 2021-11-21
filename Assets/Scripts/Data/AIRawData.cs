using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BehaviorTreeData
{
    public BehaviorNodeType NodeType;
    public string NodeName;
    public bool IsRoot;
    public string ParentName;
    public int Level;
    public int Order;
    public string ActionName;
}


[CreateAssetMenu(fileName = "AIRawData", menuName = "DataAsset/AIRawData")]
public class AIRawData : ScriptableObject
{
    [SerializeField]
    public List<BehaviorTreeData> treeDataList;
}
