using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapEditorTile : TileBase
{
    
    public int x;
    public int y;
    public int tileIndex;
    public MapNodeType nodeType;
    private bool _isShowGizmo = false;
    private bool _isSelect = false;

    public void SetShowGizmo(bool isShow)
    {
        _isShowGizmo = isShow;
    }

    public void SetShowSelectGizmo(bool isShow)
    {
        _isSelect = isShow;
    }
    public MapEditorTile InitTile(int x, int y, int index,MapNodeType nodeType)
    {
        this.x = x;
        this.y = y;
        this.tileIndex = index;
        this.nodeType = nodeType;
        return this;
    }

    private void OnDrawGizmos()
    {
        if (_isShowGizmo && _isSelect == false)
        {
            Handles.Label(transform.position, $"{nodeType.ToString()}\n{tileIndex}");
            Gizmos.DrawWireCube(transform.position,Vector3.one);
        }

        if (_isSelect)
        {
            Handles.Label(transform.position, $"{nodeType.ToString()}\n{tileIndex}");
        }
    }
}
