using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_3 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Engage(bool isOwn)
    {
        // これを破壊
        // 相手の場のをフォロワーを1体選ぶ。守護を失う
    }
}
