using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTileMap : MonoBehaviour
{
    public class Tile : IEqualityComparer<Tile>
    {
        private int _id;
        private int _x;
        private int _y;

        public override bool Equals(object obj)
        {
            return false;
        }

        public bool Equals(Tile x, Tile y)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public int GetHashCode(Tile obj)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }



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

    public void RegisterTileItem(Vector2Int pos, GameObject obj)
    {
        List<GameObject> itemList = null;
        if(_spawnablePostions.TryGetValue(pos , out itemList) == false)
        {
            itemList = new List<GameObject>();
        }

        itemList.Add(obj);

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
    }
}
