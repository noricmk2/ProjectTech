using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Define
public class MapData
{
    public int width;
    public int height;
    public List<JPSNode> nodeList;
}
public class StageData
{
    public List<CharacterData> enemyList;
    public MapData mapData;
}
public class CharacterData
{
    public string resourceName;
    public CharacterType characterType;
    public StatusData statusData;
    public Vector2 spawnPos;
}
public class StatusData
{
    public float hp;
    public float atk;
    public float def;
}
#endregion

public class DataManager : Singleton<DataManager>
{
    public StageData GetStageDataByIndex(int index)
    {
        //Test
        var result = new StageData();
        result.enemyList = new List<CharacterData>();
        for (int i = 0; i < 3; ++i)
        {
            var enemyData = new CharacterData();
            enemyData.characterType = CharacterType.Enemy;
            enemyData.resourceName = "TestEnemy";
            enemyData.statusData = GetStausDataByIndex(0);
            result.enemyList.Add(enemyData);
        }
        var mapData = new MapData();
        mapData.width = 10;
        mapData.height = 50;
        mapData.nodeList = new List<JPSNode>();
        for (int i = 0; i < testMap.Length; ++i)
        {
            var node = new JPSNode(i % mapData.width, i / mapData.width, testMap[i]);
            mapData.nodeList.Add(node);
        }
        result.mapData = mapData;
        return result;
    }

    public StatusData GetStausDataByIndex(int index)
    {
        //Test
        var result = new StatusData();
        result.atk = 100;
        result.def = 50;
        result.hp = 1000;
        return result;
    }

    private int[] testMap =
        {
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,0,0,0,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,0,0,1,1,
        0,1,1,1,1,1,0,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,0,1,1,1,1,1,
        0,1,1,0,0,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,0,1,1,1,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,0,0,0,0,
        0,1,1,1,0,0,0,0,0,0,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,0,0,1,1,1,1,
        0,1,1,1,1,0,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,0,0,0,1,1,1,1,1,1,
        0,1,1,1,1,1,1,0,1,1,
        0,1,1,1,1,1,1,0,1,1,
        0,1,1,1,1,1,1,1,1,1,
        0,1,1,1,1,1,1,1,1,1
    };
}