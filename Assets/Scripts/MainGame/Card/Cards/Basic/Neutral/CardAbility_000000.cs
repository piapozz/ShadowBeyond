using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_000000 : BaseCardAbility
{
    const int ENHANCE_COST = 4;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Enhance, null, ENHANCE_COST));
    }

    public override void Enhance(bool isOwn)
    {
        // effect‚É“n‚·
        BuffEffect effect = new BuffEffect(new List<int> { 3, 3 });
        effect.ExecuteEffect(sourceData);
    }
}
