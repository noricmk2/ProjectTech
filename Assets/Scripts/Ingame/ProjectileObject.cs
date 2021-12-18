using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MoveObject, IPoolObjectBase
{
    public class ProjectileInitData
    {
        public CharacterBase owner;
        public ProjectileMoveType moveType;
        public float moveSpeed;
        public Transform target;
    }

    private CharacterBase _owner;
    private ProjectileMoveType _moveType;
    private Transform _target;
    private Vector3 _dir;
    
    private bool _waitRemove;
    public bool WaitRemove => _waitRemove;
    
    public void Init(ProjectileInitData data)
    {
        MoveInit();
        _owner = data.owner;
        _moveType = data.moveType;
        _moveSpeed = data.moveSpeed;
        _waitRemove = false;
        _target = data.target;
        _dir = _target.position - CachedTransform.position;
        _dir.y = 0;
        _dir = _dir.normalized;
    }

    public void OnCollisionEnter(Collision other)
    {
        DebugEx.Log($"[Collision] {this} : {other}");
        var character = other.transform.GetComponent<CharacterBase>();
        if (character != null)
        {
            if (character.GetCharacterType() != _owner.GetCharacterType())
            {
                var damage = new DamageData();
                damage.atkDamage = _owner.GetStatus().GetStatusValueByType(StatusType.Atk);
                character.OnDamaged(damage);
                _waitRemove = true;
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        SetRotate(_dir);

        switch (_moveType)
        {
            case ProjectileMoveType.Curved:
                break;
            case ProjectileMoveType.RotationDirect:
                break;
            case ProjectileMoveType.ToTargetDirect:
                if (_target != null)
                    MoveDirection(_dir.normalized, _moveSpeed);
                break;
        }
    }

    public void PushAction()
    {
        gameObject?.SetActive(false);
    }

    public void PopAction()
    {
        gameObject?.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        if(gameObject != null) 
            return gameObject;
        return null;
    }
}
