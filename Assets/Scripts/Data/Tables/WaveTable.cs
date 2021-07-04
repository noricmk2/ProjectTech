using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTable : TableData
{
    private int[] _characterIndexes;
    private Vector2[] _spawnPoints;

    public int[] CharacterIndexes => _characterIndexes;
    public Vector2[] SpawnPoints => _spawnPoints;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _characterIndexes = ReadIntArray(_tabs[1]);
        _spawnPoints = ReadVector2Array(_tabs[2]);
    }
}
