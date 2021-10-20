public enum CameraState
{
    Stational,
    FollowTarget,
    Animation,
    Shake,
}

public enum CharacterType
{
    None,
    Player,
    Enemy,
}

public enum SpawnType
{
    Position,
    PrevWave,
}

public enum TableType
{
    Stage,
    Wave,
    Character,
    Status,
    Length
}

public enum MapNodeType
{
    Block = 0,
    Road = 1,
    Spanwer = 9
}

public enum StatusType
{
    Hp,
    Atk,
    Def,
    Evade,
    AccuracyRate,
    MoveRange,
    AtkRange,
    MoveSpeed,
    AtkSpeed,
}