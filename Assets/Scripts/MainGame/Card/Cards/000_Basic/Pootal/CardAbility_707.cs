using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_707 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush));

        // 相手のターン終了時、これを破壊。
    }
}
