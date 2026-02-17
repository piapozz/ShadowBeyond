using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBoostEffect : BaseEffect
{
    public SpellBoostEffect(List<int> setParam) : base(setParam)
    {

    }

    public override List<CardData> ExecuteEffect(EffectContext context)
    {
        // 手札すべてをスペルブースト
        if (context.targets == null)
        {
            context.player.hand.SpellBoost(param[0]);
        }
        // 特定のカードをスペルブースト
        else
        {
            foreach (var target in context.targets)
            {
                if (target is CardData card)
                {
                    ExecuteEffect(card);
                }
            }
        }
        return null;
    }

    public void ExecuteEffect(CardData targetCard)
    {
        KeywordAbilityInstance keyword = targetCard.GetKeywordAbility(GameEnum.KeywordAbility.SpellBoost);
        if (keyword == null) return;
        keyword.AddParam(param[0]);
    }

    public override List<CardData> ExecuteEffect(Hand targetHand)
    {
        // 手札のスペルブーストするカードを取得
        List<CardData> boostCards = targetHand.GetCards((card) => card.HaveKeyword(GameEnum.KeywordAbility.SpellBoost));
        for (int i = 0, max = boostCards.Count; i < max; i++)
        {
            KeywordAbilityInstance keyword = boostCards[i].GetKeywordAbility(GameEnum.KeywordAbility.SpellBoost);
            if (keyword == null) return null;
            keyword.AddParam(param[0]);
        }
        return null;
    }

    public override void ExecuteEffect(CardData targetCard, CardData sourceCard = null)
    {
        // 手札のスペルブーストするカードを取得
        KeywordAbilityInstance keyword = targetCard.GetKeywordAbility(GameEnum.KeywordAbility.SpellBoost);
        if (keyword == null) return;
    }
}
