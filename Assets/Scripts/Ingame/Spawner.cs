using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Transform _spawnRoot;
    #endregion

    #region Property
    private SpawnData _curSpawnData;
    private WaveController _controller;
    private float _delay;
    private float _deltaTime;
    private int _maxSpawnCount;
    private int _curSpawnCount;
    #endregion

    public void Init(SpawnData data, WaveController controller)
    {
        _curSpawnData = data;
        _controller = controller;
        _delay = data.spawnDelay;
        _maxSpawnCount = data.count;
        _curSpawnCount = 0;
        _deltaTime = 0;
    }

    public CharacterBase SpawnCharacter()
    {
        if (_maxSpawnCount <= _curSpawnCount)
        {
            DebugEx.Log($"[Faild] spawn count is max {_curSpawnCount}/{_maxSpawnCount}");
            return null;
        }

        if (_curSpawnData != null)
        {
            ++_curSpawnCount;
            CharacterBase character = null;
            if (_curSpawnData.spawnCharacter.characterType == CharacterType.Enemy)
            {
                character = new EnemyCharacter();
            }
            else
            {
                character = new PlayerCharacter();
            }
            return character;
        }
        else
        {
            DebugEx.LogError($"[Failed] not exist spawn data");
            return null;
        }
    }
    
    private void Update()
    {
        if (_maxSpawnCount > _curSpawnCount)
        {
            if (_deltaTime >= _delay)
            {
                _controller.OnSpawnCooldown(_curSpawnData.spawnId);
            }
        }
    }
}
