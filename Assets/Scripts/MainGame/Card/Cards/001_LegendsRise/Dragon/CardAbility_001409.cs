using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// これが捨てられたとき、自分の場のフォロワーからランダム1枚は+1/+0する。
//【突進】
public class CardAbility_001409 : BaseCardAbility
{
    public override void Initialize(CardData setCard)
    {
        sourceData = setCard;
        keywordAbilities.Add(new KeywordAbilityInstance(GameEnum.KeywordAbility.Rush, null));
    }
}
