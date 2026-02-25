using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_001420 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward, null));

        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm, null));

    }
}

