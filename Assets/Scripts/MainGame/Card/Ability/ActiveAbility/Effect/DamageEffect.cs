using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : BaseEffect
{
    public DamageEffect(List<int> setParam) : base(setParam)
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
        targetComponent.DealDamage(param[0]);
        if (targetComponent is CardData card)
        {
            // オブジェクトを除外
            card.GetCardObject().CheckDestroyCard();
        }
    }

    public override void ExecuteEffect(List<BaseComponent> targetComponents)
    {
        foreach (var target in targetComponents)
        {
            ExecuteEffect(target);
        }
    }
}
