using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MoveObject, IBehaviorTreeOwner
{
    #region Define
    public class CharacterInitData
    {
        public CharacterData charData;
    }
    #endregion

    protected BehaviorTree _behaviorTree = new BehaviorTree();
    public virtual void Init(CharacterInitData data)
    {
    }
}


