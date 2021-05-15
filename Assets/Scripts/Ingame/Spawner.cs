using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spawner
{
    private Transform _createRoot;
    private MapData _curMapData;

    public void Init(Transform root, MapData data)
    {
        _createRoot = root;
        _curMapData = data;
    }

    public List<CharacterBase> CreateCharacter(List<CharacterBase.CharacterInitData> dataList)
    {
        var list = new List<CharacterBase>();

        for (int i = 0; i < dataList.Count; ++i)
        {
        }

        return list;
    }
}
