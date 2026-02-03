using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_407 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Intimidate));
    }

}
