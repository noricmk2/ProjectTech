using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTileMap : MonoBehaviour
{
    
    public void DestroyAllTiles()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
                
    }
}
