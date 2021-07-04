using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TCUtil;
using UnityEngine;

public class TableData
{
    protected string[] _tabs;
    private int _index;
    public int Index => _index;
    
    public virtual void ParsingData(string line)
    {
        _tabs = line.Split('\t');
        int.TryParse(_tabs[0], out _index);
    }

    protected int ReadInt(string tab)
    {
        return int.Parse(tab);
    }
    
    protected int[] ReadIntArray(string tab)
    {
        var split = tab.Split(';');
        var array = new int[split.Length];
        for (int i = 0; i < split.Length; ++i)
            array[i] = int.Parse(split[i]);
        return array;
    }
    
    protected float ReadFloat(string tab)
    {
        return float.Parse(tab);
    }
    
    protected float[] ReadFloatArray(string tab)
    {
        var split = tab.Split(';');
        var array = new float[split.Length];
        for (int i = 0; i < split.Length; ++i)
            array[i] = float.Parse(split[i]);
        return array; 
    }
    
    protected Vector2 ReadVector2(string tab)
    {
        var index = tab.IndexOf(")");
        var subString = tab.Substring(1, tab.Length - 2);
        var split = subString.Split(',');
        var vector = new Vector2();
        vector.x = float.Parse(split[0]);
        vector.y = float.Parse(split[1]);
        return vector;
    }
    
    protected Vector2[] ReadVector2Array(string tab)
    {
        var split = tab.Split(';');
        var vectors = new Vector2[split.Length];
        for (int i = 0; i < split.Length; ++i)
            vectors[i] = ReadVector2(split[i]);
        return vectors;
    }

    protected Vector3 ReadVector3(string tab)
    {
        var index = tab.IndexOf(")");
        var subString = tab.Substring(1, tab.Length - 2);
        var split = subString.Split(',');
        var vector = new Vector3();
        vector.x = float.Parse(split[0]);
        vector.y = float.Parse(split[1]);
        vector.z = float.Parse(split[2]);
        return vector;
    }
    
    protected Vector3[] ReadVector3Array(string tab)
    {
        var split = tab.Split(';');
        var vectors = new Vector3[split.Length];
        for (int i = 0; i < split.Length; ++i)
            vectors[i] = ReadVector3(split[i]);
        return vectors;
    }

    protected bool ReadBoolInt(string tab)
    {
        var num = int.Parse(tab);
        return num == 1;
    }
    
    protected bool ReadBoolText(string tab)
    {
        if (Func.FastIndexOf(tab.ToLower(), "true") != -1)
            return true;
        else
            return false;
    }

    protected string[] ReadStringArray(string tab)
    {
        return tab.Split(';');
    }
}
