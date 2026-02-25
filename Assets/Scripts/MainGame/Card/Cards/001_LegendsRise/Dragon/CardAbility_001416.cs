using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÅyéæëñÅz
//Åyà–à≥Åz
public class CardAbility_001416 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Storm, null));
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Intimidate, null));
    }
}
