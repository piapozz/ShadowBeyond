using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : BaseEffect
{
    public HealEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        foreach (var target in context.targets)
        {
             ExecuteEffect(target);
        }
        return null;
    }

    public override void ExecuteEffect(BaseComponent targetComponent)
    {
        targetComponent.HealDamage(param[0]);
    }

    public override void ExecuteEffect(List<BaseComponent> targetComponents)
    {
        foreach (var target in targetComponents)
        {
            ExecuteEffect(target);
        }
    }
}
