using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController
{
    private List<EnemyCharacter> _allEnemyList = new List<EnemyCharacter>();
    private List<EnemyCharacter> _activateEnemyList = new List<EnemyCharacter>();
    private Queue<WaveData> _waveDataQueue;
    
    public void Init(List<WaveData> dataList)
    {
        _waveDataQueue = new Queue<WaveData>(dataList);
    }

    public void OnUpdate()
    {
        if (_waveDataQueue.Count > 0)
        {
            if (_activateEnemyList.Count == 0)
            {
                var waveData = _waveDataQueue.Dequeue();
                CreateWave(waveData);
            }
            else
            {
                for (int i = 0; i < _activateEnemyList.Count; ++i)
                {
                    _activateEnemyList[i].OnUpdate();
                }
            }
        }
        else
        {
            if (_activateEnemyList.Count == 0)
            {
                
            }
            else
            {
                
            }
        }
    }

    private void CreateWave(WaveData wave)
    {
        var spawnList = wave.spawnDataList;
        for (int i = 0; i < spawnList.Count; ++i)
        {
            var spawnData = spawnList[i];
            var targetSpawner = MapManager.Instance.GetSpawnerByName(spawnData.spawnerName);
            if(targetSpawner != null)
                targetSpawner.Init(spawnData, this);
            var character = targetSpawner.SpawnCharacter() as EnemyCharacter;
            _activateEnemyList.Add(character);
            character.SetAIEnable(true);
        }
    }
}
