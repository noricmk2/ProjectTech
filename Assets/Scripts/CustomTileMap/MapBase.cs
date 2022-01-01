using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<MapNodeType, bool> _typeDic = new Dictionary<MapNodeType, bool>();
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
        
        _mapTiles = _mapTiles.OrderByDescending(x=>x.y).ThenBy(x=>x.x).ToList();
    }

    public void SetFilterDic(Dictionary<MapNodeType, bool> typeDic)
    {
        _typeDic = typeDic;

        for (int i = 0; i < _mapTiles.Count; i++)
        {
            if(_typeDic.ContainsKey(_mapTiles[i].nodeType))
                _mapTiles[i].SetShowGizmo(_typeDic[_mapTiles[i].nodeType]);
        }
    }
}
