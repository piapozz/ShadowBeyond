using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : BaseEffect
{
    public DamageEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(BaseComponent targetComponent)
    {
        targetComponent.DealDamage(param[0]);
    }

    public override void ExecuteEffect(List<BaseComponent> targetComponents)
    {
        targetComponents.ForEach(targetComponent => targetComponent.DealDamage(param[0]));
    }
}
