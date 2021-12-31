using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    public class UIDataBase
    {
    }

    public virtual void Init(UIDataBase data)
    {
    }

    public virtual bool PreActivate()
    {
        return true;
    }

    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual bool PreDeactivate()
    {
        return true;
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
