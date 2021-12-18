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
            MapBase targetData = (MapBase) target;
            CustomTileMapEditor.SaveMapAsFile(targetData.Data.buildXSize,targetData.Data.mapName,true,targetData.gameObject,targetData.MapTiles);
            
        }

    }
}
