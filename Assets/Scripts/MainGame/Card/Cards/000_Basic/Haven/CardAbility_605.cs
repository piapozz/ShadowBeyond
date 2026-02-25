using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_605 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Ward));

        // 自分がアミュレットを【アクト】したとき、これは【疾走】を持つ。
    }
}
