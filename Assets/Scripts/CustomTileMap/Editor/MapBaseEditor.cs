using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapBase))]
[CanEditMultipleObjects]
public class MapBaseEditor : Editor
{
    private Dictionary<MapNodeType, bool> _typeDic = new Dictionary<MapNodeType, bool>();
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Sync"))
        {
           SyncMap();
        }

        foreach (MapNodeType type in Enum.GetValues(typeof(MapNodeType)))
        {
            if (_typeDic.ContainsKey(type) == false)
            {
                _typeDic.Add(type,false);
            }

            _typeDic[type] = EditorGUILayout.Toggle(type.ToString(), _typeDic[type]);

        }
        if(GUILayout.Button("SetFilter"))
        {
            MapBase targetData = (MapBase) target;
            targetData.SetFilterDic(_typeDic);
        }

    }
    
    private void SyncMap()
    {
        
        MapBase targetData = (MapBase) target;
        CustomTileMapEditor.SaveMapAsFile(targetData.Data.buildXSize,targetData.Data.buildYSize,targetData.Data.mapName,true,targetData.gameObject, targetData.MapTiles);
    }
    

}
