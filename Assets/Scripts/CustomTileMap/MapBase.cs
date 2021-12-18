using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBase : MonoBehaviour
{
    [Serializable]
    public class MapBaseData
    {
        public int buildXSize;
        public int buildYSize;
        public string mapName;
    }

    [SerializeField] private MapBaseData _data;
    [SerializeField] private List<MapEditorTile> _mapTiles;
    [SerializeField] private GameObject _tileBase;
    [SerializeField] private GameObject _mapBase;
    public GameObject TileBaseObj => _tileBase;
    public GameObject MapBaseObj => _mapBase;
    public MapBaseData Data => _data;
    public List<MapEditorTile> MapTiles => _mapTiles;
    public void SetDatas(GameObject tileBase, GameObject mapBase,List<MapEditorTile> tiles,MapBaseData data)
    {
        _tileBase = tileBase;
        _mapBase = mapBase;
        _mapTiles = tiles;
        _data = data;
    }
}