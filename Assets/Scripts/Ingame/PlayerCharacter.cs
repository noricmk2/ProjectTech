using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCharacter : CharacterBase
{
    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        if (data.aiData != null)
        {
            _behaviorTree = data.aiData.behaviorTree;
            Assert.IsNotNull(_behaviorTree, $"[Failed] {data.charData.index} has no aiData");
            _behaviorTree.SetOwner(this);
        }
    }
}
