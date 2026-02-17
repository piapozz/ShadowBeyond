using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : BaseEffect
{
    public BuffEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        foreach (var target in context.targets)
        {
            if (target is CardData card)
            {
                ExecuteEffect(card);
            }
        }
        return null;
    }

    public void ExecuteEffect(CardData card)
    {
        card.AddStatus(param[0], param[1]);
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        ExecuteEffect(targetCard);
    }

    public override void ExecuteEffect(List<CardData> targetCard, CardData sourceCard = null)
    {
        for (int i = 0, max = targetCard.Count; i < max; i++)
        {
            ExecuteEffect(targetCard[i]);
        }
    }
}
