using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTable : TableData
{
    private string _resourceName;
    private int _statusIndex;
    private int[] _skillIndexes;
    private int _characterType;
    private int[] _launcherIndexes;

    public string ResourceName => _resourceName;
    public int StatusIndex => _statusIndex;
    public int[] SkillIndexes => _skillIndexes;
    public CharacterType CharacterType => (CharacterType)_characterType;
    public int[] LanucherIndexes => _launcherIndexes;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _resourceName = _tabs[1];
        _statusIndex = ReadInt(_tabs[2]);
        _skillIndexes = ReadIntArray(_tabs[3]);
        _characterType = ReadInt(_tabs[4]);
        _launcherIndexes = ReadIntArray(_tabs[5]);
    }
}
