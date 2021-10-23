using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController
{
    private List<ProjectileObject> _projectileList = new List<ProjectileObject>();

    public void Init()
    {
    }

    public void AddProjectile(ProjectileObject projectile)
    {
        if(!_projectileList.Contains(projectile))
            _projectileList.Add(projectile);
    }

    public void OnUpdate()
    {
        for (int i = 0; i < _projectileList.Count;)
        {
            var projectile = _projectileList[i];
            if (projectile.WaitRemove)
            {
                ObjectFactory.Instance.DeactivePoolObject(projectile);
                _projectileList.Remove(projectile);
            }
            else
            {
                _projectileList[i].OnUpdate();
                ++i;
            }
        }
    }

    public void Release()
    {
        for (int i = 0; i < _projectileList.Count; ++i)
        {
            ObjectFactory.Instance.DeactivePoolObject(_projectileList[i]);
        }
        _projectileList.Clear();
    }
}
