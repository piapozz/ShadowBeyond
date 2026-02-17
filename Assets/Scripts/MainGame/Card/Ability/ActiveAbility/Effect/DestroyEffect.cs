using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : BaseEffect
{
    public DestroyEffect(List<int> setParam) : base(setParam)
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

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        if (targetCard.HaveKeyword(GameEnum.KeywordAbility.NoDestroy)) return;
        targetCard.Destroy();
        targetCard.GetCardObject().CheckDestroyCard();
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
