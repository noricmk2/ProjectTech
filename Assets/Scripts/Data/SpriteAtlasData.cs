using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteAtlasData
{
    private static readonly string addressFormat = "{0}[{1}]";

    public Dictionary<string, List<string>> atlasInfo = new Dictionary<string, List<string>>();

    public void SetAtlasInfo(string atlasName, List<string> spriteNames)
    {
        atlasInfo[atlasName] = spriteNames;
    }

    public void SetAtlasInfo(string atlasName, string spriteName)
    {
        if (!atlasInfo.ContainsKey(atlasName))
            atlasInfo[atlasName] = new List<string>();

        atlasInfo[atlasName].Add(spriteName);
    }

    public void SortList(string atlasName)
    {
        atlasInfo[atlasName].Sort();
    }

    public string GetAtlasBySpriteName(string spriteName)
    {
        var iter = atlasInfo.GetEnumerator();
        while (iter.MoveNext())
        {
            if (0 <= iter.Current.Value.BinarySearch(spriteName))
                return iter.Current.Key;
        }

        DebugEx.Log($"[Failed] failed to search atlas by sprite:{spriteName}");
        return null;
    }

    public string GetAddressBySpriteName(string spriteName)
    {
        var atlas = GetAtlasBySpriteName(spriteName);
        if (!string.IsNullOrEmpty(atlas))
            return string.Format(addressFormat, atlas, spriteName);
        return null;
    }
}
