using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseComponent
{
    private Func<BaseFieldObject> _getObject;

    public void SetGetObjectAction(Func<BaseFieldObject> action)
    {
        _getObject = action;
    }

    public BaseFieldObject GetObject()
    {
        return _getObject();
    }

    public abstract void DealDamage(int damage);

    public abstract void HealDamage(int heal);
}
