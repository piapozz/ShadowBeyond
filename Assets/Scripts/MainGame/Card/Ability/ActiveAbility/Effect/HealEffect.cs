using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : BaseEffect
{
    public HealEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(BaseComponent targetComponent)
    {
        targetComponent.HealDamage(param[0]);
    }

    public override void ExecuteEffect(List<BaseComponent> targetComponents)
    {
        targetComponents.ForEach(component => component.HealDamage(param[0]));
    }
}
