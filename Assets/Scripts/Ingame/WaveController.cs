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
                for (int i = 0; i < _activateEnemyList.Count;)
                {
                    if (_activateEnemyList[i].WaitRemove)
                    {
                        _activateEnemyList[i].Release();
                        ObjectFactory.Instance.DeactivePoolObject(_activateEnemyList[i]);
                        _activateEnemyList.Remove(_activateEnemyList[i]);
                    }
                    else
                    {
                        _activateEnemyList[i].OnUpdate();
                        ++i;
                    }
                }
            }
        }
        else
        {
            if (_activateEnemyList.Count == 0)
            {
                var gameData = new IngameManager.GameEndData();
                gameData.victory = true;
                IngameManager.Instance.SetIngameEnd(gameData);
            }
            else
            {
                for (int i = 0; i < _activateEnemyList.Count; ++i)
                {
                    if (_activateEnemyList[i].WaitRemove)
                    {
                        _activateEnemyList[i].Release();
                        ObjectFactory.Instance.DeactivePoolObject(_activateEnemyList[i]);
                        _activateEnemyList.Remove(_activateEnemyList[i]);
                    }
                    else
                    {
                        _activateEnemyList[i].OnUpdate();
                        ++i;
                    }
                }
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

    public List<CharacterBase> GetActivateEnemyList()
    {
        var list = new List<CharacterBase>(_activateEnemyList);
        return list;
    }
}
