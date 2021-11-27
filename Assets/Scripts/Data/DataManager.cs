using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TCUtil;
using UnityEngine;
using UnityEngine.Assertions;

#region Define
public class MapRawData
{
    public int width;
    public int height;
    public List<JPSNode> nodeList;
    public int[,] mapDetailData;
}

public class StageData
{
    public List<WaveData> waveList;
    public MapRawData mapData;
}

public class CharacterData
{
    public int index;
    public string resourceName;
    public string aiDataName;
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
    public float moveRange;
    public float atkRange;
    public float moveSpeed;
    public float atkSpeed;

    public StatusData Clone()
    {
        var clone = new StatusData();
        clone.hp = hp;
        clone.atk = atk;
        clone.def = def;
        clone.evade = evade;
        clone.accuracyRate = accuracyRate;
        clone.moveRange = moveRange;
        clone.atkRange = atkRange;
        clone.moveSpeed = moveSpeed;
        clone.atkSpeed = atkSpeed;
        return clone;
    }
}

public class WaveData
{
    public int waveOrder;
    public List<SpawnData> spawnDataList;
}

public class SpawnData
{
    public string spawnerName;
    public CharacterData spawnCharacter;
    public float spawnDelay;
}

public class DamageData
{
    public float atkDamage;
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
        var table = GetRecord<StageTable>(index);
        var rawMapData = AddressableManager.Instance.LoadAssetSync<TextAsset>(table.MapData);
        var rawMapDetail = AddressableManager.Instance.LoadAssetSync<TextAsset>(table.MapDetail);
        var result = new StageData();
        var mapData = new MapRawData();
        mapData.width = (int)table.MapSize.x;
        mapData.height = (int)table.MapSize.y;
        mapData.nodeList = new List<JPSNode>();
        mapData.mapDetailData = new int[mapData.width, mapData.height];
        var mapNodeList = GetNodeListByRawMapData(rawMapData.text);
        var mapDetailList = GetNodeListByRawMapData(rawMapDetail.text);
        for (int i = 0; i < mapNodeList.Count; ++i)
        {
            int x = i % mapData.width;
            int y = i / mapData.width;
            var node = new JPSNode(x, y, mapNodeList[i]);
            mapData.nodeList.Add(node);
            mapData.mapDetailData[x, y] = mapDetailList[i];
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
            for (int j = 0; j < waveTable.CharacterIndexes.Length; ++j)
                waveData.spawnDataList.Add(CreateSpawnData(waveTable.CharacterIndexes[j], waveTable.SpawnNames[j],
                    waveTable.SpawnDelay[j]));
            result.Add(waveData);
        }

        return result;
    }

    public SpawnData CreateSpawnData(int characterIdx, string spawnPoint, float spawnDelay)
    {
        var spawnData = new SpawnData();
        spawnData.spawnCharacter = CreateCharacterData(characterIdx);
        spawnData.spawnerName = spawnPoint;
        spawnData.spawnDelay = spawnDelay;
        return spawnData;
    }

    public CharacterData CreateCharacterData(int index)
    {
        var table = GetRecord<CharacterTable>(index);
        var data = new CharacterData();
        data.index = index;
        data.statusData = CreateStatusData(table.StatusIndex);
        data.resourceName = table.ResourceName;
        data.aiDataName = table.AIData;
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
        result.moveRange = table.MoveRange;
        result.atkRange = table.AttackRange;
        result.moveSpeed = table.MoveSpeed;
        result.atkSpeed = table.AttackSpeed;
        return result;
    }
    
    public CharacterBase.AIData CreateAIData(string aiDataName)
    {
        var aiRawData = AddressableManager.Instance.LoadAssetSync<AIRawData>(aiDataName);
        Assert.IsNotNull(aiRawData, $"[Failed] ai asset is null : {aiDataName}");
        var tree = new BehaviorTree();
        
        aiRawData.treeDataList.Sort((x, y) =>
        {
            if (x.IsRoot)
                return -1;
            else if (y.IsRoot)
                return 1;

            if (x.Level < y.Level)
                return -1;
            else if (x.Level > y.Level)
                return 1;

            if (x.Order < y.Order)
                return -1;
            else if (x.Order > y.Order)
                return 1;
            
            return 0;
        });

        for (int i = 0; i < aiRawData.treeDataList.Count; ++i)
        {
            var nodeData = aiRawData.treeDataList[i];
            if (nodeData.IsRoot)
            {
                var rootNode = new SelectorNode();
                rootNode.SetName(nodeData.NodeName);
                tree.Init(rootNode);
            }
            else
            {
                Type type = Func.GetType(nodeData.NodeType.ToString());
                var targetNode = Activator.CreateInstance(type) as BehaviorTreeNode;
                Assert.IsNotNull(targetNode, $"[Failed] node create failed {nodeData.NodeType}/{nodeData.NodeName}");
                targetNode.SetName(nodeData.NodeName);
                if (!string.IsNullOrEmpty(nodeData.ActionName))
                    targetNode.SetNodeAction(nodeData.ActionName);
                tree.AddNode(nodeData.ParentName, targetNode);
            }
        }

        var data = new CharacterBase.AIData();
        data.behaviorTree = tree;
        return data;
    }
    
    public List<LauncherTable> GetLauncherTableList(int charIdx)
    {
        var charTable = GetRecord<CharacterTable>(charIdx);
        return charTable.LanucherIndexes.Select(x => GetRecord<LauncherTable>(x)).ToList();
    }

    private List<int> GetNodeListByRawMapData(string text)
    {
        var result = new List<int>();
        string trim = text.Replace("\r", "");
        trim = trim.Replace("\n", "");
        var split = trim.Split(',');

        for (int i = 0; i < split.Length; ++i)
        {
            int nodeValue = int.Parse(split[i]);
            result.Add(nodeValue);
        }
        return result;
    }
}