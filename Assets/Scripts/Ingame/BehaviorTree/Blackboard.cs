using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{

}

public class BattleBlackBoard : Blackboard
{
    private List<CharacterBase> _characterList;
    private MapController.MapData _mapData;
}
