using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_201 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm, null));
    }

    public override void LastWord(bool isOwn)
    {
        // Ž©•ª‚ÌƒfƒbƒL‚©‚ç1–‡‚ðˆø‚­
    }
}
