using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseAbilityEffect : BaseEffect
{
    public LoseAbilityEffect(List<int> setParam) : base(setParam)
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
        if (param == null)
        {
            card.ClearAllAbility();
        }
        else
        {
            card.RemoveKeyword((GameEnum.KeywordAbility)param[0]);
        }
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        ExecuteEffect(targetCard);
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        foreach (var target in targetCards)
        {
            if (target is CardData card)
            {
                ExecuteEffect(card);
            }
        }
    }
}
