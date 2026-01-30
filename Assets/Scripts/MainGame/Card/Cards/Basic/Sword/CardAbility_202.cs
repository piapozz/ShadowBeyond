using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_202 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        base.Initialize(setCard);
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush, null));
    }
}
