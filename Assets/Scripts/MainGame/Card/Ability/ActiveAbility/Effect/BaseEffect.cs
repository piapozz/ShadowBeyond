using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect
{
    protected List<int> param = null;

    public BaseEffect(List<int> setParam = null)
    {
        param = setParam;
    }

    public abstract void ExecuteEffect(List<BaseComponent> targetCard);
}
