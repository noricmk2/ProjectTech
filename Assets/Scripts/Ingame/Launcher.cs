using System.Collections;
using System.Collections.Generic;
using TCUtil;
using UnityEngine;

public class Launcher
{
    public enum LaunchState
    {
        Stational,
        StartFire,
        Reload,
        Skill,
    }
    
    public class LauncherInitData
    {
        public CharacterBase owner;
        public LauncherTable tableData;
    }

    private CharacterBase _owner;
    private LauncherTable _tableData;
    private float _autoFireDeltaTime;
    private float _reloadDeltaTime;
    private int _createCount;
    private int _maxBulletCount;
    private int _curLaunchCount;
    private LaunchState _launchState;
    private Transform _launcherSlot;
    private Transform _attackTarget;
    
    public void Init(LauncherInitData data)
    {
        _autoFireDeltaTime = 0;
        _reloadDeltaTime = 0;
        _createCount = 0;
        _curLaunchCount = 0;
        _maxBulletCount = data.tableData.BulletCount;
        _launchState = LaunchState.Stational;
        _owner = data.owner;
        _tableData = data.tableData;
    }

    public void Fire(Transform launcherSlot, CharacterBase attackTarget = null)
    {
        if(_launchState == LaunchState.Reload)
            return;
        
        if (_tableData.AttackType == AttackType.Immediate && attackTarget != null)
        {
            var damage = new DamageData();
            damage.atkDamage = _owner.GetStatus().GetStatusValueByType(StatusType.Atk);
            attackTarget.OnDamaged(damage);
            ++_curLaunchCount;
            if (_curLaunchCount >= _maxBulletCount)
                _launchState = LaunchState.Reload;
        }
        else
        {
            if (attackTarget != null)
            {
                _attackTarget = attackTarget.CachedTransform;
                _launcherSlot = launcherSlot;
                _launchState = LaunchState.StartFire;
            }
        }
    }

    private void Reload()
    {
        //DebugEx.Log($"[Reload] {_owner}");
        if (_reloadDeltaTime < _tableData.ReloadTime)
            _reloadDeltaTime += Time.deltaTime;
        else
        {
            _curLaunchCount = 0;
            _reloadDeltaTime = 0;
            _launchState = LaunchState.Stational;
        }
    }

    public void OnUpdate()
    {
        switch (_launchState)
        {
            case LaunchState.Stational:
                break;
            case LaunchState.StartFire:
                Launch();
                break;
            case LaunchState.Reload:
                Reload();
                break;
            case LaunchState.Skill:
                break;
        }
    }

    private void Launch()
    {
        if(_createCount == 0) 
            CreateProjectile(_launcherSlot);
        
        if (_autoFireDeltaTime < _tableData.AutoFireDelay)
        {
            _autoFireDeltaTime += Time.deltaTime;
        }
        else
        {
            _autoFireDeltaTime = 0;
            CreateProjectile(_launcherSlot);
        }
        
        if (_createCount < _tableData.AutoFireCount)
            return;

        ++_curLaunchCount;
        _autoFireDeltaTime = 0;
        _createCount = 0;
        _launcherSlot = null;
        if (_curLaunchCount >= _maxBulletCount)
        {
            _launchState = LaunchState.Reload;
            _owner.SetAnimatorTrigger("Idle");
        }
        else
            _launchState = LaunchState.Stational;
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
        initData.slot = launcherSlot;
        projectile.Init(initData);
        IngameManager.Instance.RegistProjectile(projectile);
        ++_createCount;
    }
}
