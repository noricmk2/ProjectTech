using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusTable : TableData
{
    private float _hp;
    private float _atk;
    private float _def;
    private float _evade;
    private float _accuracyRate;
    private float _moveRange;
    private float _attackRange;
    private float _moveSpeed;
    private float _attackSpeed;

    public float Hp => _hp;
    public float Atk => _atk;
    public float Def => _def;
    public float Evade => _evade;
    public float AccuracyRate => _accuracyRate;
    public float MoveRange => _moveRange;
    public float AttackRange => _attackRange;
    public float MoveSpeed => _moveSpeed;
    public float AttackSpeed => _attackSpeed;

    public override void ParsingData(string line)
    {
        base.ParsingData(line);
        _hp = ReadFloat(_tabs[1]);
        _atk = ReadFloat(_tabs[2]);
        _def = ReadFloat(_tabs[3]);
        _evade = ReadFloat(_tabs[4]);
        _accuracyRate = ReadFloat(_tabs[5]);
        _moveRange = ReadFloat(_tabs[6]);
        _attackRange = ReadFloat(_tabs[7]);
        _moveSpeed = ReadFloat(_tabs[8]);
        _attackSpeed = ReadFloat(_tabs[9]);
        
    }
}
