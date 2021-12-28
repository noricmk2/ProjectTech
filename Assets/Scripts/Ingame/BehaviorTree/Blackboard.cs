using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : Singleton<Blackboard>
{
    
    #region AIMethod
    public static bool CheckFindEnemy(IBehaviorTreeOwner owner)
    {
        if (owner is CharacterBase)
        {
            var characterBase = owner as CharacterBase;
            return characterBase.FindAttackTarget();
        }
        
        return false;
    }
    
    public static bool CheckFindMove(IBehaviorTreeOwner owner)
    {
        if (owner is CharacterBase)
        {
            var character = owner as CharacterBase;
            return character.FindMoveTarget();
        }
        
        return false;
    }

    public static bool CheckDead(IBehaviorTreeOwner owner)
    {
        if (owner is CharacterBase)
        {
            var character = owner as CharacterBase;
            return character.CheckDead();
        }

        return false;
    }

    public static bool OnAIMove(IBehaviorTreeOwner owner, Action onMoveEnd)
    {
        if (owner is CharacterBase)
        {
            var character = owner as CharacterBase;
            return character.MoveToSearchedPath(onMoveEnd);
        }
        
        return false;
    }

    public static bool OnAIAttack(IBehaviorTreeOwner owner, Action onAttackEnd)
    {
        if (owner is CharacterBase)
        {
            var character = owner as CharacterBase;
            return character.Attack(onAttackEnd);
        }

        return false;
    }
    #endregion
}
