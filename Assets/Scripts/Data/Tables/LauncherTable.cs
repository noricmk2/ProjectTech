using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherTable : TableData
{
    private int _attackType;
    private string _resourceName;
    private int _autoFireCount;
    private float _projectileSpeed;
    private int _projectileMoveType;
    private float _rotation;
    private float _autoFireDelay;
    private float _launcherValue;
    private int _bulletCount;
    private float _reloadTime;
    private float _startDelay;

    public AttackType AttackType => (AttackType)_attackType;
    public string ResourceName => _resourceName;
    public int AutoFireCount => _autoFireCount;
    public float ProjectileSpeed => _projectileSpeed;
    public ProjectileMoveType ProjectileMoveType => (ProjectileMoveType)_projectileMoveType;
    public float Rotation => _rotation;
    public float AutoFireDelay => _autoFireDelay;
    public float LauncherValue => _launcherValue;
    public int BulletCount => _bulletCount;
    public float ReloadTime => _reloadTime;
    public float StartDelay => _startDelay;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _attackType = ReadInt(_tabs[1]);
        _resourceName = _tabs[2];
        _autoFireCount = ReadInt(_tabs[3]);
        _projectileSpeed = ReadFloat(_tabs[4]);
        _projectileMoveType = ReadInt(_tabs[5]);
        _rotation = ReadFloat(_tabs[6]);
        _autoFireDelay = ReadFloat(_tabs[7]);
        _launcherValue = ReadFloat(_tabs[8]);
        _bulletCount = ReadInt(_tabs[9]);
        _reloadTime = ReadFloat(_tabs[10]);
        _startDelay = ReadFloat(_tabs[11]);
    }
}
