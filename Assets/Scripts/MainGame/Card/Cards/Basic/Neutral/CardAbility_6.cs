using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility_6 : BaseCardAbility
{
    private const int ENGAGE_COST = 0;
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Engage, null, ENGAGE_COST));
    }

    public override void Fanfare(bool isOwn)
    {
        // フォロワーを一枚引く
    }

    public override void Engage(bool isOwn)
    {
        // これを破壊

        // 自分の場のをフォロワーを1体選ぶ。突進
    }
}
