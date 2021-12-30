using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditorTile : TileBase
{
    
    public int x;
    public int y;
    public int tileIndex;
    public MapNodeType nodeType;

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
        Handles.Label(transform.position, $"{nodeType.ToString()}\n{tileIndex}");

    }
}
