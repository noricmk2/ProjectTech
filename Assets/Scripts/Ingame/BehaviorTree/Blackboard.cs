using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : Singleton<Blackboard>
{
    
    #region AIMethod
    public static bool CheckFindEnemy(IBehaviorTreeOwner owner)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.FindAttackTarget();
        }
        
        return false;
    }
    
    public static bool CheckFindMove(IBehaviorTreeOwner owner)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.FindMoveTarget();
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
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.MoveToSearchedPath(onMoveEnd);
        }

        return false;
    }

    public static bool OnAIAttack(IBehaviorTreeOwner owner, Action onAttackEnd)
    {
        if (owner is EnemyCharacter)
        {
            var enemy = owner as EnemyCharacter;
            return enemy.Attack(onAttackEnd);
        }

        return false;
    }
    #endregion
}
