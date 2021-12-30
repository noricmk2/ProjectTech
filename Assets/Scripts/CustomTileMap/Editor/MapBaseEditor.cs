using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapBase))]
[CanEditMultipleObjects]
public class MapBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Sync"))
        {
           SyncMap();
        }

    }
    
    private void SyncMap()
    {
        
        MapBase targetData = (MapBase) target;
        CustomTileMapEditor.SaveMapAsFile(targetData.Data.buildXSize,targetData.Data.buildYSize,targetData.Data.mapName,true,targetData.gameObject, targetData.MapTiles);
    }
    
    private void OnValidate()
    {
        
    }
}
