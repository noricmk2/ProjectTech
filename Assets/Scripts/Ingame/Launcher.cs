using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;

public class Launcher
{
    public class LauncherInitData
    {
        public CharacterBase owner;
        public LauncherTable tableData;
    }

    private CharacterBase _owner;
    private LauncherTable _tableData;
    private float _deltaTime;
    private int _createCount;
    private bool _startFire;
    private Transform _launcherSlot;
    private Transform _attackTarget;
    
    public void Init(LauncherInitData data)
    {
        _deltaTime = 0;
        _createCount = 0;
        _startFire = false;
        _owner = data.owner;
        _tableData = data.tableData;
    }

    public void Fire(Transform launcherSlot, CharacterBase attackTarget = null)
    {
        if (_tableData.AttackType == AttackType.Immediate && attackTarget != null)
        {
            var damage = new DamageData();
            damage.atkDamage = _owner.GetStatus().GetStatusValueByType(StatusType.Atk);
            attackTarget.OnDamaged(damage);
        }
        else
        {
            if (attackTarget != null)
            {
                _attackTarget = attackTarget.CachedTransform;
                _launcherSlot = launcherSlot;
                _startFire = true;   
            }
        }
    }

    public void OnUpdate()
    {
        if (_startFire)
        {
            CreateProjectile(_launcherSlot);
            while (_createCount < _tableData.AutoFireCount)
            {
                if (_deltaTime < _tableData.AutoFireDelay)
                {
                    _deltaTime += Time.deltaTime;
                }
                else
                {
                    _deltaTime = 0;
                    CreateProjectile(_launcherSlot);
                }
            }
            _deltaTime = 0;
            _createCount = 0;
            _startFire = false;
            _launcherSlot = null;
        }
    }

    private void CreateProjectile(Transform launcherSlot)
    {
        var projectile = ObjectFactory.Instance.GetPoolObject<ProjectileObject>(_tableData.ResourceName);
        projectile.transform.Init(IngameManager.Instance.ProjectileRoot);
        projectile.transform.position = launcherSlot.position;
        var initData = new ProjectileObject.ProjectileInitData();
        initData.owner = _owner;
        initData.moveSpeed = _tableData.ProjectileSpeed;
        initData.moveType = _tableData.ProjectileMoveType;
        initData.target = _attackTarget;
        projectile.Init(initData);
        IngameManager.Instance.RegistProjectile(projectile);
        ++_createCount;
    }
}
