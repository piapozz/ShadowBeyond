using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveKeywordAbilityEffect : BaseEffect
{
    public GiveKeywordAbilityEffect(List<int> setParam) : base(setParam)
    {

    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        KeywordAbilityInstance keywordAbility = new KeywordAbilityInstance((GameEnum.KeywordAbility)param[0], sourceCard);
        targetCard.AddKeyword(keywordAbility);
    }

    public override void ExecuteEffect(List<CardData> targetCards, CardData sourceCard = null)
    {
        for (int i = 0, max = targetCards.Count; i < max; i++)
        {
            ExecuteEffect(targetCards[i], sourceCard);
        }
    }
}
