using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    public class PlayerCharacterInitData : CharacterInitData
    {
        public ConditionNode.ConditionCheckDelegate checkFindEnemyDelegate;
        public ConditionNode.ConditionCheckDelegate checkMoveDelegate;
    }

    public override void Init(CharacterInitData data)
    {
        base.Init(data);
        var pcData = data as PlayerCharacterInitData;

        var rootNode = new SelectorNode();
        rootNode.SetOwner(this);

        var baseSequence = new SequenceNode();
        rootNode.AddNode(baseSequence);

        var deadChcek = new ConditionNode();
        deadChcek.ConditionCheckFunc = null;
        baseSequence.AddNode(deadChcek);

        var dead = new DeadNode();
        deadChcek.AddNode(dead);

        var findEnemyCheck = new ConditionNode();
        findEnemyCheck.ConditionCheckFunc = pcData.checkFindEnemyDelegate;
        baseSequence.AddNode(findEnemyCheck);

        var attackSequence = new SequenceNode();
        findEnemyCheck.AddNode(attackSequence);

        var attack = new AttackNode();
        var skill = new ExcuteSkillNode();
        attackSequence.AddNode(attack);
        attackSequence.AddNode(skill);

        var findMoveCheck = new ConditionNode();
        findMoveCheck.ConditionCheckFunc = pcData.checkMoveDelegate;
        baseSequence.AddNode(findMoveCheck);

        var moveSequence = new SequenceNode();
        findMoveCheck.AddNode(moveSequence);

        var move = new MoveNode();
        var hide = new HideNode();
        moveSequence.AddNode(move);
        moveSequence.AddNode(hide);

        var idle = new IdleNode();
        baseSequence.AddNode(idle);

        _behaviorTree.Init(rootNode);
    }
}
