using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
[CustomEditor(typeof(CustomTileMap))]
[CanEditMultipleObjects]
public class CustomTileMapInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
       EditorGUILayout.TextArea("rerererre");
    }
}
