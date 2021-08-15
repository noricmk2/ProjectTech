using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTable : TableData
{
    private int[] _characterIndexes;
    private string[] _spawnerNames;
    private float[] _spawnDelay;

    public int[] CharacterIndexes => _characterIndexes;
    public string[] SpawnNames => _spawnerNames;
    public float[] SpawnDelay => _spawnDelay;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _characterIndexes = ReadIntArray(_tabs[1]);
        _spawnerNames = ReadStringArray(_tabs[2]);
        _spawnDelay = ReadFloatArray(_tabs[3]);
    }
}
