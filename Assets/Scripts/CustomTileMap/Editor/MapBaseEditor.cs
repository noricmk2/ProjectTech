using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

[CustomEditor(typeof(MapBase))]
[CanEditMultipleObjects]
public class MapBaseEditor : Editor
{
    private Dictionary<MapNodeType, bool> _typeDic = new Dictionary<MapNodeType, bool>();
    private GameObject _curObject;
    private bool _isObjHelperOn;
    private bool _isObjFilterOn;
    private int _selectX = -1;
    private int _selectY = -1;
    private List<MapEditorTile> _mapEditorTiles = null;
    private MapNodeType _mapNodeType;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapBase targetData = (MapBase) target;  
        if (GUILayout.Button("Sync"))
        {
            SyncMap();
        }

        _isObjHelperOn = EditorGUILayout.Foldout(_isObjHelperOn, "Object Helper");
        if (_isObjHelperOn)
        {

            _curObject = (GameObject) EditorGUILayout.ObjectField("Select Obj", _curObject, typeof(GameObject), true);

            _selectX = EditorGUILayout.IntField("Select X", _selectX);
            _selectY = EditorGUILayout.IntField("Select Y", _selectY);

            if (GUILayout.Button("Select Objs"))
            {
                targetData.MapTiles.ForEach(x => x.SetShowSelectGizmo(false));
                targetData.MapTiles.ForEach(x => x.SetShowGizmo(false));

                if (_selectX == -1 && _selectY != -1)
                {
                    _mapEditorTiles = targetData.MapTiles.FindAll(x => x.y == _selectY);
                }
                else if (_selectY == -1 && _selectX != -1)
                {
                    _mapEditorTiles = targetData.MapTiles.FindAll(x => x.x == _selectX);
                }
                else
                {
                    _mapEditorTiles = targetData.MapTiles.FindAll(x => x.y == _selectY && x.x == _selectX);
                }

                _mapEditorTiles.ForEach(x => x.SetShowSelectGizmo(true));
            }

            if (_mapEditorTiles != null)
            {
                EditorGUILayout.LabelField($"Select Obj Count : {_mapEditorTiles.Count}");

                if (_curObject != null)
                {
                    if (GUILayout.Button("Add Objs"))
                    {
                        System.Action<MapEditorTile> addAction = (MapEditorTile tile) =>
                        {
                            GameObject copyObj = GameObject.Instantiate(_curObject);
                            copyObj.transform.SetParent(tile.transform);
                            copyObj.transform.localPosition = Vector3.zero;
                        };

                        _mapEditorTiles.ForEach(addAction);
                    }
                }

                if (GUILayout.Button("Delete Objs"))
                {
                    System.Action<MapEditorTile> deleteAction = (MapEditorTile tile) =>
                    {
                        foreach (Transform child in tile.transform)
                        {
                            DestroyImmediate(child.gameObject);
                            break;
                        }
                    };

                    _mapEditorTiles.ForEach(deleteAction);
                }
                _mapNodeType =(MapNodeType) EditorGUILayout.EnumPopup("Enum To Change", _mapNodeType);
                if (GUILayout.Button("Change Node"))
                {
                    _mapEditorTiles.ForEach(x=>x.nodeType = _mapNodeType);
                }
            }
        }
    

        _isObjFilterOn = EditorGUILayout.Foldout(_isObjFilterOn, "Object Filter");
        if (_isObjFilterOn)
        {
            foreach (MapNodeType type in Enum.GetValues(typeof(MapNodeType)))
            {
                if (_typeDic.ContainsKey(type) == false)
                {
                    _typeDic.Add(type,false);
                }

                _typeDic[type] = EditorGUILayout.Toggle(type.ToString(), _typeDic[type]);

            }

        }
      
        targetData.SetFilterDic(_typeDic);

    }
    
    private void SyncMap()
    {
        
        MapBase targetData = (MapBase) target;
        CustomTileMapEditor.SaveMapAsFile(targetData.Data.buildXSize,targetData.Data.buildYSize,targetData.Data.mapName,true,targetData.gameObject, targetData.MapTiles);
    }
    

}
