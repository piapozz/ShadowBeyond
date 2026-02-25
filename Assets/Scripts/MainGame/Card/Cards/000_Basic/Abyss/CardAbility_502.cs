using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_502 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;

        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Bane));
    }
}
