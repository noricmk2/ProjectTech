using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        var aiRootNode = data.aiData.rootNode;
        if (aiRootNode == null)
            DebugEx.LogError($"[Failed] {data.charData.index} has no aiData");
        else
        {
            aiRootNode.SetOwner(this);
            _behaviorTree.Init(aiRootNode);
        }
    }
}
