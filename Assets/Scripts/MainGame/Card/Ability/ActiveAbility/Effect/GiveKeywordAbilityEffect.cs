using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveKeywordAbilityEffect : BaseEffect
{
    public GiveKeywordAbilityEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        foreach (var target in context.targets)
        {
            if (target is CardData targetCard)
            {
                ExecuteEffect(targetCard, context.sourceCard);
            }
        }
        return null;
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        KeywordAbilityInstance keywordAbility = new KeywordAbilityInstance((GameEnum.KeywordAbility)param[0], sourceCard);
        targetCard.AddKeyword(keywordAbility);
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        foreach (var target in targetCards)
        {
            if (target is CardData targetCard)
            {
                ExecuteEffect(targetCard, sourceCard);
            }
        }
    }
}
