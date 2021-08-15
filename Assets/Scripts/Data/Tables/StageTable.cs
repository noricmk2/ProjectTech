using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTable : TableData
{
    private int[] _waveIndexes;
    private string _mapData;
    private Vector2 _mapSize;
    public int[] WaveIndexes => _waveIndexes;
    public string MapData => _mapData;
    public Vector2 MapSize => _mapSize;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _waveIndexes = ReadIntArray(_tabs[1]);
        _mapData = _tabs[2];
        _mapSize = ReadVector2(_tabs[3]);
    }
}
