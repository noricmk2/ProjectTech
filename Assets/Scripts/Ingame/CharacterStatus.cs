using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus
{
    private StatusData _orgStatusData;
    private StatusData _curStatusData;
    
    public void Init(StatusData data)
    {
        _orgStatusData = data;
        _curStatusData = _orgStatusData;
    }

    public float GetStatus(StatusType type)
    {
        switch (type)
        {
            case StatusType.Hp:
                return _curStatusData.hp;
            case StatusType.Atk:
                return _curStatusData.atk;
            case StatusType.Def:
                return _curStatusData.def;
            case StatusType.Evade:
                return _curStatusData.evade;
            case StatusType.AccuracyRate:
                return _curStatusData.accuracyRate;
            case StatusType.MoveRange:
                return _curStatusData.moveRange;
            case StatusType.AtkRange:
                return _curStatusData.atkRange;
            case StatusType.MoveSpeed:
                return _curStatusData.moveSpeed;
            case StatusType.AtkSpeed:
                return _curStatusData.atkSpeed;
        }
        return 0;
    }
}
