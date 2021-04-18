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

    HashSet<Vector3Int> _spawnablePostions = new HashSet<Vector3Int>();
    List<ChildTile> _childTiles = new List<ChildTile>();
    public Material CopyObjMaterial { get => _copyObjMaterial; }
    public HashSet<Vector3Int> SpawnablePostions { get => _spawnablePostions; set => _spawnablePostions = value; }
    public Material OriginMaterial { get => _originMaterial; set => _originMaterial = value; }
    public List<ChildTile> ChildTiles { get => _childTiles; set => _childTiles = value; }

    private void OnDrawGizmos()
    {
        _childTiles.Clear();
        for(int i=0; i < _childTiles.Count; i++)
        {
            if(_childTiles[i] != null)
                Gizmos.DrawCube(_childTiles[i].transform.position,Vector3.one);
        }
    }


    public void DestroyAllTiles()
    {
        _spawnablePostions.Clear();
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
                
    }
}
