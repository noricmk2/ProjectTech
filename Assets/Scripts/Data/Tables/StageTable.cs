using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTable : TableData
{
    private int[] _waveIndexes;
    private string _mapData;
    private string _mapDetail;
    private Vector2 _mapSize;
    private string _mapPrefab;
    public int[] WaveIndexes => _waveIndexes;
    public string MapData => _mapData;
    public string MapDetail => _mapDetail;
    public Vector2 MapSize => _mapSize;
    public string MapPrefab => _mapPrefab;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _waveIndexes = ReadIntArray(_tabs[1]);
        _mapData = _tabs[2];
        _mapDetail = _tabs[3];
        _mapSize = ReadVector2(_tabs[4]);
        _mapPrefab = _tabs[5];
    }
}
