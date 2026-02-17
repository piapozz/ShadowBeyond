using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveEffect : BaseEffect
{
    public EvolveEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        foreach (var target in context.targets)
        {
            if (target is CardData targetCard)
            {
                ExecuteEffect(targetCard);
            }
        }
        return null;
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        targetCard.GetCardObject().EvolveFollower();
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        foreach (var target in targetCards)
        {
            if (target is CardData targetCard)
            {
                ExecuteEffect(targetCard);
            }
        }
    }
}
