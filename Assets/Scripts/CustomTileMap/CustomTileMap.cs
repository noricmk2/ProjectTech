using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor;
using UnityEngine;

public class CustomTileMap : MonoBehaviour
{
    [SerializeField]
    Material _copyObjMaterial;

    [SerializeField]
    Material _originMaterial;

    public GameObject test;

    public GameObject ingameTest;

    Dictionary<Vector2Int,List<GameObject>> _spawnablePostions = new Dictionary<Vector2Int, List<GameObject>>();

    List<ChildTile> _childTiles = new List<ChildTile>();
    
    public Material CopyObjMaterial { get => _copyObjMaterial; }
  //  public Dictionary<Vector2Int,GameObject> SpawnablePostions { get => _spawnablePostions; set => _spawnablePostions = value; }
    public Material OriginMaterial { get => _originMaterial; set => _originMaterial = value; }
    public List<ChildTile> ChildTiles { get => _childTiles; set => _childTiles = value; }

    private void OnDrawGizmos()
    {

        for (int i = 0; i < _childTiles.Count; i++)
        {
            if (_childTiles[i] != null)
            {
                ChildTile curTile = _childTiles[i];
                Gizmos.DrawWireMesh(curTile.mesh, 0, curTile.pos, curTile.rot, curTile.scale);

            }

        }
    }

    public MapEditorTile RegisterTileItem(Vector2Int pos, GameObject obj,int index =-1, MapNodeType mapNodeType = MapNodeType.Block)
    {
        List<GameObject> itemList = null;
        if(_spawnablePostions.TryGetValue(pos , out itemList) == false)
        { 
            itemList = new List<GameObject>();
            _spawnablePostions.Add(pos,itemList);
        }
        GameObject tileObjBase = new GameObject("TileObj");
        tileObjBase.transform.SetParent(obj.transform.parent);
        tileObjBase.transform.position = obj.transform.position;
        var box = obj.AddComponent<BoxCollider>();
        box.center = Vector3.zero;
        box.size = new Vector3(1, 1, 0.1f);
        var tileComp = tileObjBase.AddComponent<MapEditorTile>().InitTile(pos.x,pos.y,index,mapNodeType);
        var meshRen = tileObjBase.GetComponent<MeshRenderer>();
        if (meshRen != null)
        {
            tileComp.renderer = meshRen;
        }
        obj.transform.SetParent(tileObjBase.transform);
        obj.transform.localPosition = Vector3.zero;
        itemList.Add(obj);
        return tileComp;
    }

    public bool IsRegisteredTile(Vector2Int pos, GameObject obj)
    {
        return _spawnablePostions.ContainsKey(pos);
    }

    public int GetTileCount()
    {
        return _spawnablePostions.Count;
    }

    public void DestroyAllTiles()
    {
        _spawnablePostions.Clear();
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        _childTiles.Clear();
        EditorUtility.SetDirty(this);
    }
}
