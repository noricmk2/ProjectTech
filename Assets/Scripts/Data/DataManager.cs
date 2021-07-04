using System;
using System.Collections;
using System.Collections.Generic;
using TCUtil;
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
    public List<WaveData> waveList;
    public MapData mapData;
}

public class CharacterData
{
    public string resourceName;
    public CharacterType characterType;
    public StatusData statusData;
}

public class StatusData
{
    public float hp;
    public float atk;
    public float def;
    public float evade;
    public float accuracyRate;
}

public class WaveData
{
    public int waveOrder;
    public List<SpawnData> spawnDataList;
}

public class SpawnData
{
    public int spawnId;
    public string spawnerName;
    public CharacterData spawnCharacter;
    public float spawnDelay;
    public int count;
}
#endregion

public class DataManager : Singleton<DataManager>
{
    #region Table
    private Dictionary<TableType, List<TableData>> _tableDataDict;
    private Dictionary<Type, TableType> _tableTypeDict = new Dictionary<Type, TableType>();

    public void SetTableData(Dictionary<TableType, List<TableData>> dict)
    {
        _tableDataDict = dict;
        var iter = _tableDataDict.GetEnumerator();
        while (iter.MoveNext())
        {
            var data = iter.Current.Value.Peek();
            _tableTypeDict[data.GetType()] = iter.Current.Key;
        }
    }

    public T GetRecord<T>(int index) where T : TableData
    {
        var type = GetTableType(typeof(T));
        if (!_tableDataDict.ContainsKey(type))
        {
            DebugEx.LogError($"[Failed] not exist table type: {type}");
            return null;
        }
        
        var list = _tableDataDict[type];
        var data = list.Find(x => x.Index == index) as T;
        if (data == null)
        {
            DebugEx.LogError($"[Failed] not exist {type} table data. index : {index}");
            return null;
        }
        else
            return data;
    }

    public List<T> GetRecords<T>(System.Func<T, bool> selection) where T : TableData
    {
        var type = GetTableType(typeof(T));
        if (!_tableDataDict.ContainsKey(type))
        {
            DebugEx.LogError($"[Failed] not exist table type: {type}");
            return null;
        }
        
        var list = _tableDataDict[type];
        var result = new List<T>();
        for (int i = 0; i < list.Count; ++i)
        {
            var data = list[i] as T;
            if(selection(data))
                result.Add(data);
        }
        return result;
    }

    private TableType GetTableType(System.Type type)
    {
        if (_tableTypeDict.ContainsKey(type))
            return _tableTypeDict[type];
        else
        {
            DebugEx.LogError($"[Failed] not exist table type:{type}");
            return TableType.Length;
        }
    }
    #endregion

    public StageData GetStageDataByIndex(int index)
    {
        //Test
        var table = GetRecord<StageTable>(index);
        
        var result = new StageData();
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
        result.waveList = CreateWaveListByStageIdx(index);
        return result;
    }

    public List<WaveData> CreateWaveListByStageIdx(int index)
    {
        var result = new List<WaveData>();
        var stageTable = GetRecord<StageTable>(index);
        for (int i = 0; i < stageTable.WaveIndexes.Length; ++i)
        {
            var waveTable = GetRecord<WaveTable>(stageTable.WaveIndexes[i]);
            var waveData = new WaveData();
            waveData.waveOrder = i;
            waveData.spawnDataList = new List<SpawnData>();
            for(int j=0; j<waveTable.CharacterIndexes.Length; ++j)
                waveData.spawnDataList.Add(CreateSpawnData(waveTable.CharacterIndexes[j], waveTable.SpawnPoints[j]));
            result.Add(waveData);
        }

        return result;
    }

    public SpawnData CreateSpawnData(int characterIdx, Vector2 spawnPoint)
    {
        var spawnData = new SpawnData();
        spawnData.spawnCharacter = CreateCharacterData(characterIdx);
        return spawnData;
    }

    public CharacterData CreateCharacterData(int index)
    {
        var table = GetRecord<CharacterTable>(index);
        var data = new CharacterData();
        data.statusData = CreateStatusData(table.StatusIndex);
        data.resourceName = table.ResourceName;
        data.characterType = table.CharacterType;
        return data;
    }

    public StatusData CreateStatusData(int index)
    {
        var table = GetRecord<StatusTable>(index);
        var result = new StatusData();
        result.hp = table.Hp;
        result.atk = table.Atk;
        result.def = table.Def;
        result.evade = table.Evade;
        result.accuracyRate = table.AccuracyRate;
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