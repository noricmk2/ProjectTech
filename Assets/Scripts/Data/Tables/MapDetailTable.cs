using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDetailTable : TableData
{
    private string _tileResourceName;
    private string _subResourceName;
    private int _valueType;
    private int _numberValue;
    private string _stringValue;

    public string TileResourceName => _tileResourceName;
    public string SubResourceName => _subResourceName;
    public MapDetailValueType ValueType => (MapDetailValueType)_valueType;
    public int NumberValue => _numberValue;
    public string StringValue => _stringValue;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _tileResourceName = _tabs[1];
        _subResourceName = _tabs[2];
        _valueType = ReadInt(_tabs[3]);
        _numberValue = ReadInt(_tabs[4]);
        _stringValue = _tabs[5];
    }
}
