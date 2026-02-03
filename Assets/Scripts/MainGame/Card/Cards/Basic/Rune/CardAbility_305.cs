using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_305 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.SpellBoost));
    }

    public override void SpellBoost(bool isOwn)
    {
        // コストマイナス1
    }

}
