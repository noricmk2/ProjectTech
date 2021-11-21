using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Transform _spawnRoot;
    [SerializeField] private string _spawnerName;
    #endregion

    #region Property
    private SpawnData _curSpawnData;
    private WaveController _controller;
    private float _delay;
    private float _deltaTime;

    public string SpawnerName => _spawnerName;
    #endregion

    public void Init(SpawnData data, WaveController controller)
    {
        _curSpawnData = data;
        _controller = controller;
        _delay = data.spawnDelay;
        _deltaTime = 0;
    }

    public void SetSpanwerName(string name)
    {
        _spawnerName = name;
    }
    
    public CharacterBase SpawnCharacter()
    {
        if (_curSpawnData != null)
        {
            if (_curSpawnData.spawnCharacter.characterType == CharacterType.Enemy)
            {
                var charData = _curSpawnData.spawnCharacter;
                //var enemy = ObjectFactory.Instance.CreateObject<EnemyCharacter>(charData.resourceName, IngameManager.Instance.CharacterRoot);
                var enemy = ObjectFactory.Instance.GetPoolObject<EnemyCharacter>(charData.resourceName);
                enemy.transform.Init(IngameManager.Instance.CharacterRoot);
                enemy.CachedTransform.position = new Vector3(transform.position.x, 0, transform.position.z);
                var initData = new CharacterBase.CharacterInitData();
                initData.charData = _curSpawnData.spawnCharacter;
                initData.launcherTableList = DataManager.Instance.GetLauncherTableList(initData.charData.index);
                initData.aiData = DataManager.Instance.CreateAIData(_curSpawnData.spawnCharacter.aiDataName);
                enemy.Init(initData);
                enemy.CachedTransform.rotation = Quaternion.Euler(0, -180, 0);
                return enemy;
            }
            else
            {
                return null;
            }
        }
        else
        {
            DebugEx.LogError($"[Failed] not exist spawn data");
            return null;
        }
    }
    
    private void Update()
    {
        if (_deltaTime >= _delay)
        {
        }
    }
}
