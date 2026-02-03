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
        CardData targetCard = targetComponent as CardData;
        if (targetCard != null)
        {
            // オブジェクトを除外
            targetCard.GetObject().CheckDestroyCard();
        }
    }

    public override void ExecuteEffect(List<BaseComponent> targetComponents)
    {
        for (int i = 0, max = targetComponents.Count; i < max; i++)
        {
            ExecuteEffect(targetComponents[i]);
        }
    }
}
