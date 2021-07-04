using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController
{
    private List<EnemyCharacter> _allEnemyList = new List<EnemyCharacter>();
    private List<EnemyCharacter> _activateEnemyList = new List<EnemyCharacter>();
    private List<WaveData> _waveDataList;
    
    public void Init(List<WaveData> dataList)
    {
        _waveDataList = dataList;
    }

    public void OnUpdate()
    {
        
    }

    public void OnSpawnCooldown(int spawnID)
    {
        
    }
}
